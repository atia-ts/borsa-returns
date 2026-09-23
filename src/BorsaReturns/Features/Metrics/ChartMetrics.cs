using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class ChartMetrics
{
    [JsonPropertyName("cumulativePnl")]
    public ChartDataSet CumulativePnl { get; set; } = new();

    [JsonPropertyName("pnlByStock")]
    public ChartDataSet PnlByStock { get; set; } = new();

    [JsonPropertyName("monthlyPnl")]
    public ChartDataSet MonthlyPnl { get; set; } = new();

    [JsonPropertyName("winRateByStock")]
    public ChartDataSet WinRateByStock { get; set; } = new();
}
