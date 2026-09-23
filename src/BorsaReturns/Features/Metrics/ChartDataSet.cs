using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class ChartDataSet
{
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = [];

    [JsonPropertyName("values")]
    public List<double> Values { get; set; } = [];
}
