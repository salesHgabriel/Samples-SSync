using SSync.Server.LitebDB.Abstractions;

namespace PocServerSync.Sync;

public class NoteSync : ISchema
{
    public NoteSync(Guid id) : base(id)
    {
    }

    public bool Completed { get; set; }

    public string? Message { get; set; }

    public string? UserName { get; set; }
}