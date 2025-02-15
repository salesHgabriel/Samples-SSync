
using PocClientSync.Models;

namespace PocClientSync.Repositories
{
    public interface IUserRepo
    {
        Task Delete(User entity);
        Task Drop();
        User GetBydId(Guid id);
        List<User> GetUsers();
        Task SaveAsync(User entity);
        Task Update(User entity);
    }
}
