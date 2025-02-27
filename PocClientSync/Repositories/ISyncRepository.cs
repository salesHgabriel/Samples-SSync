
namespace PocClientSync.Repositories
{
    public interface ISyncRepository
    {
        Task<string> PullLocalChangesToServer(DateTime lastPulledAt);
        Task PushServerChangesToLocal(string jsonServerChanges);
    }
}
