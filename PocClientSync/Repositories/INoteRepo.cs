
using PocClientSync.Models;

namespace PocClientSync.Repositories
{
    public interface INoteRepo
    {
        Task Delete(Note entity);
        Task Drop();
        Note GetNoteBydId(Guid id);
        List<Note> GetNotes();
        Task Save(Note entity);
        Task Update(Note entity);
    }
}
