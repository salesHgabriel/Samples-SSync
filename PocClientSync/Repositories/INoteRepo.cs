
using PocClientSync.Models;

namespace PocClientSync.Repositories
{
    public interface INoteRepo
    {
        Task Delete(Note entity);
        Task Drop();
        Task<Note> GetNoteBydId(Guid id);
        Task<List<Note>> GetNotes();
        Task Save(Note entity);
        Task Update(Note entity);
    }
}
