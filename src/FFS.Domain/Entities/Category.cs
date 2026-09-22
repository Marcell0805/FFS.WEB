using FFS.Domain.Enums;

namespace FFS.Domain.Entities;

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public string? IconName { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Planning assignment only — independent of <see cref="CategoryType"/>.
    /// </summary>
    public BudgetBucketKind BudgetBucket { get; set; } = BudgetBucketKind.Unassigned;
}
