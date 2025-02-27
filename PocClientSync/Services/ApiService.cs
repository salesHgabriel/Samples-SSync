using Flurl;
using Flurl.Http;
using PocClientSync.Models;
using PocClientSync.Repositories;
using SQLite;
using SSync.Client.SQLite.Sync;
using System.Net.Http.Headers;

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
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            using var sync = new Synchronize(db);
            //get local database
            var time = await sync.GetLastPulledAtAsync();

            var localDatabaseChanges = await _syncRepository.PullLocalChangesToServer(time);

            Guid? userIdLogged = null; //get id user logged for example

            //send local database to server
            var result = await EndPoint.BaseURL
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

           await sync.ReplaceLastPulledAtAsync(dta.Date);

            return time;
        }

        // call method after push sync data, to use same timestamp of sync data
        public async Task<List<Doc>> PushDocsToServerAsync(DateTime timestamp)
        {
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            using var sync = new Synchronize(db);

            var synDocs = await sync!.PullChangesResultAsync<Doc>(timestamp, Doc.CollectionName);

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
                
                var fileStream = File.OpenRead(doc.Path!);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "files", Path.GetFileName(doc.Path)!);
                
                //send files to server
                var result = await EndPoint.BaseURL
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
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags); ;
            using var sync = new Synchronize(db);

            // get server database
            var time = all ? DateTime.MinValue : await sync.GetLastPulledAtAsync();
            var result = await EndPoint.BaseURL
                .AppendPathSegment("api/Sync/Pull")
                .AppendQueryParam("Colletions", User.CollectionName)
                .AppendQueryParam("Colletions", Note.CollectionName)
                .AppendQueryParam("Timestamp", time.ToString("o"))
                .GetAsync();

            var res = await result.ResponseMessage.Content.ReadAsStringAsync();

            //update local database from server

            await _syncRepository.PushServerChangesToLocal(res);
        }
    }
}