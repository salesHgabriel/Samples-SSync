
using PocClientSync.Models;

namespace PocClientSync.Repositories
{
    public interface IUserRepo
    {
        Task Delete(User entity);
        Task Drop();
        Task<User> GetBydId(Guid id);
        Task<List<User>> GetUsers();
        Task SaveAsync(User entity);
        Task Update(User entity);
    }
}
