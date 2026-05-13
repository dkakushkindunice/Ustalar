using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Localization;

namespace Ustalar.Services;

public sealed class ResxLocalizer : IStringLocalizer<SharedResource>
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _data;

    public ResxLocalizer(IWebHostEnvironment env)
    {
        var result = new Dictionary<string, IReadOnlyDictionary<string, string>>();
        foreach (var culture in new[] { "az", "ru" })
        {
            var path = Path.Combine(env.ContentRootPath, "Resources", $"SharedResource.{culture}.resx");
            if (!File.Exists(path)) continue;
            var dict = XDocument.Load(path)
                .Descendants("data")
                .Where(el => el.Attribute("name") != null && el.Element("value") != null)
                .ToDictionary(
                    el => el.Attribute("name")!.Value,
                    el => el.Element("value")!.Value,
                    StringComparer.OrdinalIgnoreCase);
            result[culture] = dict;
        }
        _data = result;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (_data.TryGetValue(culture, out var dict) && dict.TryGetValue(name, out var value))
                return new LocalizedString(name, value);
            if (_data.TryGetValue("az", out var fallback) && fallback.TryGetValue(name, out var fb))
                return new LocalizedString(name, fb);
            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var ls = this[name];
            return new LocalizedString(name, string.Format(ls.Value, arguments), ls.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (_data.TryGetValue(culture, out var dict))
            return dict.Select(kv => new LocalizedString(kv.Key, kv.Value));
        return [];
    }
}
