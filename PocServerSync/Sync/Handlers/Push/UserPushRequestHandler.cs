using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using PocServerSync.Models;
using SSync.Server.LitebDB.Abstractions.Sync;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Sync.Handlers.Push
{
    public class UserPushRequestHandler(PocDbContext context) : ISSyncPushRequest<UserSync>
    {
        private readonly PocDbContext _context = context;

        public async Task<UserSync?> FindByIdAsync(Guid id)
        {
            return await _context.Users.Where(u => u.Id == id)
                .Select(u => new UserSync(id)
                {
                    Age = u.Age,
                    Name = u.Name,
                    CreatedAt = u.CreatedAt,
                    DeletedAt = u.DeletedAt,
                    UpdatedAt = u.UpdatedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(UserSync schema)
        {
            var newUser = new User(schema.Id, Time.UTC)
            {
                Age = schema.Age,
                Name = schema.Name
            };

            await _context.Users.AddAsync(newUser);

            return await Save();
        }

        public async Task<bool> UpdateAsync(UserSync schema)
        {
            var entity = await _context.Users.FindAsync(schema.Id);

            entity!.Age = schema.Age;
            entity.Name = schema.Name;

            entity.SetUpdatedAt(DateTime.UtcNow);

            _context.Users.Update(entity);

            return await Save();
        }

        public async Task<bool> DeleteAsync(UserSync schema)
        {
            var entity = await _context.Users.FindAsync(schema.Id);

            
            entity!.SetDeletedAt(DateTime.UtcNow);

            _context.Users.Update(entity);

            return await Save();
        }

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}