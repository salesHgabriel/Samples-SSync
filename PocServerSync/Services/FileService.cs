namespace PocServerSync.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
    }
    public async Task<string> AddOrUpdateFileAsync(IFormFile file, string fileName)
    {
     
        var filePath = CreateFilePath(fileName!);

        await using var stream = new FileStream(filePath, FileMode.Create);
        
        await file.CopyToAsync(stream);
        
        var request = _httpContextAccessor.HttpContext!.Request;
        
        return $"{request.Scheme}://{request?.Host}/temp/uploads-ssync/{fileName}";
    }
    
    private string CreateFilePath(string fileName)
    {
        var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "temp", "uploads-ssync");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var pathFile = Path.Combine(directoryPath, fileName);

        //replace file
        if (File.Exists(pathFile))
            File.Delete(pathFile);
        
        return pathFile;
    }
}