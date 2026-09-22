using FFS.Domain.Enums;

namespace FFS.Domain.Entities;

public class Transaction
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Merchant { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public MoneyDirection Direction { get; set; }
    public long? CategoryId { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Confirmed;
    public string? Reference { get; set; }
}
