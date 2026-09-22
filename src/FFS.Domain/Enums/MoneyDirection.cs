namespace FFS.Domain.Enums;

public enum MoneyDirection
{
    MoneyIn,
    MoneyOut,
    Transfer
}

public static class MoneyDirectionRules
{
    public static bool IsIncome(MoneyDirection direction) =>
        direction == MoneyDirection.MoneyIn;

    public static bool IsTransfer(MoneyDirection direction) =>
        direction == MoneyDirection.Transfer;

    /// <summary>Transfers do not affect cash-flow totals (FFS Mobile rule).</summary>
    public static bool AffectsCashFlow(MoneyDirection direction) =>
        !IsTransfer(direction);
}
