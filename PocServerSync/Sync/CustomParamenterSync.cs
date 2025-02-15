using SSync.Server.LitebDB.Engine;

namespace PocServerSync.Sync;

public class CustomParamenterSync : SSyncParameter
{
    public Guid? UserId { get; set; }
    public string? phoneId { get; set; }
}