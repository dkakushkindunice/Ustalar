namespace Ustalar.Services;

public static class HtmxExtensions
{
    public static bool IsHtmx(this HttpRequest request) =>
        request.Headers.ContainsKey("HX-Request");
}
