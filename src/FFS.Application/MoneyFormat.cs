using System.Globalization;

namespace FFS.Application;

public static class MoneyFormat
{
    private static readonly CultureInfo Za = CultureInfo.GetCultureInfo("en-ZA");

    public static string Zar(decimal amount) =>
        string.Format(Za, "R{0:N2}", amount);

    public static string ZarCompact(decimal amount) =>
        string.Format(Za, "R{0:N0}", amount);
}
