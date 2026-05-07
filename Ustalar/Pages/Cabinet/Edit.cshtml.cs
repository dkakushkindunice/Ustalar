using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ustalar.Data;

namespace Ustalar.Pages.Cabinet;

[Authorize(AuthenticationSchemes = "MasterCookie")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string? About { get; set; }
        public string? Whatsapp { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var masterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var master = await _db.Masters.AsNoTracking().FirstOrDefaultAsync(m => m.Id == masterId);
        if (master == null) return RedirectToPage("/Register/Index");

        Input = new InputModel
        {
            About = master.About,
            Whatsapp = master.Whatsapp,
            ExperienceYears = master.ExperienceYears,
            PriceFrom = master.PriceFrom,
            PriceTo = master.PriceTo
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var masterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var master = await _db.Masters.FirstOrDefaultAsync(m => m.Id == masterId);
        if (master == null) return RedirectToPage("/Register/Index");

        master.About = Input.About?.Trim();
        master.Whatsapp = Input.Whatsapp?.Trim();
        master.ExperienceYears = Input.ExperienceYears;
        master.PriceFrom = Input.PriceFrom;
        master.PriceTo = Input.PriceTo;
        master.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("/Cabinet/Index");
    }
}
