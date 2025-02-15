using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using SSync.Server.LitebDB.Abstractions.Sync;

namespace PocServerSync.Sync.Handlers.Pull;

public class UserPullRequestHandler(ILogger<UserPullRequestHandler> logger, PocDbContext context)
    : ISSyncPullRequest<UserSync, CustomParamenterSync>
{
    public async Task<IEnumerable<UserSync>> QueryAsync(CustomParamenterSync parameter)
    {
        logger.LogInformation("QueryAsync");

        var users = await context.Users.Select(u => new UserSync(u.Id)
        {
            Age = u.Age,
            DeletedAt = u.DeletedAt,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            Name = u.Name,
        }).ToListAsync();

        return users;
    }
}