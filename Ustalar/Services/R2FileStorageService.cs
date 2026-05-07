using Amazon.S3;
using Amazon.S3.Model;

namespace Ustalar.Services;

public class R2FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly string _publicUrl;

    private static readonly HashSet<string> AllowedMimeTypes =
        ["image/jpeg", "image/png", "image/webp"];

    public R2FileStorageService(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _bucket = configuration["R2:Bucket"]
            ?? throw new InvalidOperationException("R2:Bucket not configured");
        _publicUrl = configuration["R2:PublicUrl"]
            ?? throw new InvalidOperationException("R2:PublicUrl not configured");
    }

    public async Task<string> UploadAsync(Stream stream, string contentType, string folder = "photos")
    {
        if (!AllowedMimeTypes.Contains(contentType))
            throw new InvalidOperationException($"Недопустимый тип файла: {contentType}");

        var ext = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "bin"
        };

        var key = $"{folder}/{Guid.NewGuid():N}.{ext}";

        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true
        });

        return $"{_publicUrl}/{key}";
    }

    public async Task DeleteAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        var key = url.Replace($"{_publicUrl}/", "");
        await _s3.DeleteObjectAsync(_bucket, key);
    }
}
