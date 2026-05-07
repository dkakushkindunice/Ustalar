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

    public IndexModel(ApplicationDbContext db, MasterAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public Master Master { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        var masterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync();
        return RedirectToPage("/Register/Index");
    }
}
