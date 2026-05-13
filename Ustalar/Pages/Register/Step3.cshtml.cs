using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ustalar.Data;
using Ustalar.Models;
using Ustalar.Services;
using Microsoft.EntityFrameworkCore;
using Ustalar.Helpers;

namespace Ustalar.Pages.Register;

public class Step3Model : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly MasterAuthService _masterAuth;

    public Step3Model(ApplicationDbContext db, MasterAuthService masterAuth)
    {
        _db = db;
        _masterAuth = masterAuth;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Cities { get; set; } = []; 
    public List<SelectListItem> Specializations { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public int CityId { get; set; }

        public List<int> SpecializationIds { get; set; } = [];

        public string? About { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Whatsapp { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var phone = TempData.Peek("RegisterPhone") as string;
        var verified = TempData.Peek("PhoneVerified") as bool?;

        if (string.IsNullOrEmpty(phone) || verified != true)
            return RedirectToPage("/Register/Index");

        await LoadSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var phone = TempData.Peek("RegisterPhone") as string;
        var verified = TempData.Peek("PhoneVerified") as bool?;

        if (string.IsNullOrEmpty(phone) || verified != true)
            return RedirectToPage("/Register/Index");

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        if (Input.SpecializationIds.Count == 0)
        {
            ErrorMessage = "Ən az bir ixtisas seçin";
            await LoadSelectListsAsync();
            return Page();
        }

        var cityExists = await _db.Cities.AnyAsync(c => c.Id == Input.CityId);
        if (!cityExists)
        {
            ErrorMessage = "Düzgün şəhər seçin";
            await LoadSelectListsAsync();
            return Page();
        }

        var master = new Master
        {
            FullName = Input.FullName.Trim(),
            Phone = phone,
            CityId = Input.CityId,
            About = Input.About?.Trim(),
            ExperienceYears = Input.ExperienceYears,
            Whatsapp = Input.Whatsapp?.Trim(),
            PriceFrom = Input.PriceFrom,
            PriceTo = Input.PriceTo,
            Status = MasterStatus.Pending,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Masters.Add(master);
        await _db.SaveChangesAsync();

        var validSpecIds = await _db.Specializations
            .Where(s => Input.SpecializationIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync();

        foreach (var specId in validSpecIds)
        {
            _db.MasterSpecializations.Add(new MasterSpecialization
            {
                MasterId = master.Id,
                SpecializationId = specId
            });
        }

        await _db.SaveChangesAsync();

        // Очищаем TempData и входим
        TempData.Remove("RegisterPhone");
        TempData.Remove("PhoneVerified");

        await _masterAuth.SignInAsync(phone);
        return RedirectToPage("/Cabinet/Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var cities = await _db.Cities.AsNoTracking().OrderBy(c => c.NameAz).ToListAsync();
        Cities = cities.Select(c => new SelectListItem(c.GetName(), c.Id.ToString())).ToList();

        var specs = await _db.Specializations.AsNoTracking().OrderBy(s => s.NameAz).ToListAsync();
        Specializations = specs.Select(s => new SelectListItem(s.GetName(), s.Id.ToString())).ToList();
    }
}
