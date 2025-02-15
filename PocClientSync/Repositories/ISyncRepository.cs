
namespace PocClientSync.Repositories
{
    public interface ISyncRepository
    {
        string PullLocalChangesToServer(DateTime lastPulledAt);
        Task PushServerChangesToLocal(string jsonServerChanges);
    }
}
