using LiteDB;
using PocClientSync.Models;
using SSync.Client.LitebDB.Sync;

namespace PocClientSync.Repositories
{
    public class NoteRepo : INoteRepo
    {
     
        public List<Note> GetNotes()
        {
            using var db = new LiteDatabase(Database.GetPath());
            return db!
                .GetCollection<Note>(Note.CollectionName)
                .FindAll()
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
        }

        public Note GetNoteBydId(Guid id)
        {
            using var db = new LiteDatabase(Database.GetPath());
            return db!
                .GetCollection<Note>(Note.CollectionName)
                .FindById(id);
        }

        public Task Save(Note entity)
        {
            using var db = new LiteDatabase(Database.GetPath());
            var sync = new Synchronize(db);
            sync!.InsertSync(entity, Note.CollectionName);

            return Task.CompletedTask;
        }

        public Task Update(Note entity)
        {
            using var db = new LiteDatabase(Database.GetPath());
            var sync = new Synchronize(db);
            sync!.UpdateSync(entity, Note.CollectionName);

            return Task.CompletedTask;
        }


        public Task Delete(Note entity)
        {
            using var db = new LiteDatabase(Database.GetPath());
            var sync = new Synchronize(db);
            sync!.DeleteSync(entity, Note.CollectionName);

            return Task.CompletedTask;
        }


        public Task Drop()
        {
            using var db = new LiteDatabase(Database.GetPath());

            db!.GetCollection<Note>().DeleteAll();
            return Task.CompletedTask;
        }







    }
}
