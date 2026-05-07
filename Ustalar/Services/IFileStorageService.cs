namespace Ustalar.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string contentType, string folder = "photos");
    Task DeleteAsync(string key);
}
