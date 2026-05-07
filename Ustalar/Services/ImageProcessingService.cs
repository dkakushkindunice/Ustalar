using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Ustalar.Services;

public class ImageProcessingService
{
    private static readonly string[] AllowedSignatures =
    [
        "\xFF\xD8\xFF",       // JPEG
        "\x89PNG",             // PNG
        "RIFF",                // WebP (проверяется вместе с WEBP)
    ];

    public async Task<(Stream Original, Stream Thumbnail, string ContentType)>
        ProcessUploadAsync(IFormFile file)
    {
        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("Fayl 10MB-dan böyükdür");

        // Валидация MIME по содержимому, не расширению
        var detectedType = await DetectMimeTypeAsync(file);
        if (detectedType == null)
            throw new InvalidOperationException("Yalnız JPEG, PNG, WebP yüklənə bilər");

        // Thumbnail 400x300 WebP
        using var image = await Image.LoadAsync(file.OpenReadStream());
        var thumb = new MemoryStream();
        var clone = image.Clone(ctx =>
            ctx.Resize(new ResizeOptions
            {
                Size = new Size(400, 300),
                Mode = ResizeMode.Crop
            }));
        await clone.SaveAsync(thumb, new WebpEncoder { Quality = 80 });
        thumb.Position = 0;

        // Оригинал в WebP
        var original = new MemoryStream();
        var resized = image.Clone(ctx =>
            ctx.Resize(new ResizeOptions
            {
                Size = new Size(1200, 900),
                Mode = ResizeMode.Max
            }));
        await resized.SaveAsync(original, new WebpEncoder { Quality = 85 });
        original.Position = 0;

        return (original, thumb, "image/webp");
    }

    public async Task<(Stream Avatar, string ContentType)> ProcessAvatarAsync(IFormFile file)
    {
        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("Fayl 10MB-dan böyükdür");

        var detectedType = await DetectMimeTypeAsync(file);
        if (detectedType == null)
            throw new InvalidOperationException("Yalnız JPEG, PNG, WebP yüklənə bilər");

        using var image = await Image.LoadAsync(file.OpenReadStream());
        var avatar = new MemoryStream();
        var square = image.Clone(ctx =>
            ctx.Resize(new ResizeOptions
            {
                Size = new Size(300, 300),
                Mode = ResizeMode.Crop
            }));
        await square.SaveAsync(avatar, new WebpEncoder { Quality = 85 });
        avatar.Position = 0;

        return (avatar, "image/webp");
    }

    private static async Task<string?> DetectMimeTypeAsync(IFormFile file)
    {
        var buffer = new byte[12];
        await using var stream = file.OpenReadStream();
        await stream.ReadExactlyAsync(buffer);

        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return "image/jpeg";
        if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
            return "image/png";
        if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46
            && buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)
            return "image/webp";

        return null;
    }
}
