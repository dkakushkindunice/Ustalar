using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Ustalar.Services;

namespace Ustalar.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly AdminAuthService _auth;

    public LoginModel(AdminAuthService auth)
    {
        _auth = auth;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var success = await _auth.SignInAsync(Input.Email, Input.Password);
        if (!success)
        {
            ErrorMessage = "Неверный email или пароль";
            return Page();
        }

        return RedirectToPage("/Admin/Index");
    }
}
