using FFS.Application.Data;
using FFS.Domain.Entities;

namespace FFS.Application.Services;

public sealed class GoalService
{
    private readonly IFinancialDataProvider _data;

    public GoalService(IFinancialDataProvider data) => _data = data;

    public IReadOnlyList<GoalView> GetActiveGoals()
    {
        return _data.Goals
            .Where(g => g.IsActive)
            .OrderByDescending(g => g.Progress)
            .Select(ToView)
            .ToList();
    }

    public GoalView? GetGoal(long id)
    {
        var goal = _data.Goals.FirstOrDefault(g => g.Id == id);
        return goal is null ? null : ToView(goal);
    }

    private GoalView ToView(Goal g)
    {
        var contributions = _data.GoalTransactions
            .Where(gt => gt.GoalId == g.Id)
            .OrderByDescending(gt => gt.ContributionDate)
            .ToList();

        var linkedIds = contributions
            .Where(c => c.TransactionId.HasValue)
            .Select(c => c.TransactionId!.Value)
            .ToHashSet();

        var related = _data.Transactions
            .Where(t =>
                linkedIds.Contains(t.Id) ||
                t.Merchant.Contains(g.Name, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(g.Name, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.TransactionDate)
            .ToList();

        var moneyIn = contributions.Sum(c => c.Amount);
        var moneyOut = related
            .Where(t => t.Direction == Domain.Enums.MoneyDirection.MoneyOut
                        && !t.Merchant.Contains(g.Name, StringComparison.OrdinalIgnoreCase)
                        && t.Description.Contains("withdraw", StringComparison.OrdinalIgnoreCase))
            .Sum(t => t.Amount);

        // V0: contributions = money into the goal; related ledger rows show account movement context
        return new GoalView(g, contributions, related, moneyIn, moneyOut);
    }
}

public record GoalView(
    Goal Goal,
    IReadOnlyList<GoalTransaction> Contributions,
    IReadOnlyList<Transaction> RelatedTransactions,
    decimal MoneyIn,
    decimal MoneyOut);
