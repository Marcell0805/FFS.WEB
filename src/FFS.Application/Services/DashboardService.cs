using FFS.Application.Models;
using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

public sealed class DashboardService
{
    private readonly ReportingService _reporting;
    private readonly BudgetService _budget;
    private readonly GoalService _goals;

    public DashboardService(ReportingService reporting, BudgetService budget, GoalService goals)
    {
        _reporting = reporting;
        _budget = budget;
        _goals = goals;
    }

    public DashboardViewModel GetDashboard(DateTime? asOf = null)
    {
        var today = (asOf ?? DateTime.Today).Date;
        var (start, end) = DateRangePreset.ThisMonth.Resolve(today);
        var cash = _reporting.CashFlow(start, end);
        var trendStart = today.AddMonths(-5);
        trendStart = new DateTime(trendStart.Year, trendStart.Month, 1);
        var trend = _reporting.MonthlyTrend(trendStart, end);
        var spending = _reporting.SpendingByCategory(start, end).Take(6).ToList();
        var recent = _reporting.RecentTransactions(8);
        var budget = _budget.GetCurrentMonthOverview(today);
        var goals = _goals.GetActiveGoals();

        return new DashboardViewModel(cash, trend, spending, recent, budget, goals);
    }
}

public record DashboardViewModel(
    CashFlowSummary ThisMonthCashFlow,
    IReadOnlyList<MonthlyTrendPoint> CashFlowTrend,
    IReadOnlyList<CategoryTotal> TopSpending,
    IReadOnlyList<Transaction> RecentTransactions,
    BudgetOverview Budget,
    IReadOnlyList<GoalView> Goals);
