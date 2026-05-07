using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Pages.Admin;

[Authorize(AuthenticationSchemes = "AdminCookie")]
public class PhotosModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public PhotosModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<PortfolioPhoto> PendingPhotos { get; set; } = [];

    public async Task OnGetAsync()
    {
        PendingPhotos = await _db.PortfolioPhotos
            .Where(p => !p.IsApproved)
            .Include(p => p.Master)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var photo = await _db.PortfolioPhotos.FirstOrDefaultAsync(p => p.Id == id);
        if (photo != null)
        {
            photo.IsApproved = true;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id)
    {
        var photo = await _db.PortfolioPhotos.FirstOrDefaultAsync(p => p.Id == id);
        if (photo != null)
        {
            _db.PortfolioPhotos.Remove(photo);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
