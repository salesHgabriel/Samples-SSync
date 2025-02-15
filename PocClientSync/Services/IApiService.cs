
using PocClientSync.Models;

namespace PocClientSync.Services
{
    public interface IApiService
    {
        Task PullServerAsync(bool all);
        Task<DateTime> PushServerAsync();
        Task<List<Doc>> PushDocsToServerAsync(DateTime timestamp);
    }
}
