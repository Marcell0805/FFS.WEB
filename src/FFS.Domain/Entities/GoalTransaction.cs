namespace FFS.Domain.Entities;

public class GoalTransaction
{
    public long Id { get; set; }
    public long GoalId { get; set; }
    public long? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public string? Notes { get; set; }
}
