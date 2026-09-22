using FFS.Application.Data;
using FFS.Application.Models;
using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

/// <summary>
/// Mirrors FFS Mobile ReportingService rules:
/// Ignored excluded; Transfer excluded from cash flow; spending = MoneyOut only.
/// </summary>
public sealed class ReportingService
{
    private readonly IFinancialDataProvider _data;

    public ReportingService(IFinancialDataProvider data) => _data = data;

    public CashFlowSummary CashFlow(DateTime start, DateTime end)
    {
        decimal moneyIn = 0, moneyOut = 0;
        var counted = 0;

        foreach (var txn in TransactionsInRange(start, end))
        {
            if (txn.Status == TransactionStatus.Ignored) continue;
            if (!MoneyDirectionRules.AffectsCashFlow(txn.Direction)) continue;

            counted++;
            if (MoneyDirectionRules.IsIncome(txn.Direction))
                moneyIn += txn.Amount;
            else
                moneyOut += txn.Amount;
        }

        return new CashFlowSummary(moneyIn, moneyOut, moneyIn - moneyOut, counted);
    }

    public IReadOnlyList<CategoryTotal> SpendingByCategory(DateTime start, DateTime end)
    {
        var categories = _data.Categories.ToDictionary(c => c.Id);

        return TransactionsInRange(start, end)
            .Where(t => t.Status != TransactionStatus.Ignored)
            .Where(t => t.Direction == MoneyDirection.MoneyOut)
            .GroupBy(t => t.CategoryId)
            .Select(g =>
            {
                string name = "Uncategorized";
                if (g.Key is long id && categories.TryGetValue(id, out var cat))
                    name = cat.Name;
                return new CategoryTotal(g.Key, name, g.Sum(x => x.Amount));
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public IReadOnlyList<MerchantTotal> TopMerchants(DateTime start, DateTime end, int take = 10)
    {
        var categories = _data.Categories.ToDictionary(c => c.Id);

        return TransactionsInRange(start, end)
            .Where(t => t.Status != TransactionStatus.Ignored)
            .Where(t => t.Direction == MoneyDirection.MoneyOut)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Merchant) ? "Unknown" : t.Merchant)
            .Select(g =>
            {
                var topCat = g
                    .GroupBy(x => x.CategoryId)
                    .OrderByDescending(x => x.Sum(t => t.Amount))
                    .Select(x => x.Key is long id && categories.TryGetValue(id, out var c) ? c.Name : null)
                    .FirstOrDefault();
                return new MerchantTotal(g.Key, g.Sum(x => x.Amount), g.Count(), topCat);
            })
            .OrderByDescending(x => x.Total)
            .Take(take)
            .ToList();
    }

    public IReadOnlyList<MonthlyTrendPoint> MonthlyTrend(DateTime start, DateTime end)
    {
        var points = new List<MonthlyTrendPoint>();
        var cursor = new DateTime(start.Year, start.Month, 1);
        var last = new DateTime(end.Year, end.Month, 1);

        while (cursor <= last)
        {
            var monthEnd = cursor.AddMonths(1).AddTicks(-1);
            var rangeStart = cursor < start ? start : cursor;
            var rangeEnd = monthEnd > end ? end : monthEnd;
            var summary = CashFlow(rangeStart, rangeEnd);
            points.Add(new MonthlyTrendPoint(
                cursor.ToString("MMM yyyy"),
                cursor,
                summary.MoneyIn,
                summary.MoneyOut));
            cursor = cursor.AddMonths(1);
        }

        return points;
    }

    public IReadOnlyList<DailyCashFlowPoint> PeriodCashFlow(DateTime start, DateTime end)
    {
        var days = (end.Date - start.Date).TotalDays;
        return days <= 45
            ? DailyCashFlow(start, end)
            : WeeklyCashFlow(start, end);
    }

    public IReadOnlyList<DailyCashFlowPoint> DailyCashFlow(DateTime start, DateTime end)
    {
        var map = new Dictionary<DateTime, (decimal In, decimal Out)>();
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            map[d] = (0, 0);

        foreach (var txn in TransactionsInRange(start, end))
        {
            if (txn.Status == TransactionStatus.Ignored) continue;
            if (!MoneyDirectionRules.AffectsCashFlow(txn.Direction)) continue;

            var day = txn.TransactionDate.Date;
            if (!map.ContainsKey(day)) continue;

            var (inn, outt) = map[day];
            if (MoneyDirectionRules.IsIncome(txn.Direction))
                map[day] = (inn + txn.Amount, outt);
            else
                map[day] = (inn, outt + txn.Amount);
        }

        return map
            .OrderBy(x => x.Key)
            .Select(x => new DailyCashFlowPoint(x.Key, x.Key.ToString("dd MMM"), x.Value.In, x.Value.Out))
            .ToList();
    }

    private IReadOnlyList<DailyCashFlowPoint> WeeklyCashFlow(DateTime start, DateTime end)
    {
        var points = new List<DailyCashFlowPoint>();
        var cursor = start.Date;
        while (cursor <= end.Date)
        {
            var weekEnd = cursor.AddDays(6);
            if (weekEnd > end.Date) weekEnd = end.Date;
            var summary = CashFlow(cursor, weekEnd.AddDays(1).AddTicks(-1));
            points.Add(new DailyCashFlowPoint(
                cursor,
                $"{cursor:dd MMM}–{weekEnd:dd MMM}",
                summary.MoneyIn,
                summary.MoneyOut));
            cursor = weekEnd.AddDays(1);
        }

        return points;
    }

    public IReadOnlyList<Transaction> RecentTransactions(int take = 8) =>
        _data.Transactions
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.Id)
            .Take(take)
            .ToList();

    private IEnumerable<Transaction> TransactionsInRange(DateTime start, DateTime end) =>
        _data.Transactions.Where(t => t.TransactionDate >= start && t.TransactionDate <= end);
}
