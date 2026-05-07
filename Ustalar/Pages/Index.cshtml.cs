using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Specialization> PopularSpecs { get; set; } = [];
    public List<Master> FeaturedMasters { get; set; } = [];

    public async Task OnGetAsync()
    {
        PopularSpecs = await _db.Specializations
            .AsNoTracking()
            .OrderBy(s => s.NameAz)
            .Take(10)
            .ToListAsync();

        FeaturedMasters = await _db.Masters
            .Where(m => m.Status == MasterStatus.Active && m.IsVerified)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .AsNoTracking()
            .OrderBy(_ => EF.Functions.Random())
            .Take(6)
            .ToListAsync();
    }
}
