
using PocClientSync.Models;
using SQLite;
using SSync.Client.SQLite.Sync;


namespace PocClientSync.Repositories
{
    public class SyncRepository : ISyncRepository
    {
        public async Task<string> PullLocalChangesToServer(DateTime lastPulledAt)
        {
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            using var sync = new Synchronize(db);

            var pullChangesBuilder = new SyncPullBuilder();

            var last = await sync!.GetLastPulledAtAsync();

           await pullChangesBuilder
                .AddPullSync(() => sync!.PullChangesResultAsync<User>(last, User.CollectionName))
                .AddPullSync(() => sync!.PullChangesResultAsync<Note>(last, Note.CollectionName))
                .AddPullSync(() => sync!.PullChangesResultAsync<Doc>(last, Doc.CollectionName))
                .BuildAsync();

            var databaseLocal = pullChangesBuilder.DatabaseLocalChanges;
            var jsonDatabaseLocal = pullChangesBuilder.JsonDatabaseLocalChanges;

         

            return jsonDatabaseLocal!;
        }

        //Load database server to my local
        public async Task PushServerChangesToLocal(string jsonServerChanges)
        {
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            using var sync = new Synchronize(db);

            var pushBuilder = new SyncPushBuilder(jsonServerChanges);

            await pushBuilder
                .AddPushSchemaSync<User>(sync!.PushChangesResultAsync, User.CollectionName)
                .AddPushSchemaSync<Note>(sync!.PushChangesResultAsync, Note.CollectionName)
                .BuildAsync();
        }
    }
}
