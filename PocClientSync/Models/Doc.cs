using SSync.Client.SQLite.Abstractions.Sync;
using SSync.Client.SQLite.Enums;
using System.Text.Json.Serialization;


namespace PocClientSync.Models;

public class Doc : SchemaSync
{
    public const string CollectionName = "ss_tb_doc";


    public Doc()
    {
            
    }
    public Doc(Guid id, Time? time) : base(id, time)
    {
    }

    public string? Name { get; set; }
    public string? FileName { get; set; }
    
    [JsonIgnore]
    public string? Path { get; set; }
}