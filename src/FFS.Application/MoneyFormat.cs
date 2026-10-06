using System.Globalization;

namespace FFS.Application;

public static class MoneyFormat
{
    private static readonly CultureInfo Za = CultureInfo.GetCultureInfo("en-ZA");

    public static string Zar(decimal amount) =>
        string.Format(Za, "R{0:N2}", amount);

    public static string ZarCompact(decimal amount) =>
        string.Format(Za, "R{0:N0}", amount);

    /// <summary>Short axis label, e.g. R450k, so large values stay inside the chart.</summary>
    public static string Axis(decimal amount)
    {
        var sign = amount < 0 ? "-" : "";
        var abs = Math.Abs(amount);
        if (abs >= 1_000_000m)
            return sign + "R" + Compact(abs / 1_000_000m, abs >= 10_000_000m) + "m";
        if (abs >= 1_000m)
            return sign + "R" + Compact(abs / 1_000m, abs >= 10_000m) + "k";
        return sign + "R" + abs.ToString("0", CultureInfo.InvariantCulture);
    }

    private static string Compact(decimal scaled, bool whole) =>
        scaled.ToString(whole ? "0" : "0.#", CultureInfo.InvariantCulture);
}
