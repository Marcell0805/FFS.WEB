namespace FFS.Web.Services;

public sealed class PaletteColors
{
    public string MoneyIn { get; set; } = "#14b8a6";
    public string MoneyOut { get; set; } = "#fb7185";
    public string Needs { get; set; } = "#38bdf8";
    public string Wants { get; set; } = "#a78bfa";
    public string Savings { get; set; } = "#34d399";
    public string Over { get; set; } = "#f59e0b";
    public string Warning { get; set; } = "#ef4444";
    public string Chart1 { get; set; } = "#14b8a6";
    public string Chart2 { get; set; } = "#fb7185";
    public string Chart3 { get; set; } = "#38bdf8";
    public string Chart4 { get; set; } = "#a78bfa";
    public string Chart5 { get; set; } = "#f59e0b";
    public string Chart6 { get; set; } = "#34d399";
    public string ShadowLight { get; set; } = "#0f172a";
    public string ShadowDark { get; set; } = "#000000";

    public string ChartAt(int index)
    {
        var colors = new[] { Chart1, Chart2, Chart3, Chart4, Chart5, Chart6 };
        return colors[Math.Abs(index) % colors.Length];
    }

    public List<string> ChartSeries() => [Chart1, Chart2, Chart3, Chart4, Chart5, Chart6];

    public PaletteColors Copy() => new()
    {
        MoneyIn = MoneyIn,
        MoneyOut = MoneyOut,
        Needs = Needs,
        Wants = Wants,
        Savings = Savings,
        Over = Over,
        Warning = Warning,
        Chart1 = Chart1,
        Chart2 = Chart2,
        Chart3 = Chart3,
        Chart4 = Chart4,
        Chart5 = Chart5,
        Chart6 = Chart6,
        ShadowLight = ShadowLight,
        ShadowDark = ShadowDark
    };
}
