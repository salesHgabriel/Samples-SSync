using PocClientSync.Models;
using SQLite;
using SSync.Client.SQLite.Sync;

namespace PocClientSync.Repositories;

public class DocRepo : IDocRepo
{
    SQLiteAsyncConnection? Db;

    async Task Init()
    {
        if (Db is not null)
            return;

        Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
        var result = await Db.CreateTableAsync<Doc>();
    }

    public async Task<Doc> GetBydId(Guid id)
    {
        await Init();
        Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);
        return await Db!.Table<Doc>().FirstAsync(s => s.Id.Equals(id));
    }

    public async Task<List<Doc>> GetDocs()
    {
        await Init();
        var Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

        return await Db!
            .Table<Doc>()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveAsync(Doc entity)
    {
        await Init();
        var Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

        var sync = new Synchronize(Db);
        await sync!.InsertSyncAsync(entity);
    }

    public async Task Update(Doc entity)
    {
        await Init();
        var Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

        var sync = new Synchronize(Db);

        await sync!.UpdateSyncAsync(entity);
    }

    public async Task Delete(Doc entity)
    {
        await Init();
        var Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

        var sync = new Synchronize(Db);

        await sync!.DeleteSyncAsync(entity);
    }

    public async Task Drop()
    {
        await Init();
        var Db = new SQLiteAsyncConnection(ContantsSqlite.DatabasePath, ContantsSqlite.Flags);

        await Db!.DeleteAllAsync<Doc>();
    }
}