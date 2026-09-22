namespace FFS.Domain.Enums;

public enum DateRangePreset
{
    ThisMonth,
    PreviousMonth,
    Last3Months,
    Last6Months,
    Last12Months
}

public static class DateRangePresetExtensions
{
    public static (DateTime Start, DateTime End) Resolve(this DateRangePreset preset, DateTime? today = null)
    {
        var now = (today ?? DateTime.Today).Date;
        var endOfToday = now.AddDays(1).AddTicks(-1);

        return preset switch
        {
            DateRangePreset.ThisMonth =>
                (new DateTime(now.Year, now.Month, 1), endOfToday),
            DateRangePreset.PreviousMonth =>
                ResolvePreviousMonth(now),
            DateRangePreset.Last3Months =>
                (now.AddMonths(-3).Date, endOfToday),
            DateRangePreset.Last6Months =>
                (now.AddMonths(-6).Date, endOfToday),
            DateRangePreset.Last12Months =>
                (now.AddMonths(-12).Date, endOfToday),
            _ => (new DateTime(now.Year, now.Month, 1), endOfToday)
        };
    }

    private static (DateTime Start, DateTime End) ResolvePreviousMonth(DateTime now)
    {
        var firstThisMonth = new DateTime(now.Year, now.Month, 1);
        var firstPrev = firstThisMonth.AddMonths(-1);
        var endPrev = firstThisMonth.AddTicks(-1);
        return (firstPrev, endPrev);
    }

    public static string DisplayName(this DateRangePreset preset) => preset switch
    {
        DateRangePreset.ThisMonth => "This Month",
        DateRangePreset.PreviousMonth => "Previous Month",
        DateRangePreset.Last3Months => "Last 3 Months",
        DateRangePreset.Last6Months => "Last 6 Months",
        DateRangePreset.Last12Months => "Last 12 Months",
        _ => preset.ToString()
    };
}
