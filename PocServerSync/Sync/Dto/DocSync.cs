using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Sync;

public class DocSync : ISchema
{
    public DocSync(Guid id) : base(id)
    {
    }

    public string? Name { get; set; }
    public string? FileName { get; set; }
    public string? Path { get; set; }
}