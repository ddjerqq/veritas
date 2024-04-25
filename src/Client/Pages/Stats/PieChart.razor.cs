using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using Client.Common;
using Domain.ValueObjects;

namespace Client.Pages.Stats;

public partial class PieChart
{
    private PieConfig? _config;

    private Dictionary<int, int>? Votes { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Votes = (await Api.GetPartyVoteCounts(CancellationToken))!;
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

        foreach (Party party in Votes.Keys)
        {
            _config.Data.Labels.Add($" {party.Name}");
        }

        var dataset = new PieDataset<int>(Votes.Values)
        {
            BackgroundColor = Votes.Keys.Select(party =>
            {
                var (r, g, b) = ((Party)party).GetColorRgb();
                return ColorUtil.ColorHexString(r, g, b);
            }).ToArray(),
        };

        _config.Data.Datasets.Add(dataset);
    }
}