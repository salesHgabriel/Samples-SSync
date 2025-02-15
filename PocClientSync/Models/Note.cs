using SSync.Client.LitebDB.Abstractions.Sync;
using SSync.Client.LitebDB.Enums;

namespace PocClientSync.Models
{
    public class Note : SchemaSync
    {
        public const string CollectionName = "ss_tb_note";


        public Note()
        {
            
        }
        public Note(Guid id, Time? time) : base(id, time)
        {
        }

        public bool Completed { get; set; }
        public string? Message { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
