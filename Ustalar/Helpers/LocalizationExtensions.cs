using System.Globalization;
using Ustalar.Models;

namespace Ustalar.Helpers;

public static class LocalizationExtensions
{
    private static bool IsRu => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru";

    public static string GetName(this City city) => IsRu ? city.NameRu : city.NameAz;
    public static string GetName(this Specialization spec) => IsRu ? spec.NameRu : spec.NameAz;
}
