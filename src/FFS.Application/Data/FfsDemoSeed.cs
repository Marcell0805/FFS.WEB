using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Data;

public sealed class FfsDemoSeed
{
    public List<Account> Accounts { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Transaction> Transactions { get; set; } = [];
    public List<Goal> Goals { get; set; } = [];
    public List<GoalTransaction> GoalTransactions { get; set; } = [];
    public List<Budget> Budgets { get; set; } = [];
    public List<BudgetItem> BudgetItems { get; set; } = [];
    public List<NotificationSource> NotificationSources { get; set; } = [];

    public static FfsDemoSeed Create(DateTime? asOf = null)
    {
        var today = (asOf ?? DateTime.Today).Date;
        var seed = new FfsDemoSeed();

        seed.Accounts =
        [
            new Account
            {
                Id = 1, Name = "Everyday Banking", InstitutionName = "FNB",
                AccountType = "Cheque", CurrencyCode = "ZAR", IsActive = true, OpeningBalance = 18500m
            },
            new Account
            {
                Id = 2, Name = "Savings", InstitutionName = "Capitec",
                AccountType = "Savings", CurrencyCode = "ZAR", IsActive = true, OpeningBalance = 42000m
            },
            new Account
            {
                Id = 3, Name = "Credit Card", InstitutionName = "Standard Bank",
                AccountType = "Credit Card", CurrencyCode = "ZAR", IsActive = true, OpeningBalance = -3200m
            }
        ];

        seed.Categories =
        [
            Cat(1, "Salary", CategoryType.Income, BudgetBucketKind.Unassigned, "payments"),
            Cat(2, "Groceries", CategoryType.Expense, BudgetBucketKind.Needs, "cart"),
            Cat(3, "Fuel", CategoryType.Expense, BudgetBucketKind.Needs, "fuel"),
            Cat(4, "Housing", CategoryType.Expense, BudgetBucketKind.Needs, "home"),
            Cat(5, "Utilities", CategoryType.Expense, BudgetBucketKind.Needs, "bolt"),
            Cat(6, "Subscriptions", CategoryType.Expense, BudgetBucketKind.Wants, "repeat"),
            Cat(7, "Entertainment", CategoryType.Expense, BudgetBucketKind.Wants, "film"),
            Cat(8, "Eating Out", CategoryType.Expense, BudgetBucketKind.Wants, "utensils"),
            Cat(9, "Transport", CategoryType.Expense, BudgetBucketKind.Needs, "bus"),
            Cat(10, "Shopping", CategoryType.Expense, BudgetBucketKind.Wants, "bag"),
            Cat(11, "Insurance", CategoryType.Expense, BudgetBucketKind.Needs, "shield"),
            Cat(12, "Savings", CategoryType.Expense, BudgetBucketKind.Savings, "piggy"),
            Cat(13, "Transfers", CategoryType.Transfer, BudgetBucketKind.Unassigned, "arrows")
        ];

        seed.NotificationSources =
        [
            new NotificationSource { Id = 1, BankName = "Demo", DisplayName = "Demo Bank", IsEnabled = true }
        ];

        var rng = new Random(42);
        long txnId = 1;
        var transactions = new List<Transaction>();

        for (var monthsAgo = 11; monthsAgo >= 0; monthsAgo--)
        {
            var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-monthsAgo);
            var daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);

            // Salary: 25th normally; for the current month use a date on/before today so Money In isn't empty
            var salaryDay = Math.Min(25, daysInMonth);
            if (monthsAgo == 0)
                salaryDay = Math.Min(salaryDay, Math.Max(1, today.Day));

            var salaryDate = monthStart.AddDays(salaryDay - 1);
            if (salaryDate <= today)
            {
                transactions.Add(Txn(ref txnId, 1, salaryDate, "Employer Pty Ltd",
                    "Monthly salary", 45000m, MoneyDirection.MoneyIn, 1, TransactionStatus.Confirmed));
            }

            // Housing / rent
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(1), "Landlord",
                "Rent", 12500m, MoneyDirection.MoneyOut, 4, TransactionStatus.Confirmed));

            // Utilities
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(6), "City Power",
                "Electricity & water", 1850m + rng.Next(0, 400), MoneyDirection.MoneyOut, 5, TransactionStatus.Confirmed));

            // Insurance
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(3), "Outsurance",
                "Car & home cover", 1420m, MoneyDirection.MoneyOut, 11, TransactionStatus.Confirmed));

            // Subscriptions
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(4), "Netflix",
                "Streaming", 199m, MoneyDirection.MoneyOut, 6, TransactionStatus.Confirmed));
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(4), "Spotify",
                "Music", 79.99m, MoneyDirection.MoneyOut, 6, TransactionStatus.Confirmed));
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(5), "DSTV",
                "Compact", 699m, MoneyDirection.MoneyOut, 6, TransactionStatus.Confirmed));

            // Groceries (weekly-ish)
            for (var w = 0; w < 4; w++)
            {
                var day = Math.Min(7 + w * 7, daysInMonth);
                transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(day - 1),
                    Pick(rng, "Checkers", "Woolworths", "Pick n Pay", "Spar"),
                    "Groceries", 950m + rng.Next(0, 650), MoneyDirection.MoneyOut, 2, TransactionStatus.Confirmed));
            }

            // Fuel
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(8), "Engen",
                "Petrol", 1100m + rng.Next(0, 300), MoneyDirection.MoneyOut, 3, TransactionStatus.Confirmed));
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(22), "Shell",
                "Petrol", 980m + rng.Next(0, 250), MoneyDirection.MoneyOut, 3, TransactionStatus.Confirmed));

            // Eating out / entertainment
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(12), Pick(rng, "Nando's", "Ocean Basket", "Mugg & Bean"),
                "Dinner", 380m + rng.Next(0, 220), MoneyDirection.MoneyOut, 8, TransactionStatus.Confirmed));
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(18), Pick(rng, "Ster-Kinekor", "Nu Metro"),
                "Movies", 220m + rng.Next(0, 80), MoneyDirection.MoneyOut, 7, TransactionStatus.Confirmed));

            // Transport / Uber
            transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(10), "Uber",
                "Rides", 320m + rng.Next(0, 180), MoneyDirection.MoneyOut, 9, TransactionStatus.Confirmed));

            // Shopping occasional
            if (monthsAgo % 2 == 0)
            {
                transactions.Add(Txn(ref txnId, 3, monthStart.AddDays(15), Pick(rng, "Mr Price", "Takealot", "Cotton On"),
                    "Shopping", 650m + rng.Next(0, 900), MoneyDirection.MoneyOut, 10, TransactionStatus.Confirmed));
            }

            // Larger expense occasionally
            if (monthsAgo == 2 || monthsAgo == 7)
            {
                transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(14), "Makro",
                    "Appliance / home", 4500m + rng.Next(0, 2000), MoneyDirection.MoneyOut, 10, TransactionStatus.Edited));
            }

            // Transfer to savings
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(26), "Internal Transfer",
                "To savings", 5000m, MoneyDirection.Transfer, 13, TransactionStatus.Confirmed));
            transactions.Add(Txn(ref txnId, 2, monthStart.AddDays(26), "Internal Transfer",
                "From everyday", 5000m, MoneyDirection.Transfer, 13, TransactionStatus.Confirmed));

            // Savings contribution (MoneyOut to savings category on everyday — simplified)
            transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(27), "Emergency Fund",
                "Goal contribution", 2500m, MoneyDirection.MoneyOut, 12, TransactionStatus.Confirmed));

            // Ignored noise
            if (monthsAgo <= 2)
            {
                transactions.Add(Txn(ref txnId, 1, monthStart.AddDays(9), "Spam Merchant",
                    "Duplicate / ignore", 50m, MoneyDirection.MoneyOut, 10, TransactionStatus.Ignored));
            }
        }

        seed.Transactions = transactions;

        // Current month budget
        var budget = new Budget
        {
            Id = 1,
            Year = today.Year,
            Month = today.Month,
            IncomeTarget = 45000m
        };
        seed.Budgets = [budget];

        seed.BudgetItems =
        [
            Item(1, 1, 4, BudgetBucketKind.Needs, 12500m),   // Housing
            Item(2, 1, 2, BudgetBucketKind.Needs, 4500m),    // Groceries
            Item(3, 1, 3, BudgetBucketKind.Needs, 2200m),    // Fuel
            Item(4, 1, 5, BudgetBucketKind.Needs, 2000m),    // Utilities
            Item(5, 1, 11, BudgetBucketKind.Needs, 1420m),   // Insurance
            Item(6, 1, 9, BudgetBucketKind.Needs, 600m),     // Transport
            Item(7, 1, 6, BudgetBucketKind.Wants, 1000m),    // Subscriptions
            Item(8, 1, 8, BudgetBucketKind.Wants, 1200m),    // Eating out
            Item(9, 1, 7, BudgetBucketKind.Wants, 500m),     // Entertainment
            Item(10, 1, 10, BudgetBucketKind.Wants, 1500m),  // Shopping
            Item(11, 1, 12, BudgetBucketKind.Savings, 7500m) // Savings
        ];

        seed.Goals =
        [
            new Goal
            {
                Id = 1, Name = "Emergency Fund", Description = "3–6 months of expenses",
                TargetAmount = 50000m, CurrentAmount = 41200m,
                TargetDate = new DateTime(2026, 12, 31), IconName = "shield", IsActive = true
            },
            new Goal
            {
                Id = 2, Name = "Cape Town Holiday", Description = "Family trip",
                TargetAmount = 18000m, CurrentAmount = 7400m,
                TargetDate = new DateTime(2026, 12, 15), IconName = "plane", IsActive = true
            },
            new Goal
            {
                Id = 3, Name = "New Laptop", Description = "Work machine upgrade",
                TargetAmount = 22000m, CurrentAmount = 9500m,
                TargetDate = new DateTime(2027, 3, 31), IconName = "laptop", IsActive = true
            }
        ];

        long gtId = 1;
        seed.GoalTransactions =
        [
            Gt(ref gtId, 1, 2500m, today.AddMonths(-5), "Monthly transfer"),
            Gt(ref gtId, 1, 2500m, today.AddMonths(-4), "Monthly transfer"),
            Gt(ref gtId, 1, 2500m, today.AddMonths(-3), "Monthly transfer"),
            Gt(ref gtId, 1, 2500m, today.AddMonths(-2), "Monthly transfer"),
            Gt(ref gtId, 1, 2500m, today.AddMonths(-1), "Monthly transfer"),
            Gt(ref gtId, 1, 2500m, today.AddDays(-5), "Monthly transfer"),
            Gt(ref gtId, 2, 1200m, today.AddMonths(-3), "Holiday pot"),
            Gt(ref gtId, 2, 1500m, today.AddMonths(-2), "Holiday pot"),
            Gt(ref gtId, 2, 1500m, today.AddMonths(-1), "Holiday pot"),
            Gt(ref gtId, 2, 800m, today.AddDays(-10), "Bonus allocation"),
            Gt(ref gtId, 3, 3000m, today.AddMonths(-4), "Starting deposit"),
            Gt(ref gtId, 3, 2000m, today.AddMonths(-2), "Top-up"),
            Gt(ref gtId, 3, 2500m, today.AddMonths(-1), "Top-up"),
            Gt(ref gtId, 3, 2000m, today.AddDays(-8), "Top-up")
        ];

        return seed;
    }

    private static Category Cat(long id, string name, CategoryType type, BudgetBucketKind bucket, string icon) =>
        new() { Id = id, Name = name, CategoryType = type, BudgetBucket = bucket, IconName = icon, IsActive = true };

    private static BudgetItem Item(long id, long budgetId, long categoryId, BudgetBucketKind bucket, decimal planned) =>
        new() { Id = id, BudgetId = budgetId, CategoryId = categoryId, Bucket = bucket, PlannedAmount = planned };

    private static GoalTransaction Gt(ref long id, long goalId, decimal amount, DateTime date, string notes) =>
        new() { Id = id++, GoalId = goalId, Amount = amount, ContributionDate = date.Date, Notes = notes };

    private static Transaction Txn(
        ref long id, long accountId, DateTime date, string merchant, string description,
        decimal amount, MoneyDirection direction, long categoryId, TransactionStatus status) =>
        new()
        {
            Id = id++,
            AccountId = accountId,
            TransactionDate = date.Date,
            Merchant = merchant,
            Description = description,
            Amount = Math.Round(amount, 2),
            Direction = direction,
            CategoryId = categoryId,
            Status = status
        };

    private static string Pick(Random rng, params string[] values) =>
        values[rng.Next(values.Length)];
}
