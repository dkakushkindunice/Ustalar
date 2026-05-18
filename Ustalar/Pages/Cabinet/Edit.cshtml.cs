using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Ustalar.Data;
using Ustalar.Services;

namespace Ustalar.Pages.Cabinet;

[Authorize(AuthenticationSchemes = "MasterCookie")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly ImageProcessingService _imaging;
    private readonly IFileStorageService _storage;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public EditModel(ApplicationDbContext db, ImageProcessingService imaging, IFileStorageService storage, IStringLocalizer<SharedResource> localizer)
    {
        _db = db;
        _imaging = imaging;
        _storage = storage;
        _localizer = localizer;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? AvatarUrl { get; set; }
    public string? AvatarError { get; set; }

    public class InputModel
    {
        public string? About { get; set; }
        public string? Whatsapp { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var masterId = GetMasterId();
        var master = await _db.Masters.AsNoTracking().FirstOrDefaultAsync(m => m.Id == masterId);
        if (master == null) return RedirectToPage("/Register/Index");

        AvatarUrl = master.AvatarUrl;
        Input = new InputModel
        {
            About = master.About,
            Whatsapp = master.Whatsapp,
            ExperienceYears = master.ExperienceYears,
            PriceFrom = master.PriceFrom,
            PriceTo = master.PriceTo
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var masterId = GetMasterId();
        var master = await _db.Masters.FirstOrDefaultAsync(m => m.Id == masterId);
        if (master == null) return RedirectToPage("/Register/Index");

        master.About = Input.About?.Trim();
        master.Whatsapp = Input.Whatsapp?.Trim();
        master.ExperienceYears = Input.ExperienceYears;
        master.PriceFrom = Input.PriceFrom;
        master.PriceTo = Input.PriceTo;
        master.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("/Cabinet/Index");
    }

    public async Task<IActionResult> OnPostUploadAvatarAsync(IFormFile avatar)
    {
        var masterId = GetMasterId();
        var master = await _db.Masters.FirstOrDefaultAsync(m => m.Id == masterId);
        if (master == null) return RedirectToPage("/Register/Index");

        if (avatar == null || avatar.Length == 0)
        {
            AvatarError = _localizer["PhotoRequired"];
            await LoadViewData(master);
            return Page();
        }

        try
        {
            var (stream, contentType) = await _imaging.ProcessAvatarAsync(avatar);

            if (!string.IsNullOrEmpty(master.AvatarUrl))
                await _storage.DeleteAsync(master.AvatarUrl);

            var url = await _storage.UploadAsync(stream, contentType, "avatars");
            master.AvatarUrl = url;
            master.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
        catch (InvalidOperationException ex)
        {
            AvatarError = ex.Message;
            await LoadViewData(master);
            return Page();
        }
    }

    private async Task LoadViewData(Ustalar.Models.Master master)
    {
        AvatarUrl = master.AvatarUrl;
        var dbMaster = await _db.Masters.AsNoTracking().FirstOrDefaultAsync(m => m.Id == master.Id);
        Input = new InputModel
        {
            About = dbMaster?.About,
            Whatsapp = dbMaster?.Whatsapp,
            ExperienceYears = dbMaster?.ExperienceYears,
            PriceFrom = dbMaster?.PriceFrom,
            PriceTo = dbMaster?.PriceTo
        };
    }

    private int GetMasterId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
