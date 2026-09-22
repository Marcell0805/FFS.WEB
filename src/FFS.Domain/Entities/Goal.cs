namespace FFS.Domain.Entities;

public class Goal
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public string? IconName { get; set; }
    public bool IsActive { get; set; } = true;

    public decimal Progress =>
        TargetAmount <= 0 ? 0 : Math.Clamp(CurrentAmount / TargetAmount, 0, 1);

    public decimal Remaining => Math.Max(TargetAmount - CurrentAmount, 0);
}
