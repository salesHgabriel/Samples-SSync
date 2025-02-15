using LiteDB;
using PocClientSync.Models;
using SSync.Client.LitebDB.Sync;

namespace PocClientSync.Repositories;

public class DocRepo : IDocRepo
{
    public Doc GetBydId(Guid id)
    {
        using var db = new LiteDatabase(Database.GetPath());
        return db!.GetCollection<Doc>(Doc.CollectionName).FindById(id);
    }

    public List<Doc> GetDocs()
    {
        using var db = new LiteDatabase(Database.GetPath());

        return db!
            .GetCollection<Doc>(Doc.CollectionName)
            .FindAll()
            .OrderByDescending(u => u.CreatedAt)
            .ToList();
    }

    public Task SaveAsync(Doc entity)
    {
        using var db = new LiteDatabase(Database.GetPath());

        var sync = new Synchronize(db);
        sync!.InsertSync(entity, Doc.CollectionName);

        return Task.CompletedTask;
    }


    public Task Update(Doc entity)
    {
        using var db = new LiteDatabase(Database.GetPath());

        var sync = new Synchronize(db);

        sync!.UpdateSync(entity, Doc.CollectionName);

        return Task.CompletedTask;
    }

    public Task Delete(Doc entity)
    {
        using var db = new LiteDatabase(Database.GetPath());

        var sync = new Synchronize(db);

        sync!.DeleteSync(entity, Doc.CollectionName);

        return Task.CompletedTask;
    }

    public Task Drop()
    {
        using var db = new LiteDatabase(Database.GetPath());


        db!.GetCollection<Doc>().DeleteAll();
        return Task.CompletedTask;
    }
}