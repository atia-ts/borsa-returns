using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class TopStockMetrics
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("pnl")]
    public double Pnl { get; set; }

    [JsonPropertyName("tradeCount")]
    public int TradeCount { get; set; }
}
