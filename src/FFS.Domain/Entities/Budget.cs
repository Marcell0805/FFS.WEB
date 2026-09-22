namespace FFS.Domain.Entities;

public class Budget
{
    public long Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal IncomeTarget { get; set; }
}
