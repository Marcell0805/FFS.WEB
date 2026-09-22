using FFS.Domain.Enums;

namespace FFS.Domain.Entities;

public class BudgetItem
{
    public long Id { get; set; }
    public long BudgetId { get; set; }
    public long? CategoryId { get; set; }
    public BudgetBucketKind Bucket { get; set; }
    public decimal PlannedAmount { get; set; }
}
