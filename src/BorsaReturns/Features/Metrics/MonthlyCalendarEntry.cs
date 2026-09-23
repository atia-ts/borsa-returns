using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class MonthlyCalendarEntry
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = "";

    [JsonPropertyName("pnl")]
    public double Pnl { get; set; }

    [JsonPropertyName("tradeCount")]
    public int TradeCount { get; set; }
}
