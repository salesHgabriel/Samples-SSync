using PocClientSync.Models;

namespace PocClientSync.Repositories;

public interface IDocRepo
{
    Task Delete(Doc entity);
    Task Drop();
    Task<Doc> GetBydId(Guid id);
    Task<List<Doc>> GetDocs();
    Task SaveAsync(Doc entity);
    Task Update(Doc entity);
}