using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Cabinet;

[Authorize(AuthenticationSchemes = "MasterCookie")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly MasterAuthService _auth;
    private readonly IFileStorageService _storage;

    public IndexModel(ApplicationDbContext db, MasterAuthService auth, IFileStorageService storage)
    {
        _db = db;
        _auth = auth;
        _storage = storage;
    }

    public Master Master { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        var masterId = GetMasterId();

        var master = await _db.Masters
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .Include(m => m.PortfolioPhotos.Where(p => true))
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == masterId);

        if (master == null) return RedirectToPage("/Register/Index");

        Master = master;
        return Page();
    }

    public async Task<IActionResult> OnPostDeletePhotoAsync(int photoId)
    {
        var masterId = GetMasterId();
        var photo = await _db.PortfolioPhotos
            .FirstOrDefaultAsync(p => p.Id == photoId && p.MasterId == masterId);

        if (photo != null)
        {
            await _storage.DeleteAsync(photo.ImageUrl);
            await _storage.DeleteAsync(photo.ThumbnailUrl);
            _db.PortfolioPhotos.Remove(photo);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync();
        return RedirectToPage("/Register/Index");
    }

    private int GetMasterId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
