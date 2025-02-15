using SSync.Server.LitebDB.Abstractions;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Models;

public class Doc : ISSyncEntityRoot
{
    public const string CollectionName = "ss_tb_doc";

    public Doc()
    {
        
    }


    public Doc(Guid id, Time time) : base(id, time)
    {
    }

    public string? Name { get; set; }
    public string? FileName { get; set; }
    public string? Path { get; set; }
}