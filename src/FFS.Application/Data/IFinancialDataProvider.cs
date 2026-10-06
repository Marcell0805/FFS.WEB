using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Data;

public interface IFinancialDataProvider
{
    event Action? Changed;

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
    Transaction AddMoney(MoneyDirection direction);
    long? GoalIdForTransaction(long transactionId);
    void SetTransactionGoal(long transactionId, long? goalId);
    void ReloadDemo();
    void ClearLedger();
}
