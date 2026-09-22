namespace FFS.Domain.Entities;

public class NotificationSource
{
    public long Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
