using Microsoft.EntityFrameworkCore;
using PocServerSync.Data;
using PocServerSync.Models;
using SSync.Server.LitebDB.Abstractions.Sync;
using SSync.Server.LitebDB.Enums;

namespace PocServerSync.Sync.Handlers.Push;

public class DocPushRequestHandler(PocDbContext context) : ISSyncPushRequest<DocSync>
{
    public async Task<DocSync?> FindByIdAsync(Guid id)
        {
            return await context.Docs.Where(u => u.Id == id)
                .Select(u => new DocSync(id)
                {
                 Name = u.Name ,
                 FileName = u.FileName ,
                 Path = u.Path ,
                    CreatedAt = u.CreatedAt,
                    DeletedAt = u.DeletedAt,
                    UpdatedAt = u.UpdatedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(DocSync schema)
        {

            var newEntity = new Doc(schema.Id, Time.UTC)
            {
                Name = schema.Name ,
                FileName = schema.FileName ,
                Path = schema.Path
            };

            await context.Docs.AddAsync(newEntity);

            return await Save();
        }

        public async Task<bool> UpdateAsync(DocSync schema)
        {
            var entity = await context.Docs.FindAsync(schema.Id);


            entity!.Name = schema.Name;
            entity!.FileName = schema.FileName;

            entity.SetUpdatedAt(DateTime.UtcNow);

            context.Docs.Update(entity);

            return await Save();
        }

        public async Task<bool> DeleteAsync(DocSync schema)
        {
            var entity = await context.Docs.FindAsync(schema.Id);

            entity.SetDeletedAt(DateTime.UtcNow);

            context.Docs.Update(entity);

            return await Save();
        }

        private async Task<bool> Save()
        {
            return await context.SaveChangesAsync() > 0;
        }
}