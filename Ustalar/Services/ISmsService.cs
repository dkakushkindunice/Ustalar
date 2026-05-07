namespace Ustalar.Services;

public interface ISmsService
{
    Task SendVerificationCodeAsync(string phone, string code);
}
