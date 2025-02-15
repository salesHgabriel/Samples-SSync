using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Models
{
    public class User : ISSyncEntityRoot
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

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}