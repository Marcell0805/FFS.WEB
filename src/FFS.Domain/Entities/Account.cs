namespace FFS.Domain.Entities;

public class Account
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "ZAR";
    public bool IsActive { get; set; } = true;
    public decimal OpeningBalance { get; set; }
}
