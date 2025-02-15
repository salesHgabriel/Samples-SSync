using SSync.Client.LitebDB.Abstractions.Sync;
using SSync.Client.LitebDB.Enums;

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
