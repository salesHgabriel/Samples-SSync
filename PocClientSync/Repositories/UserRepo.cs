using PocClientSync.Models;
using SQLite;
using SSync.Client.SQLite.Sync;

namespace PocClientSync.Repositories
{
    public class UserRepo : IUserRepo
    {
        SQLiteAsyncConnection? Db;

        async Task Init()
        {
            if (Db is not null)
                return;

            Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            var result = await Db.CreateTableAsync<User>();
        }


        public async Task<User> GetBydId(Guid id)
        {
            await Init();
            SQLiteAsyncConnection? db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            return await db!.Table<User>().FirstAsync(e => e.Id.Equals(id));
        }

        public async Task<List<User>> GetUsers()
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            return await db!
                .Table<User>()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveAsync(User entity)
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            var sync = new Synchronize(db);
            await sync!.InsertSyncAsync(entity);
        }

        public async Task Update(User entity)
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            var sync = new Synchronize(db);

            await sync!.UpdateSyncAsync(entity);
        }

        public async Task Delete(User entity)
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            var sync = new Synchronize(db);

            await sync!.DeleteSyncAsync(entity);
        }

        public async Task Drop()
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

            await db!.DeleteAllAsync<User>();
        }
    }
}