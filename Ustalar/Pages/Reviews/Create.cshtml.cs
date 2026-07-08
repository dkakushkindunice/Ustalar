using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Ustalar.Data;
using Ustalar.Models;
using Microsoft.EntityFrameworkCore;
using Ustalar.Services;

namespace Ustalar.Pages.Reviews;

[EnableRateLimiting(RateLimitPolicies.Reviews)]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int MasterId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAuthenticated { get; set; }

    public class InputModel
    {
        [Required]
        public string ReviewerName { get; set; } = string.Empty;

        [Required]
        public string ReviewerPhone { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Text { get; set; }
    }

    public void OnGet(int masterId)
    {
        MasterId = masterId;
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        if (IsAuthenticated)
        {
            Input.ReviewerName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            Input.ReviewerPhone = User.FindFirstValue("Phone") ?? string.Empty;
        }
    }

    public async Task<IActionResult> OnPostAsync(int masterId)
    {
        MasterId = masterId;
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;

        // Если пользователь залогинен — берём данные из claims, игнорируем поля формы
        if (IsAuthenticated)
        {
            Input.ReviewerName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            Input.ReviewerPhone = User.FindFirstValue("Phone") ?? string.Empty;
            ModelState.Remove(nameof(Input.ReviewerName));
            ModelState.Remove(nameof(Input.ReviewerPhone));
        }

        if (!ModelState.IsValid) return Page();

        var master = await _db.Masters
            .Where(m => m.Id == masterId && m.Status == MasterStatus.Active)
            .Include(m => m.City)
            .Include(m => m.MasterSpecializations).ThenInclude(ms => ms.Specialization)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (master == null)
        {
            ErrorMessage = "Usta tapılmadı";
            return Page();
        }

        _db.Reviews.Add(new Review
        {
            MasterId = masterId,
            ReviewerName = Input.ReviewerName.Trim(),
            ReviewerPhone = Input.ReviewerPhone.Trim(),
            Rating = Input.Rating,
            Text = Input.Text?.Trim(),
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        TempData["ReviewSent"] = true;
        var citySlug = master.City!.Slug;
        var specSlug = master.MasterSpecializations.FirstOrDefault()?.Specialization.Slug ?? "";
        return Redirect($"/masters/{citySlug}/{specSlug}/{masterId}");
    }
}
