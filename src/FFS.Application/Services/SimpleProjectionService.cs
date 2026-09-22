using FFS.Application.Data;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

/// <summary>
/// Naive historical average projection — not the final FFS forecasting engine.
/// </summary>
public sealed class SimpleProjectionService
{
    private readonly IFinancialDataProvider _data;
    private readonly ReportingService _reporting;

    public SimpleProjectionService(IFinancialDataProvider data, ReportingService reporting)
    {
        _data = data;
        _reporting = reporting;
    }

    public SimpleProjectionResult Project(int monthsAhead, DateTime? asOf = null)
    {
        var today = (asOf ?? DateTime.Today).Date;
        var lookbackStart = today.AddMonths(-3);
        var lookbackEnd = today.AddDays(1).AddTicks(-1);
        var recent = _reporting.CashFlow(lookbackStart, lookbackEnd);

        // Average per month over ~3 months lookback
        var avgIn = recent.MoneyIn / 3m;
        var avgOut = recent.MoneyOut / 3m;

        var currentBalance = EstimateCurrentBalance();
        var points = new List<ProjectionPoint>
        {
            new("Now", currentBalance, 0, 0)
        };

        var balance = currentBalance;
        decimal totalIn = 0, totalOut = 0;
        for (var i = 1; i <= monthsAhead; i++)
        {
            balance += avgIn - avgOut;
            totalIn += avgIn;
            totalOut += avgOut;
            var label = today.AddMonths(i).ToString("MMM yyyy");
            points.Add(new ProjectionPoint(label, balance, avgIn, avgOut));
        }

        return new SimpleProjectionResult(
            currentBalance,
            avgIn,
            avgOut,
            totalIn,
            totalOut,
            balance,
            monthsAhead,
            points);
    }

    private decimal EstimateCurrentBalance()
    {
        // Opening balances + all non-ignored cash-flow txns (transfers still move between accounts;
        // for a single "total liquid" view we sum account openings and apply MoneyIn/Out only).
        var openings = _data.Accounts.Where(a => a.IsActive).Sum(a => a.OpeningBalance);
        var adjustment = 0m;
        foreach (var txn in _data.Transactions)
        {
            if (txn.Status == TransactionStatus.Ignored) continue;
            if (!MoneyDirectionRules.AffectsCashFlow(txn.Direction)) continue;
            adjustment += MoneyDirectionRules.IsIncome(txn.Direction) ? txn.Amount : -txn.Amount;
        }

        return openings + adjustment;
    }
}

public record SimpleProjectionResult(
    decimal CurrentBalance,
    decimal AvgMonthlyIncome,
    decimal AvgMonthlyExpenses,
    decimal ProjectedIncome,
    decimal ProjectedExpenses,
    decimal ProjectedBalance,
    int Months,
    IReadOnlyList<ProjectionPoint> Points);

public record ProjectionPoint(string Label, decimal Balance, decimal Income, decimal Expenses);
