using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Pages.Masters;

public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ProfileModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Master Master { get; set; } = null!;
    public Specialization? PrimarySpec { get; set; }
    public List<PortfolioPhoto> ApprovedPhotos { get; set; } = [];
    public List<Review> ApprovedReviews { get; set; } = [];
    public double AverageRating { get; set; }
    public string CitySlug { get; set; } = string.Empty;
    public string SpecSlug { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string citySlug, string specSlug, int masterId)
    {
        CitySlug = citySlug;
        SpecSlug = specSlug;

        var master = await _db.Masters
            .Where(m => m.Id == masterId && m.Status == MasterStatus.Active)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations)
                .ThenInclude(ms => ms.Specialization)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (master == null) return NotFound();
        if (master.City?.Slug != citySlug) return NotFound();

        Master = master;
        PrimarySpec = master.MasterSpecializations
            .FirstOrDefault(ms => ms.Specialization.Slug == specSlug)?.Specialization
            ?? master.MasterSpecializations.FirstOrDefault()?.Specialization;

        ApprovedPhotos = await _db.PortfolioPhotos
            .Where(p => p.MasterId == masterId && p.IsApproved)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        ApprovedReviews = await _db.Reviews
            .Where(r => r.MasterId == masterId && r.IsApproved)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        if (ApprovedReviews.Any())
            AverageRating = ApprovedReviews.Average(r => r.Rating);

        return Page();
    }

    public IActionResult OnPost(string citySlug, string specSlug, int masterId) =>
        RedirectToPage(new { citySlug, specSlug, masterId });
}
