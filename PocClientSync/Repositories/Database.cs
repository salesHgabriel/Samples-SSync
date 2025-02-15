
namespace PocClientSync.Repositories
{
    public class Database
    {
        public static string GetPath()
        {
            var path = FileSystem.Current.AppDataDirectory;

#if WINDOWS
            return Path.Combine(path, "litedbwin.db");
#else
            return Path.Combine(path, "litedb.db");
#endif
        }
    }
}