namespace Ustalar.Services;

/// <summary>
/// Константы для политик Rate Limiting.
/// Используются вместо magic strings при применении атрибутов [EnableRateLimiting].
/// </summary>
public static class RateLimitPolicies
{
    /// <summary>
    /// Политика для SMS-запросов: 3 запроса за 10 минут с одного IP.
    /// </summary>
    public const string Sms = "sms";

    /// <summary>
    /// Политика для отзывов: 2 запроса за час с одного IP.
    /// </summary>
    public const string Reviews = "reviews";
}
