using System.Globalization;

namespace FFS.Web.Services;

public static class ProgressScale
{
    public static double Percent(decimal actual, decimal target)
    {
        if (target <= 0) return 0;
        var pct = (double)(actual / target) * 100d;
        return pct < 0 ? 0 : pct;
    }

    public static string Width(double percent) =>
        Math.Min(Math.Max(percent, 0), 100).ToString("0.##", CultureInfo.InvariantCulture);

    public static string Label(double percent) =>
        Math.Max(percent, 0).ToString("0", CultureInfo.InvariantCulture);

    public static string BarClass(double percent, string tone) =>
        percent > 100 ? "over" : tone;

    /// <summary>Under 50% is money out, 50–100% is savings, past 100% is over.</summary>
    public static string GoalTone(double percent) =>
        percent > 100 ? "over" : percent >= 50 ? "savings" : "out";
}
