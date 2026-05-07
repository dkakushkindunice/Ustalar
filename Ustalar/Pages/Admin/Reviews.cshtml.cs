using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;
using Ustalar.Models;

namespace Ustalar.Pages.Admin;

[Authorize(AuthenticationSchemes = "AdminCookie")]
public class ReviewsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ReviewsModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Review> PendingReviews { get; set; } = [];

    public async Task OnGetAsync()
    {
        PendingReviews = await _db.Reviews
            .Where(r => !r.IsApproved)
            .Include(r => r.Master).ThenInclude(m => m.City)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review != null)
        {
            review.IsApproved = true;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id)
    {
        var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review != null)
        {
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
