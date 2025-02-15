using LiteDB;
using PocClientSync.Models;
using SSync.Client.LitebDB.Sync;

namespace PocClientSync.Repositories
{
    public class UserRepo : IUserRepo
    {

        public User GetBydId(Guid id)
        {
            using var db = new LiteDatabase(Database.GetPath());
           return db!.GetCollection<User>(User.CollectionName).FindById(id);
        }

        public List<User> GetUsers()
        {
            using var db = new LiteDatabase(Database.GetPath());

            return db!
                .GetCollection<User>(User.CollectionName)
                .FindAll()
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
        }

        public Task SaveAsync(User entity)
        {
            using var db = new LiteDatabase(Database.GetPath());

            var sync = new Synchronize(db);
            sync!.InsertSync(entity, User.CollectionName);

            return Task.CompletedTask;
        }


        public Task Update(User entity)
        {
            using var db = new LiteDatabase(Database.GetPath());

            var sync = new Synchronize(db);

            sync!.UpdateSync(entity, User.CollectionName);

            return Task.CompletedTask;
        }

        public Task Delete(User entity)
        {
            using var db = new LiteDatabase(Database.GetPath());

            var sync = new Synchronize(db);

            sync!.DeleteSync(entity, User.CollectionName);

            return Task.CompletedTask;
        }

        public Task Drop()
        {
            using var db = new LiteDatabase(Database.GetPath());


            db!.GetCollection<User>().DeleteAll();
            return Task.CompletedTask;
        }

    }
}