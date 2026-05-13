using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Ustalar.Data;
using Ustalar.Services;
using Microsoft.EntityFrameworkCore;

namespace Ustalar.Pages.Register;

[EnableRateLimiting(RateLimitPolicies.Sms)]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly SmsVerificationService _smsVerification;
    private readonly MasterAuthService _masterAuth;
    private readonly IWebHostEnvironment _env;

    public IndexModel(ApplicationDbContext db, SmsVerificationService smsVerification,
        MasterAuthService masterAuth, IWebHostEnvironment env)
    {
        _db = db;
        _smsVerification = smsVerification;
        _masterAuth = masterAuth;
        _env = env;
    }

    [BindProperty]
    [Required(ErrorMessage = "Telefon nömrəsi tələb olunur")]
    public string Phone { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? DevCode { get; set; } // только в Development

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var phone = Phone.Trim();

        if (!Regex.IsMatch(phone, @"^\+994\d{9}$"))
        {
            ErrorMessage = "Nömrə formatı: +994XXXXXXXXX";
            return Page();
        }

        var existing = await _db.Masters
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Phone == phone);

        if (existing != null)
        {
            await _masterAuth.SignInAsync(phone);
            return RedirectToPage("/Cabinet/Index");
        }

        if (_env.IsDevelopment())
        {
            // В Development: сохраняем код в БД но не отправляем SMS
            await _smsVerification.SaveCodeWithoutSendingAsync(phone);
            var code = await _smsVerification.GetLastCodeAsync(phone);
            DevCode = code;
            TempData["RegisterPhone"] = phone;
            return Page(); // показываем код прямо на странице
        }

        await _smsVerification.SendCodeAsync(phone);
        TempData["RegisterPhone"] = phone;
        return RedirectToPage("/Register/Step2");
    }
}
