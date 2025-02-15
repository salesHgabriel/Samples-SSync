namespace PocServerSync.Services;

public interface IFileService
{
    Task<string> AddOrUpdateFileAsync(IFormFile file, string fileName);
}