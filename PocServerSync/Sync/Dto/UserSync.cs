using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Sync;

public class UserSync : ISchema
{
    public UserSync(Guid id) : base(id)
    {
    }
    
    public string? Name { get; set; }
    
    public int Age { get; set; }
}