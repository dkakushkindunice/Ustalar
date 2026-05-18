using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Admin;

[Authorize(AuthenticationSchemes = "AdminCookie")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly AdminAuthService _auth;
    private const int PageSize = 20;

    public IndexModel(ApplicationDbContext db, AdminAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public List<Master> Masters { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string CurrentStatus { get; set; } = "";
    public int PendingCount { get; set; }
    public int PendingReviewsCount { get; set; }

    public async Task OnGetAsync(string? status, int page = 1)
    {
        CurrentStatus = status ?? "";
        CurrentPage = page < 1 ? 1 : page;

        PendingCount = await _db.Masters.CountAsync(m => m.Status == MasterStatus.Pending);
        PendingReviewsCount = await _db.Reviews.CountAsync(r => !r.IsApproved);

        var query = _db.Masters
            .Include(m => m.City)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<MasterStatus>(status, out var parsed))
            query = query.Where(m => m.Status == parsed);

        var total = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        Masters = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var master = await _db.Masters.FirstOrDefaultAsync(m => m.Id == id);
        if (master != null)
        {
            master.Status = MasterStatus.Active;
            master.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBlockAsync(int id)
    {
        var master = await _db.Masters.FirstOrDefaultAsync(m => m.Id == id);
        if (master != null)
        {
            master.Status = MasterStatus.Blocked;
            master.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync();
        return RedirectToPage("/Admin/Login");
    }
}
