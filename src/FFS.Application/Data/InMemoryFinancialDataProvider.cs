using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Data;

public sealed class InMemoryFinancialDataProvider : IFinancialDataProvider
{
    public event Action? Changed;
    private readonly List<Account> _accounts;
    private readonly List<Category> _categories;
    private readonly List<Transaction> _transactions;
    private readonly List<Goal> _goals;
    private readonly List<GoalTransaction> _goalTransactions;
    private readonly List<Budget> _budgets;
    private readonly List<BudgetItem> _budgetItems;
    private readonly List<NotificationSource> _notificationSources;

    public InMemoryFinancialDataProvider()
    {
        var seed = FfsDemoSeed.Create();
        _accounts = seed.Accounts;
        _categories = seed.Categories;
        _transactions = seed.Transactions;
        _goals = seed.Goals;
        _goalTransactions = seed.GoalTransactions;
        _budgets = seed.Budgets;
        _budgetItems = seed.BudgetItems;
        _notificationSources = seed.NotificationSources;
    }

    public IReadOnlyList<Account> Accounts => _accounts;
    public IReadOnlyList<Category> Categories => _categories;
    public IReadOnlyList<Transaction> Transactions => _transactions;
    public IReadOnlyList<Goal> Goals => _goals;
    public IReadOnlyList<GoalTransaction> GoalTransactions => _goalTransactions;
    public IReadOnlyList<Budget> Budgets => _budgets;
    public IReadOnlyList<BudgetItem> BudgetItems => _budgetItems;
    public IReadOnlyList<NotificationSource> NotificationSources => _notificationSources;

    public Transaction? GetTransaction(long id) =>
        _transactions.FirstOrDefault(t => t.Id == id);

    public void UpdateTransaction(Transaction transaction)
    {
        var index = _transactions.FindIndex(t => t.Id == transaction.Id);
        if (index < 0)
        {
            throw new InvalidOperationException($"Transaction {transaction.Id} not found.");
        }

        _transactions[index] = transaction;
        Changed?.Invoke();
    }

    public Transaction AddMoney(MoneyDirection direction)
    {
        var account = _accounts.First(a => a.IsActive);
        var txn = new Transaction
        {
            Id = NextId(_transactions, t => t.Id),
            AccountId = account.Id,
            TransactionDate = DateTime.Now,
            Merchant = direction == MoneyDirection.MoneyIn ? "Salary top-up" : "Card purchase",
            Description = "Added from the test lab",
            Amount = direction == MoneyDirection.MoneyIn ? 2500m : 186.50m,
            Direction = direction,
            Status = TransactionStatus.Confirmed,
            CategoryId = _categories.FirstOrDefault(c => c.IsActive)?.Id
        };
        _transactions.Add(txn);
        Changed?.Invoke();
        return txn;
    }

    public long? GoalIdForTransaction(long transactionId) =>
        _goalTransactions.FirstOrDefault(g => g.TransactionId == transactionId)?.GoalId;

    public void SetTransactionGoal(long transactionId, long? goalId)
    {
        var txn = GetTransaction(transactionId)
            ?? throw new InvalidOperationException($"Transaction {transactionId} not found.");

        foreach (var row in _goalTransactions.Where(g => g.TransactionId == transactionId).ToList())
        {
            ShiftGoal(row.GoalId, -row.Amount);
            _goalTransactions.Remove(row);
        }

        if (goalId is long id)
        {
            if (_goals.All(g => g.Id != id))
                throw new InvalidOperationException($"Goal {id} not found.");

            _goalTransactions.Add(new GoalTransaction
            {
                Id = NextId(_goalTransactions, g => g.Id),
                GoalId = id,
                TransactionId = transactionId,
                Amount = txn.Amount,
                ContributionDate = txn.TransactionDate,
                Notes = txn.Merchant
            });
            ShiftGoal(id, txn.Amount);
        }

        Changed?.Invoke();
    }

    public void ReloadDemo()
    {
        var seed = FfsDemoSeed.Create();
        Replace(_accounts, seed.Accounts);
        Replace(_categories, seed.Categories);
        Replace(_transactions, seed.Transactions);
        Replace(_goals, seed.Goals);
        Replace(_goalTransactions, seed.GoalTransactions);
        Replace(_budgets, seed.Budgets);
        Replace(_budgetItems, seed.BudgetItems);
        Replace(_notificationSources, seed.NotificationSources);
        Changed?.Invoke();
    }

    public void ClearLedger()
    {
        _transactions.Clear();
        _goalTransactions.Clear();
        foreach (var goal in _goals)
            goal.CurrentAmount = 0;
        foreach (var account in _accounts)
            account.OpeningBalance = 0;
        foreach (var item in _budgetItems)
            item.PlannedAmount = 0;
        Changed?.Invoke();
    }

    private void ShiftGoal(long goalId, decimal delta)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == goalId);
        if (goal is null || delta == 0) return;
        goal.CurrentAmount = Math.Max(0, goal.CurrentAmount + delta);
    }

    private static void Replace<T>(List<T> target, List<T> source)
    {
        target.Clear();
        target.AddRange(source);
    }

    private static long NextId<T>(List<T> items, Func<T, long> id) =>
        items.Count == 0 ? 1 : items.Max(id) + 1;
}
