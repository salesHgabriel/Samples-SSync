using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Flurl;
using Flurl.Http;
using LiteDB;
using PocClientSync.Models;
using PocClientSync.Repositories;
using SSync.Client.LitebDB.Sync;

namespace PocClientSync.Services
{
    public class ApiService : IApiService
    {
        private readonly ISyncRepository _syncRepository;

        public ApiService(ISyncRepository syncRepository)
        {
            _syncRepository = syncRepository;
        }

        public async Task<DateTime> PushServerAsync()
        {
            using var db = new LiteDatabase(Database.GetPath());
            using var sync = new Synchronize(db);
            //get local database
            var time = sync.GetLastPulledAt();
            db.Dispose();
            var localDatabaseChanges = _syncRepository.PullLocalChangesToServer(time);

            Guid? userIdLogged = null; //get id user logged for example

            //send local database to server
            var result = await "api.backend.com"
                .AppendPathSegment("api/Sync/Push")
                .AppendQueryParam("Collections", User.CollectionName)
                .AppendQueryParam("Collections", Note.CollectionName)
                .AppendQueryParam("Collections", Doc.CollectionName)
                .AppendQueryParam("UserId", userIdLogged)
                .AppendQueryParam("Timestamp", time)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-type", "application/json")
                .PostStringAsync(localDatabaseChanges);

            var resp = await result.ResponseMessage.Content.ReadAsStringAsync();

            var dta = System.Text.Json.JsonSerializer.Deserialize<DateTimeOffset>(resp);
            sync.ReplaceLastPulledAt(dta.Date);

            return time;
        }

        // call method after push sync data, to use same timestamp of sync data
        public async Task<List<Doc>> PushDocsToServerAsync(DateTime timestamp)
        {
            using var db = new LiteDatabase(Database.GetPath());

            using var sync = new Synchronize(db);

            var synDocs = sync!.PullChangesResult<Doc>(timestamp, Doc.CollectionName);

            var allDocToUpload = new List<Doc>();

            var docsCreated = synDocs.Changes.Created.Select(sdc => new Doc()
            {
                Id = sdc.Id,
                Path = sdc.Path,
                Status = sdc.Status,
                CreatedAt = sdc.CreatedAt,
                UpdatedAt = sdc.UpdatedAt,
                Name = sdc.Name,
                FileName = sdc.FileName,
                DeletedAt = sdc.DeletedAt,
            });

            var docsUpdate = synDocs.Changes.Updated.Select(sdc => new Doc()
            {
                Id = sdc.Id,
                Path = sdc.Path,
                Status = sdc.Status,
                CreatedAt = sdc.CreatedAt,
                UpdatedAt = sdc.UpdatedAt,
                Name = sdc.Name,
                FileName = sdc.FileName,
                DeletedAt = sdc.DeletedAt,
            });


            allDocToUpload.AddRange(docsCreated);

            allDocToUpload.AddRange(docsUpdate);


            var allDocToUploadError = new List<Doc>();


            foreach (var doc in allDocToUpload)
            {
                using var form = new MultipartFormDataContent();
                
                var fileStream = File.OpenRead(doc.Path);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "files", Path.GetFileName(doc.Path));
                
                //send files to server
                var result = await "api.backend.com"
                    .AppendPathSegment("api/Sync/doc-up")
                    .AppendQueryParam("DocId", doc.Id)
                    .PostMultipartAsync(mp => mp
                        .AddFile("file1", doc.Path, "application/octet-stream")                    // local file path
                        // .AddFile("file2", stream, "foo.txt")
                    );

                var resp = await result.ResponseMessage.Content.ReadAsStringAsync();

                if (result.StatusCode != 200)
                {
                    allDocToUploadError.Add(doc);
                }
            }

            return allDocToUploadError;
        }


        public async Task PullServerAsync(bool all)
        {
            using var db = new LiteDatabase(Database.GetPath());
            using var sync = new Synchronize(db);

            // get server database
            var time = all ? DateTime.MinValue : sync.GetLastPulledAt();
            var result = await "api.backend.com"
                .AppendPathSegment("api/Sync/Pull")
                .AppendQueryParam("Colletions", User.CollectionName)
                .AppendQueryParam("Colletions", Note.CollectionName)
                .AppendQueryParam("Timestamp", time.ToString("o"))
                .GetAsync();

            var res = await result.ResponseMessage.Content.ReadAsStringAsync();

            db.Dispose();

            //update local database from server

            await _syncRepository.PushServerChangesToLocal(res);
        }
    }
}