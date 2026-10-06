using ApexCharts;

namespace FFS.Web.Services;

/// <summary>
/// Shared ApexCharts options tuned for larger axis/legend/tooltip text (accessibility).
/// </summary>
public static class ChartThemeOptions
{
    public const string AxisFontSize = "14px";
    public const string LegendFontSize = "14px";
    public const string TooltipFontSize = "14px";
    public const string DonutLabelFontSize = "15px";
    public const string DonutValueFontSize = "16px";

    public static ApexChartOptions<T> Create<T>(bool dark, Action<ApexChartOptions<T>>? configure = null)
        where T : class
    {
        var fore = dark ? "#e2e8f0" : "#334155";
        var options = new ApexChartOptions<T>
        {
            Chart = new Chart
            {
                Background = "transparent",
                ForeColor = fore,
                FontFamily = "DM Sans, system-ui, sans-serif",
                Toolbar = new Toolbar { Show = false }
            },
            Theme = new Theme { Mode = dark ? Mode.Dark : Mode.Light },
            Legend = new Legend
            {
                FontSize = LegendFontSize,
                FontFamily = "DM Sans, system-ui, sans-serif",
                Labels = new LegendLabels { Colors = fore }
            },
            Grid = new Grid
            {
                BorderColor = dark ? "rgba(148,163,184,0.16)" : "rgba(15,23,42,0.06)",
                StrokeDashArray = 4
            },
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Style = new AxisLabelStyle { FontSize = AxisFontSize, Colors = fore }
                }
            },
            Yaxis =
            [
                new YAxis
                {
                    Labels = new YAxisLabels
                    {
                        Style = new AxisLabelStyle { FontSize = AxisFontSize, Colors = fore }
                    }
                }
            ],
            Tooltip = new Tooltip
            {
                Theme = dark ? Mode.Dark : Mode.Light,
                Style = new TooltipStyle { FontSize = TooltipFontSize, FontFamily = "DM Sans, system-ui, sans-serif" }
            },
            PlotOptions = new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    Donut = new PlotOptionsDonut
                    {
                        Labels = new DonutLabels
                        {
                            Name = new DonutLabelName { FontSize = DonutLabelFontSize },
                            Value = new DonutLabelValue { FontSize = DonutValueFontSize },
                            Total = new DonutLabelTotal { FontSize = DonutLabelFontSize, Show = true }
                        }
                    }
                }
            }
        };

        configure?.Invoke(options);
        return options;
    }

    public static readonly string[] CashFlowColors = ["#14b8a6", "#fb7185"];

    public static readonly string[] CategoryColors =
    [
        "#14b8a6", "#38bdf8", "#a78bfa", "#fb7185", "#fbbf24", "#34d399", "#f472b6", "#94a3b8"
    ];

    public static void ApplyCashFlowArea<T>(ApexChartOptions<T> options) where T : class
    {
        options.Colors = CashFlowColors.ToList();
        options.Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 };
        options.DataLabels = new DataLabels { Enabled = false };
        options.Fill = AreaFill(0.45, 0.04);
        options.Markers = new Markers { Size = 0 };
        if (options.Legend is not null)
            options.Legend.Position = LegendPosition.Bottom;
    }

    public static void ApplyColumns<T>(ApexChartOptions<T> options) where T : class
    {
        options.Colors = CashFlowColors.ToList();
        options.DataLabels = new DataLabels { Enabled = false };
        options.PlotOptions ??= new PlotOptions();
        options.PlotOptions.Bar = new PlotOptionsBar
        {
            BorderRadius = 8,
            BorderRadiusApplication = BorderRadiusApplication.End,
            ColumnWidth = "46%"
        };
        if (options.Legend is not null)
            options.Legend.Position = LegendPosition.Bottom;
    }

    public static void ApplyDonut<T>(ApexChartOptions<T> options, string totalLabel) where T : class
    {
        options.Colors = CategoryColors.ToList();
        options.DataLabels = new DataLabels { Enabled = false };
        options.Stroke = new Stroke { Width = 3 };
        if (options.PlotOptions?.Pie?.Donut is { } donut)
        {
            donut.Size = "72%";
            if (donut.Labels is { } labels)
                labels.Show = true;
            if (donut.Labels?.Total is { } total)
            {
                total.Show = true;
                total.Label = totalLabel;
            }
        }

        options.Legend = new Legend { Show = false };
    }

    public static void ApplyCategoryLines<T>(ApexChartOptions<T> options) where T : class
    {
        options.Colors = CategoryColors.ToList();
        options.Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 };
        options.DataLabels = new DataLabels { Enabled = false };
        options.Markers = new Markers { Size = 0 };
        if (options.Legend is not null)
            options.Legend.Position = LegendPosition.Bottom;
    }

    public static void ApplyForecastArea<T>(ApexChartOptions<T> options) where T : class
    {
        options.Colors = ["#2dd4bf"];
        options.Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 };
        options.DataLabels = new DataLabels { Enabled = false };
        options.Fill = AreaFill(0.4, 0.02);
        options.Markers = new Markers { Size = 4 };
    }

    private static Fill AreaFill(double from, double to) => new()
    {
        Type = FillType.Gradient,
        Gradient = new FillGradient
        {
            ShadeIntensity = 0.55,
            OpacityFrom = from,
            OpacityTo = to,
            Stops = [0d, 90d, 100d]
        }
    };
}
