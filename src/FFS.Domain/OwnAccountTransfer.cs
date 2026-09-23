namespace FFS.Domain;

/// <summary>
/// Own-account movements (savings ↔ everyday) — not income or spend.
/// Mirrors FFS Mobile OwnAccountTransfer.
/// </summary>
public static class OwnAccountTransfer
{
    private static readonly string[] Needles =
    [
        "banking app transfer",
        "recurring transfer",
        "internal transfer",
        "account transfer",
        "transfer to savings",
        "transfer from savings",
        "transfer received from savings",
        "between accounts"
    ];

    public static bool Matches(string? merchant, string? description)
    {
        var hay = string.Join(' ', new[] { merchant, description }.Where(s => !string.IsNullOrWhiteSpace(s)))
            .ToLowerInvariant();
        if (Needles.Any(hay.Contains)) return true;
        return hay.Contains("transfer") &&
               (hay.Contains("savings") || hay.Contains("savingsmoney"));
    }
}
