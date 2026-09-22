using FFS.Application.Data;
using FFS.Application.Services;
using FFS.Domain.Enums;

namespace FFS.Web.Services;

/// <summary>
/// Staged first-load bootstrap — mirrors FFS Mobile splash messaging.
/// Skips when sessionStorage says splash was already seen (survives F5 in the same tab).
/// </summary>
public sealed class AppBootService
{
    private readonly IFinancialDataProvider _data;
    private readonly ReportingService _reporting;
    private readonly BudgetService _budget;
    private readonly GoalService _goals;
    private readonly SimpleProjectionService _projection;
    private Task? _running;
    private bool _skipResolved;

    public AppBootService(
        IFinancialDataProvider data,
        ReportingService reporting,
        BudgetService budget,
        GoalService goals,
        SimpleProjectionService projection)
    {
        _data = data;
        _reporting = reporting;
        _budget = budget;
        _goals = goals;
        _projection = projection;
    }

    public bool IsReady { get; private set; }
    public string Status { get; private set; } = "Starting F.F.S…";
    public double Progress { get; private set; }

    public event Action? Changed;

    /// <summary>
    /// Call from JS interop result before EnsureStartedAsync.
    /// When showSplash is false, mark ready immediately (already seen this tab session).
    /// </summary>
    public void ApplySplashCache(bool showSplash)
    {
        if (_skipResolved) return;
        _skipResolved = true;
        if (showSplash) return;

        IsReady = true;
        Progress = 1;
        Status = "Ready";
        Notify();
    }

    public Task EnsureStartedAsync()
    {
        if (IsReady) return Task.CompletedTask;
        _running ??= RunAsync();
        return _running;
    }

    private async Task RunAsync()
    {
        if (IsReady) return;

        var steps = new (string Label, int MinMs, Func<Task>? Work)[]
        {
            ("Opening your ledger…", 700, async () =>
            {
                _ = _data.Accounts.Count;
                _ = _data.Transactions.Count;
                await Task.CompletedTask;
            }),
            ("Loading demo accounts…", 550, async () =>
            {
                _ = _data.Categories.Count;
                _ = _data.Budgets.Count;
                await Task.CompletedTask;
            }),
            ("Balancing cash flow…", 650, async () =>
            {
                var (start, end) = DateRangePreset.ThisMonth.Resolve();
                _ = _reporting.CashFlow(start, end);
                await Task.CompletedTask;
            }),
            ("Checking budgets & goals…", 650, async () =>
            {
                _ = _budget.GetCurrentMonthOverview();
                _ = _goals.GetActiveGoals();
                await Task.CompletedTask;
            }),
            ("Preparing your forecast…", 600, async () =>
            {
                _ = _projection.Project(6);
                await Task.CompletedTask;
            }),
            ("Almost ready…", 400, null)
        };

        var started = DateTime.UtcNow;
        for (var i = 0; i < steps.Length; i++)
        {
            if (IsReady) return;
            var (label, minMs, work) = steps[i];
            Status = label;
            Progress = (i + 0.35) / steps.Length;
            Notify();

            var stepStart = DateTime.UtcNow;
            if (work is not null)
                await work();

            var elapsed = (DateTime.UtcNow - stepStart).TotalMilliseconds;
            var remaining = minMs - elapsed;
            if (remaining > 0)
                await Task.Delay((int)remaining);

            Progress = (i + 1.0) / steps.Length;
            Notify();
        }

        var totalElapsed = (DateTime.UtcNow - started).TotalMilliseconds;
        const int minTotalMs = 2800;
        if (totalElapsed < minTotalMs)
            await Task.Delay((int)(minTotalMs - totalElapsed));

        Status = "Ready";
        Progress = 1;
        IsReady = true;
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
