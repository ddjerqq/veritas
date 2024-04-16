using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Util;
using Client.Common;
using Domain.Common;
using Domain.ValueObjects;

namespace Client.Pages.Stats;

public partial class DailyStackedBarChart
{
    private BarConfig? _config;

    private static Dictionary<DateOnly, int> RandomData(int days)
    {
        return Enumerable
            .Range(0, days)
            .Select(i => new
            {
                Key = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-i),
                Value = Random.Shared.Next(0, 1_000_000),
            })
            .Reverse()
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private Dictionary<Party, Dictionary<DateOnly, int>> DailyVotes { get; set; } = [];

    protected override void OnInitialized()
    {
        DailyVotes = new Dictionary<Party, Dictionary<DateOnly, int>>
        {
            [42] = RandomData(Days),
            [5] = RandomData(Days),
            [9] = RandomData(Days),
            [36] = RandomData(Days),
        };

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

        foreach (var date in DailyVotes.Values.First().Keys)
        {
            _config.Data.Labels.Add(date.ToDateTime().DaysAgoGe());
        }

        foreach (var (party, votes) in DailyVotes)
        {
            var (r, g, b) = party.GetColorRgb();

            var dataset = new BarDataset<int>(votes.Values.ToArray())
            {
                Label = party.Name,
                BackgroundColor = ColorUtil.ColorHexString(r, g, b),
            };

            _config.Data.Datasets.Add(dataset);
        }
    }
}