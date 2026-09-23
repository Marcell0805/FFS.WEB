namespace FFS.Web;

/// <summary>Bootstrap icon names for categories — visual only, no invented metrics.</summary>
public static class CategoryIcons
{
    public static string For(string? categoryName, string? iconName = null)
    {
        if (!string.IsNullOrWhiteSpace(iconName) && iconName.StartsWith("bi-", StringComparison.OrdinalIgnoreCase))
            return iconName;

        var key = (categoryName ?? "").Trim().ToLowerInvariant();
        return key switch
        {
            var n when n.Contains("salary") || n.Contains("income") || n.Contains("wage") => "bi-wallet2",
            var n when n.Contains("hous") || n.Contains("rent") || n.Contains("bond") => "bi-house",
            var n when n.Contains("grocer") || n.Contains("food") => "bi-basket",
            var n when n.Contains("fuel") || n.Contains("petrol") || n.Contains("transport") => "bi-fuel-pump",
            var n when n.Contains("util") || n.Contains("electric") || n.Contains("water") => "bi-lightning",
            var n when n.Contains("insur") => "bi-shield-check",
            var n when n.Contains("shop") || n.Contains("retail") => "bi-bag",
            var n when n.Contains("eat") || n.Contains("rest") || n.Contains("coffee") => "bi-cup-hot",
            var n when n.Contains("entertain") || n.Contains("movie") => "bi-controller",
            var n when n.Contains("subscr") || n.Contains("netflix") => "bi-play-btn",
            var n when n.Contains("sav") || n.Contains("goal") || n.Contains("emergency") => "bi-piggy-bank",
            var n when n.Contains("transfer") => "bi-arrow-left-right",
            var n when n.Contains("medical") || n.Contains("health") => "bi-heart-pulse",
            var n when n.Contains("school") || n.Contains("educat") => "bi-mortarboard",
            _ => "bi-circle"
        };
    }
}
