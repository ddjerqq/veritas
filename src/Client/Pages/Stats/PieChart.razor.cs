using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using Client.Common;
using Domain.ValueObjects;

namespace Client.Pages.Stats;

public partial class PieChart
{
    private PieConfig? _config;

    protected override void OnInitialized()
    {
        Votes = new Dictionary<Party, int>
        {
            [42] = Random.Shared.Next(0, 1_000_000),
            [5] = Random.Shared.Next(0, 1_000_000),
            [9] = Random.Shared.Next(0, 1_000_000),
            [36] = Random.Shared.Next(0, 1_000_000),
        };

        // for debug only
        Votes = Votes
            .OrderBy(kv => kv.Value)
            .Reverse()
            .ToDictionary();

        _config = new PieConfig
        {
            Options = new PieOptions
            {
                MaintainAspectRatio = false,
                CutoutPercentage = 60,
                Legend = ChartConfig.NoLegend,
                Tooltips = ChartConfig.Tooltips,
                Animation = ChartConfig.EaseInOneSecAnimation,
            },
        };

        foreach (var party in Votes.Keys)
        {
            _config.Data.Labels.Add($" {party.Name}");
        }

        var dataset = new PieDataset<int>(Votes.Values)
        {
            BackgroundColor = Votes.Keys.Select(party =>
            {
                var (r, g, b) = party.GetColorRgb();
                return ColorUtil.ColorHexString(r, g, b);
            }).ToArray(),
        };

        _config.Data.Datasets.Add(dataset);
    }
}