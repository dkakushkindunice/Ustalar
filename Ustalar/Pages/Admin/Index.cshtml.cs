using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Services;

namespace Ustalar.Pages.Admin;

[Authorize(AuthenticationSchemes = "AdminCookie")]
public class IndexModel : PageModel
{
    private readonly AdminAuthService _auth;

    public IndexModel(AdminAuthService auth)
    {
        _auth = auth;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync();
        return RedirectToPage("/Admin/Login");
    }
}
