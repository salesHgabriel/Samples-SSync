using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using PocServerSync.Models;
using SSync.Server.LitebDB.Abstractions.Sync;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Sync.Handlers.Push
{
    public class NotePushRequestHandler(PocDbContext context) : ISSyncPushRequest<NoteSync>
    {
        private readonly PocDbContext _context = context;

        public async Task<NoteSync?> FindByIdAsync(Guid id)
        {
            return await _context.Notes.Where(u => u.Id == id)
                .Select(u => new NoteSync(id)
                {
                    Completed = u.Completed,
                    Message = u.Message,
                    UserName = u.User!.Name,
                    CreatedAt = u.CreatedAt,
                    DeletedAt = u.DeletedAt,
                    UpdatedAt = u.UpdatedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(NoteSync schema)
        {
            var userId = await _context.Users
                .Where(s => s.Name == schema.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var newNote = new Note(schema.Id, Time.UTC)
            {
                Completed = schema.Completed,
                Message = schema.Message,
                UserId = userId
            };

            await _context.Notes.AddAsync(newNote);

            return await Save();
        }

        public async Task<bool> UpdateAsync(NoteSync schema)
        {
            var entity = await _context.Notes.FindAsync(schema.Id);

            entity!.Completed = schema.Completed;
            entity.Message = schema.Message;

            entity.SetUpdatedAt(DateTime.UtcNow);

            _context.Notes.Update(entity);

            return await Save();
        }

        public async Task<bool> DeleteAsync(NoteSync schema)
        {
            var entity = await _context.Notes.FindAsync(schema.Id);

            entity!.Completed = schema.Completed;
            entity.Message = schema.Message;

            entity.SetDeletedAt(DateTime.UtcNow);

            _context.Notes.Update(entity);

            return await Save();
        }

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}