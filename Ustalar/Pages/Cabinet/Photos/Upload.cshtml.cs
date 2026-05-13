using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Ustalar.Data;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Cabinet.Photos;

[Authorize(AuthenticationSchemes = "MasterCookie")]
public class UploadModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly ImageProcessingService _imaging;
    private readonly IFileStorageService _storage;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public string? ErrorMessage { get; set; }
    public int PhotoCount { get; set; }

    public UploadModel(
        ApplicationDbContext db,
        ImageProcessingService imaging,
        IFileStorageService storage,
        IStringLocalizer<SharedResource> localizer)
    {
        _db = db;
        _imaging = imaging;
        _storage = storage;
        _localizer = localizer;
    }

    public async Task OnGetAsync()
    {
        var masterId = GetMasterId();
        PhotoCount = await _db.PortfolioPhotos.CountAsync(p => p.MasterId == masterId);
    }

    public async Task<IActionResult> OnPostAsync(IFormFile photo, string? description)
    {
        var masterId = GetMasterId();
        PhotoCount = await _db.PortfolioPhotos.CountAsync(p => p.MasterId == masterId);

        if (PhotoCount >= 10)
        {
            ErrorMessage = _localizer["MaxPhotoLimit"];
            return Page();
        }

        if (photo == null || photo.Length == 0)
        {
            ErrorMessage = _localizer["PhotoRequired"];
            return Page();
        }

        try
        {
            var (original, thumb, contentType) = await _imaging.ProcessUploadAsync(photo);

            var originalUrl = await _storage.UploadAsync(original, contentType, "photos");
            var thumbUrl = await _storage.UploadAsync(thumb, contentType, "thumbs");

            _db.PortfolioPhotos.Add(new PortfolioPhoto
            {
                MasterId = masterId,
                ImageUrl = originalUrl,
                ThumbnailUrl = thumbUrl,
                Description = description?.Trim(),
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return RedirectToPage("/Cabinet/Index");
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    private int GetMasterId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
