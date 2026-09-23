using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Purification;

public class NonCompliantEarningReport
{
    [JsonPropertyName("generated_at")]
    public DateTime GeneratedAt { get; set; }

    [JsonPropertyName("total_earning")]
    public double TotalEarning { get; set; }

    [JsonPropertyName("total_haram")]
    public double TotalHaram { get; set; }

    [JsonPropertyName("total_clean")]
    public double TotalClean { get; set; }

    [JsonPropertyName("computed_count")]
    public int ComputedCount { get; set; }

    [JsonPropertyName("blocked_count")]
    public int BlockedCount { get; set; }

    [JsonPropertyName("fallback_count")]
    public int FallbackCount { get; set; }

    [JsonPropertyName("items")]
    public List<NonCompliantEarning> Items { get; set; } = [];
}
