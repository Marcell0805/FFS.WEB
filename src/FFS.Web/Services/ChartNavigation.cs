using FFS.Application.Models;
using Microsoft.AspNetCore.Components;

namespace FFS.Web.Services;

public static class ChartNavigation
{
    public static void OpenMonth(NavigationManager nav, IReadOnlyList<MonthlyTrendPoint> points, int index, int series)
    {
        if (index < 0 || index >= points.Count) return;
        var start = points[index].MonthStart.Date;
        nav.NavigateTo(Url(start, start.AddMonths(1).AddDays(-1), series));
    }

    public static void OpenDay(NavigationManager nav, IReadOnlyList<DailyCashFlowPoint> points, int index, int series)
    {
        if (index < 0 || index >= points.Count) return;
        var day = points[index].Day.Date;
        nav.NavigateTo(Url(day, day, series));
    }

    public static string Url(DateTime start, DateTime end, int series, long? categoryId = null)
    {
        var query = $"from={start:yyyy-MM-dd}&to={end:yyyy-MM-dd}";
        var direction = series switch
        {
            0 => "MoneyIn",
            1 => "MoneyOut",
            _ => null
        };
        if (direction is not null)
            query += $"&direction={direction}";
        if (categoryId is long id)
            query += $"&categoryId={id}";
        return $"transactions?{query}";
    }
}

public sealed class ChartHit
{
    public int Index { get; set; } = -1;
    public int Series { get; set; } = -1;
}
