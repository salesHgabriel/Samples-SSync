using SSync.Client.SQLite.Abstractions.Sync;
using SSync.Client.SQLite.Enums;

namespace PocClientSync.Models
{
    public class User : SchemaSync
    {
        public const string CollectionName = "ss_tb_user";

        public User()
        {
            
        }
        public User(Guid id, Time time) : base(id, time)
        {
        }

        public string? Name { get; set; }
        public int Age { get; set; }

    }
}
