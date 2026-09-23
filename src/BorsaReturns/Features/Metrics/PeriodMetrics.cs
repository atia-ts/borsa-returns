using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class PeriodMetrics
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("pnl")]
    public double Pnl { get; set; }

    [JsonPropertyName("tradeCount")]
    public int TradeCount { get; set; }
}
