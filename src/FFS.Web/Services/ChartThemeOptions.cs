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
            Grid = new Grid { BorderColor = dark ? "#334155" : "#e2e8f0" },
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
}
