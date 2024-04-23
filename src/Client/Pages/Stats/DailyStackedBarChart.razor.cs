using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Util;
using Client.Common;
using Domain.Common;
using Domain.ValueObjects;

namespace Client.Pages.Stats;

public partial class DailyStackedBarChart
{
    private BarConfig? _config;

    protected override void OnInitialized()
    {
        _config = new BarConfig
        {
            Options = new BarOptions
            {
                MaintainAspectRatio = false,
                Legend = ChartConfig.NoLegend,
                Tooltips = ChartConfig.Tooltips,
                Animation = ChartConfig.EaseInOneSecAnimation,
                Scales = ChartConfig.BarScales,
            },
        };

        foreach (var date in DailyVotes.Values.FirstOrDefault()?.Keys ?? Enumerable.Empty<DateOnly>())
        {
            _config.Data.Labels.Add(date.ToDateTime().DaysAgoGe());
        }

        foreach (var (party, votes) in DailyVotes)
        {
            var (r, g, b) = ((Party)party).GetColorRgb();

            var dataset = new BarDataset<int>(votes.Values.ToArray())
            {
                Label = ((Party)party).Name,
                BackgroundColor = ColorUtil.ColorHexString(r, g, b),
            };

            _config.Data.Datasets.Add(dataset);
        }
    }
}