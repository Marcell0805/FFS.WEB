namespace FFS.Domain.Enums;

/// <summary>
/// Planning buckets — distinct from <see cref="CategoryType"/>.
/// Example: Category=Groceries, CategoryType=Expense, Bucket=Needs.
/// </summary>
public enum BudgetBucketKind
{
    Needs,
    Wants,
    Savings,
    Unassigned
}
