using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Ustalar.Services;

public class TwilioSmsService : ISmsService
{
    private readonly string _fromNumber;

    public TwilioSmsService(IConfiguration configuration)
    {
        var accountSid = configuration["Twilio:AccountSid"]
            ?? throw new InvalidOperationException("Twilio:AccountSid not configured");
        var authToken = configuration["Twilio:AuthToken"]
            ?? throw new InvalidOperationException("Twilio:AuthToken not configured");
        _fromNumber = configuration["Twilio:FromNumber"]
            ?? throw new InvalidOperationException("Twilio:FromNumber not configured");

        TwilioClient.Init(accountSid, authToken);
    }

    public async Task SendVerificationCodeAsync(string phone, string code)
    {
        await MessageResource.CreateAsync(
            body: $"Ustalar.az — təsdiq kodu / код подтверждения: {code}",
            from: new Twilio.Types.PhoneNumber(_fromNumber),
            to: new Twilio.Types.PhoneNumber(phone)
        );
    }
}
