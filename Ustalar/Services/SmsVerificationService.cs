using System.Security.Cryptography;
using Ustalar.Data;
using Ustalar.Models;
using Microsoft.EntityFrameworkCore;

namespace Ustalar.Services;

public class SmsVerificationService
{
    private readonly ApplicationDbContext _db;
    private readonly ISmsService _sms;

    public SmsVerificationService(ApplicationDbContext db, ISmsService sms)
    {
        _db = db;
        _sms = sms;
    }

    public async Task SendCodeAsync(string phone)
    {
        // Инвалидируем предыдущие коды для этого номера
        var previous = await _db.SmsVerifications
            .Where(v => v.Phone == phone && !v.IsUsed)
            .ToListAsync();

        foreach (var v in previous)
            v.IsUsed = true;

        var code = GenerateCode();

        _db.SmsVerifications.Add(new SmsVerification
        {
            Phone = phone,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        await _db.SaveChangesAsync();
        await _sms.SendVerificationCodeAsync(phone, code);
    }

    public async Task<bool> VerifyCodeAsync(string phone, string code)
    {
        var verification = await _db.SmsVerifications
            .Where(v => v.Phone == phone && v.Code == code && !v.IsUsed)
            .OrderByDescending(v => v.ExpiresAt)
            .FirstOrDefaultAsync();

        if (verification == null) return false;
        if (verification.ExpiresAt < DateTime.UtcNow) return false;

        verification.IsUsed = true;
        await _db.SaveChangesAsync();
        return true;
    }

    private static string GenerateCode()
    {
        var number = RandomNumberGenerator.GetInt32(100000, 999999);
        return number.ToString();
    }
}
