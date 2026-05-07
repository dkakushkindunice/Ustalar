using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;
using Ustalar.Services;

namespace Ustalar.Pages.Masters;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private const int PageSize = 12;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Master> Masters { get; set; } = [];
    public List<City> Cities { get; set; } = [];
    public List<Specialization> Specializations { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public string? CurrentCitySlug { get; set; }
    public string? CurrentSpecSlug { get; set; }

    public async Task<IActionResult> OnGetAsync(
        string? citySlug,
        string? specSlug,
        int page = 1)
    {
        CurrentCitySlug = citySlug;
        CurrentSpecSlug = specSlug;
        CurrentPage = page < 1 ? 1 : page;

        // Справочники (будут кешироваться в TASK-032)
        Cities = await _db.Cities.AsNoTracking().OrderBy(c => c.NameAz).ToListAsync();
        Specializations = await _db.Specializations.AsNoTracking().OrderBy(s => s.NameAz).ToListAsync();

        var query = _db.Masters
            .Where(m => m.Status == MasterStatus.Active)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(citySlug))
            query = query.Where(m => m.City!.Slug == citySlug);

        if (!string.IsNullOrEmpty(specSlug))
            query = query.Where(m => m.MasterSpecializations
                .Any(ms => ms.Specialization.Slug == specSlug));

        var total = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        Masters = await query
            .OrderByDescending(m => m.IsVerified)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        // HTMX-запрос — возвращаем только partial
        if (Request.IsHtmx())
            return Partial("_MastersList", this);

        return Page();
    }
}
