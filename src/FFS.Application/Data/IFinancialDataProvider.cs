using FFS.Domain.Entities;

namespace FFS.Application.Data;

public interface IFinancialDataProvider
{
    IReadOnlyList<Account> Accounts { get; }
    IReadOnlyList<Category> Categories { get; }
    IReadOnlyList<Transaction> Transactions { get; }
    IReadOnlyList<Goal> Goals { get; }
    IReadOnlyList<GoalTransaction> GoalTransactions { get; }
    IReadOnlyList<Budget> Budgets { get; }
    IReadOnlyList<BudgetItem> BudgetItems { get; }
    IReadOnlyList<NotificationSource> NotificationSources { get; }

    Transaction? GetTransaction(long id);
    void UpdateTransaction(Transaction transaction);
}
