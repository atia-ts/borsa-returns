using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Purification;

public class StockInfo
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = "";

    [JsonPropertyName("metricType")]
    public string MetricType { get; set; } = "";

    [JsonPropertyName("earnings")]
    public List<QuarterlyEarning> Earnings { get; set; } = [];

    [JsonPropertyName("coreActivityCompliance")]
    public string CoreActivityCompliance { get; set; } = "";
}
