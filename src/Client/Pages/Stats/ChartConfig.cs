using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.BarChart.Axes;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.Util;

namespace Client.Pages.Stats;

public static class ChartConfig
{
    public static readonly Tooltips Tooltips = new()
    {
        Mode = InteractionMode.Index,
        Intersect = true,
        BackgroundColor = "#000000",
        DisplayColors = true,
        BodyFontFamily = "archyedt",
        BodyFontSize = 18,
        BodySpacing = 10,
        XPadding = 10,
        YPadding = 10,
    };

    public static readonly ArcAnimation EaseInOneSecAnimation = new()
    {
        Easing = Easing.EaseInOutQuint,
        Duration = 1000,
    };

    public static readonly BarScales BarScales = new()
    {
        XAxes =
        [
            new BarCategoryAxis
            {
                Stacked = true,
                GridLines = new GridLines
                {
                    Color = ColorUtil.ColorString(50, 50, 50),
                    ZeroLineColor = ColorUtil.ColorString(50, 50, 50),
                },
            },
        ],
        YAxes =
        [
            new BarLinearCartesianAxis
            {
                Stacked = true,
                GridLines = new GridLines
                {
                    Color = ColorUtil.ColorString(50, 50, 50),
                    ZeroLineColor = ColorUtil.ColorString(50, 50, 50),
                },
            },
        ],
    };

    public static readonly Legend NoLegend = new()
    {
        Display = false,
    };
}