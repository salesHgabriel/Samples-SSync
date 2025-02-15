using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Models
{
    public class Note : ISSyncEntityRoot
    {
        public const string CollectionName = "ss_tb_note";
        
        public Note()
        {
            
        }
        public Note(Guid id, Time time) : base(id, time)
        {
        }
  
        
        public bool Completed { get; set; }
        public string? Message { get; set; } 
        public Guid? UserId{ get; set; }
        public virtual User? User { get; set; }

    }
}
