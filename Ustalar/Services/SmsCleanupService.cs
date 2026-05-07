using Microsoft.EntityFrameworkCore;
using Ustalar.Data;

namespace Ustalar.Services;

public class SmsCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SmsCleanupService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

    public SmsCleanupService(IServiceScopeFactory scopeFactory, ILogger<SmsCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var deleted = await db.SmsVerifications
                    .Where(v => v.ExpiresAt < DateTime.UtcNow)
                    .ExecuteDeleteAsync(stoppingToken);

                if (deleted > 0)
                    _logger.LogInformation("SMS cleanup: removed {Count} expired codes", deleted);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "SMS cleanup failed");
            }
        }
    }
}
