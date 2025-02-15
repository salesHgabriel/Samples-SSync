
using LiteDB;
using PocClientSync.Models;
using SSync.Client.LitebDB.Sync;

namespace PocClientSync.Repositories
{
    public class SyncRepository : ISyncRepository
    {
        public string PullLocalChangesToServer(DateTime lastPulledAt)
        {
            using var db = new LiteDatabase(Database.GetPath());
            using var sync = new Synchronize(db);

            var pullChangesBuilder = new SyncPullBuilder();

            var last = sync!.GetLastPulledAt();

            pullChangesBuilder
                .AddPullSync(() => sync!.PullChangesResult<User>(last, User.CollectionName))
                .AddPullSync(() => sync!.PullChangesResult<Note>(last, Note.CollectionName))
                .AddPullSync(() => sync!.PullChangesResult<Doc>(last, Doc.CollectionName))
                .Build();

            var databaseLocal = pullChangesBuilder.DatabaseLocalChanges;
            var jsonDatabaseLocal = pullChangesBuilder.JsonDatabaseLocalChanges;

            db.Dispose();

            return jsonDatabaseLocal!;
        }

        //Load database server to my local
        public Task PushServerChangesToLocal(string jsonServerChanges)
        {
            using var db = new LiteDatabase(Database.GetPath());
            using var sync = new Synchronize(db);

            var pushBuilder = new SyncPushBuilder(jsonServerChanges);

            pushBuilder
                .AddPushSchemaSync<User>(change => sync!.PushChangesResult(change), User.CollectionName)
                .AddPushSchemaSync<Note>(change => sync!.PushChangesResult(change), Note.CollectionName)
                .Build();

            db.Dispose();

            return Task.CompletedTask;
        }
    }
}
