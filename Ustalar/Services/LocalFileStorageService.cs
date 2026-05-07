namespace Ustalar.Services;

// Used when R2 credentials are not configured (local development)
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string> UploadAsync(Stream stream, string contentType, string folder = "photos")
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsDir);

        var ext = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "bin"
        };

        var fileName = $"{Guid.NewGuid():N}.{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var fs = File.Create(filePath);
        await stream.CopyToAsync(fs);

        _logger.LogInformation("LocalStorage: saved {Folder}/{File}", folder, fileName);
        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.StartsWith("/uploads/")) return Task.CompletedTask;

        var path = Path.Combine(_env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}
