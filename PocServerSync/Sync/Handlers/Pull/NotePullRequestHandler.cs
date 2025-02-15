using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using PocServerSync.Models;
using SSync.Server.LitebDB.Abstractions.Sync;

namespace PocServerSync.Sync.Handlers.Pull;

public class NotePullRequestHandler : ISSyncPullRequest<NoteSync, CustomParamenterSync>
{
    private readonly ILogger<NotePullRequestHandler> _logger;
    private readonly PocDbContext _pocDbContext;

    public NotePullRequestHandler(ILogger<NotePullRequestHandler> logger, PocDbContext pocDbContext)
    {
        _logger = logger;
        _pocDbContext = pocDbContext;
    }

    public async Task<IEnumerable<NoteSync>> QueryAsync(CustomParamenterSync parameter)
    {
        _logger.LogInformation("Not sync  pull");

        var notes = _pocDbContext.Notes.AsQueryable();

        if (parameter.UserId.HasValue)
        {
            notes = notes.Where(x => x.UserId == parameter.UserId);
        }

        return await notes.Select(n => new NoteSync(n.Id)
        {
            Completed = n.Completed,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt,
            Message = n.Message,
            DeletedAt = n.DeletedAt,
            UserName = n.User!.Name
        }).ToListAsync();
    }
}