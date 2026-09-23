using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Purification;

public class QuarterlyEarning
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("quarter")]
    public int Quarter { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }
}
