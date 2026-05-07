using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Ustalar.Data;
using Microsoft.EntityFrameworkCore;

namespace Ustalar.Services;

public class AdminAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminAuthService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        var admin = await _db.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email);

        if (admin == null) return false;
        if (!BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash)) return false;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, "AdminCookie");
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync("AdminCookie", principal);
        return true;
    }

    public async Task SignOutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync("AdminCookie");
    }

    public static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
}
