using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ustalar.Services;

namespace Ustalar.Pages.Cabinet;

[Authorize(AuthenticationSchemes = "MasterCookie")]
public class IndexModel : PageModel
{
    private readonly MasterAuthService _auth;

    public IndexModel(MasterAuthService auth)
    {
        _auth = auth;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _auth.SignOutAsync();
        return RedirectToPage("/Register/Index");
    }
}
