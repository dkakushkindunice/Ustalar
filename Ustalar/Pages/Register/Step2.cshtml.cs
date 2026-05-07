using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Services;

namespace Ustalar.Pages.Register;

public class Step2Model : PageModel
{
    private readonly SmsVerificationService _smsVerification;

    public Step2Model(SmsVerificationService smsVerification)
    {
        _smsVerification = smsVerification;
    }

    [BindProperty]
    [Required]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    [Required, StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        Phone = TempData.Peek("RegisterPhone") as string ?? string.Empty;
        if (string.IsNullOrEmpty(Phone))
            return RedirectToPage("/Register/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var valid = await _smsVerification.VerifyCodeAsync(Phone, Code);

        if (!valid)
        {
            ErrorMessage = "Kod yanlışdır və ya müddəti bitib";
            return Page();
        }

        TempData["RegisterPhone"] = Phone;
        TempData["PhoneVerified"] = true;
        return RedirectToPage("/Register/Step3");
    }
}
