using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Pages;

public record SitemapMaster(string CitySlug, string SpecSlug, int MasterId);

public class SitemapModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public List<City> Cities { get; set; } = [];
    public List<Specialization> Specializations { get; set; } = [];
    public List<SitemapMaster> ActiveMasters { get; set; } = [];

    public SitemapModel(ApplicationDbContext db) => _db = db;

    public async Task OnGetAsync()
    {
        Cities = await _db.Cities.AsNoTracking().ToListAsync();
        Specializations = await _db.Specializations.AsNoTracking().ToListAsync();

        ActiveMasters = await _db.Masters
            .Where(m => m.Status == MasterStatus.Active)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .AsNoTracking()
            .Select(m => new SitemapMaster(
                m.City.Slug,
                m.MasterSpecializations.Select(ms => ms.Specialization.Slug).FirstOrDefault() ?? "usta",
                m.Id))
            .ToListAsync();
    }
}
