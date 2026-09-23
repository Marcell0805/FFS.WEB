using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Domain;

/// <summary>
/// Shared cash-flow inclusion rules — Home, Budget, and Reports must match.
/// Skip Ignored, Transfer, and own-account / banking-app transfers.
/// </summary>
public static class CashFlowRules
{
    public static bool SkipFromCashFlow(Transaction txn)
    {
        if (txn.Status == TransactionStatus.Ignored) return true;
        if (!MoneyDirectionRules.AffectsCashFlow(txn.Direction)) return true;
        return OwnAccountTransfer.Matches(txn.Merchant, txn.Description);
    }

    public static (DateTime Start, DateTime End) SameDayCountPreviousMonth(DateTime now)
    {
        var lastPrev = new DateTime(now.Year, now.Month, 1).AddDays(-1);
        var day = now.Day > lastPrev.Day ? lastPrev.Day : now.Day;
        return (
            new DateTime(lastPrev.Year, lastPrev.Month, 1),
            new DateTime(lastPrev.Year, lastPrev.Month, day, 23, 59, 59));
    }

    public static string? VsLastCaption(decimal current, decimal? previous)
    {
        var delta = VsLastDelta(current, previous);
        if (delta is null) return null;
        if (delta.Value.Change == 0) return "Same as last period";
        return $"vs last {delta.Value.Text}";
    }

    public static (string Text, decimal Change)? VsLastDelta(decimal current, decimal? previous)
    {
        if (previous is null || Math.Abs(previous.Value) < 0.005m) return null;
        var change = ((current - previous.Value) / Math.Abs(previous.Value)) * 100m;
        if (Math.Abs(change) < 0.5m) return ("0%", 0);
        var sign = change > 0 ? "+" : "";
        return ($"{sign}{change:0}%", change);
    }
}
