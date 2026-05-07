using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Ustalar.Data;
using Microsoft.EntityFrameworkCore;

namespace Ustalar.Services;

public class MasterAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MasterAuthService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> SignInAsync(string phone)
    {
        var master = await _db.Masters
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Phone == phone);

        if (master == null) return false;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, master.Id.ToString()),
            new(ClaimTypes.Name, master.FullName),
            new("Phone", master.Phone)
        };

        var identity = new ClaimsIdentity(claims, "MasterCookie");
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync("MasterCookie", principal);
        return true;
    }

    public async Task SignOutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync("MasterCookie");
    }
}
