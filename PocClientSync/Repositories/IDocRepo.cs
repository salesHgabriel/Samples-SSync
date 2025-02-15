using PocClientSync.Models;

namespace PocClientSync.Repositories;

public interface IDocRepo
{
    Doc GetBydId(Guid id);
    List<Doc> GetDocs();
    Task SaveAsync(Doc entity);
    Task Update(Doc entity);
    Task Delete(Doc entity);
    Task Drop();
}