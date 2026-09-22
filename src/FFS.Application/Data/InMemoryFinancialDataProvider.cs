using FFS.Domain.Entities;

namespace FFS.Application.Data;

public sealed class InMemoryFinancialDataProvider : IFinancialDataProvider
{
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
    }
}
