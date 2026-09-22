using FFS.Application.Data;
using FFS.Application.Models;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

public sealed class BudgetService
{
    private readonly IFinancialDataProvider _data;
    private readonly ReportingService _reporting;

    public BudgetService(IFinancialDataProvider data, ReportingService reporting)
    {
        _data = data;
        _reporting = reporting;
    }

    public BudgetOverview GetCurrentMonthOverview(DateTime? asOf = null)
    {
        var today = (asOf ?? DateTime.Today).Date;
        var budget = _data.Budgets
            .FirstOrDefault(b => b.Year == today.Year && b.Month == today.Month)
            ?? _data.Budgets.OrderByDescending(b => b.Year).ThenByDescending(b => b.Month).FirstOrDefault();

        if (budget is null)
        {
            return new BudgetOverview(today.Year, today.Month, 0, 0, [], []);
        }

        var start = new DateTime(budget.Year, budget.Month, 1);
        var end = start.AddMonths(1).AddTicks(-1);
        var cash = _reporting.CashFlow(start, end);

        var items = _data.BudgetItems.Where(i => i.BudgetId == budget.Id).ToList();
        var categories = _data.Categories.ToDictionary(c => c.Id);

        var actualByCategory = _data.Transactions
            .Where(t => t.TransactionDate >= start && t.TransactionDate <= end)
            .Where(t => t.Status != TransactionStatus.Ignored)
            .Where(t => t.Direction == MoneyDirection.MoneyOut)
            .Where(t => t.CategoryId.HasValue)
            .GroupBy(t => t.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var categoryRows = items
            .Where(i => i.CategoryId.HasValue)
            .Select(i =>
            {
                var catName = categories.TryGetValue(i.CategoryId!.Value, out var c) ? c.Name : "Unknown";
                var actual = actualByCategory.TryGetValue(i.CategoryId.Value, out var a) ? a : 0m;
                return new BudgetCategoryRow(i.CategoryId.Value, catName, i.Bucket, i.PlannedAmount, actual, i.PlannedAmount - actual);
            })
            .OrderBy(r => r.Bucket)
            .ThenBy(r => r.CategoryName)
            .ToList();

        var buckets = Enum.GetValues<BudgetBucketKind>()
            .Where(b => b != BudgetBucketKind.Unassigned)
            .Select(bucket =>
            {
                var planned = items.Where(i => i.Bucket == bucket).Sum(i => i.PlannedAmount);
                var actual = categoryRows.Where(r => r.Bucket == bucket).Sum(r => r.Actual);
                return new BudgetBucketRow(bucket, planned, actual, planned - actual);
            })
            .ToList();

        return new BudgetOverview(
            budget.Year,
            budget.Month,
            budget.IncomeTarget,
            cash.MoneyIn,
            buckets,
            categoryRows);
    }
}

public record BudgetOverview(
    int Year,
    int Month,
    decimal IncomeTarget,
    decimal ActualIncome,
    IReadOnlyList<BudgetBucketRow> Buckets,
    IReadOnlyList<BudgetCategoryRow> Categories);

public record BudgetBucketRow(BudgetBucketKind Bucket, decimal Planned, decimal Actual, decimal Remaining);

public record BudgetCategoryRow(
    long CategoryId,
    string CategoryName,
    BudgetBucketKind Bucket,
    decimal Planned,
    decimal Actual,
    decimal Remaining);
