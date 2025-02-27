using PocClientSync.Models;
using SQLite;
using SSync.Client.SQLite.Sync;

namespace PocClientSync.Repositories
{
    public class NoteRepo : INoteRepo
    {
        SQLiteAsyncConnection? Db;

        async Task Init()
        {
            if (Db is not null)
                return;

            Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            var result = await Db.CreateTableAsync<Note>();
        }

        public async Task<List<Note>> GetNotes()
        {
            await Init();
            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            return await db!
                .Table<Note>()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<Note> GetNoteBydId(Guid id)
        {
            await Init();

            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            return await db!
                .Table<Note>()
                .FirstAsync(e => e.Id.Equals(id));
        }

        public async Task Save(Note entity)
        {
            await Init();

            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            var sync = new Synchronize(db);
            await sync!.InsertSyncAsync(entity);
        }

        public async Task Update(Note entity)
        {
            await Init();

            var db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
            var sync = new Synchronize(db);
            await sync!.UpdateSyncAsync(entity);
        }

        public async Task Delete(Note entity)
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

            await db!.DeleteAllAsync<Note>();
        }
    }
}