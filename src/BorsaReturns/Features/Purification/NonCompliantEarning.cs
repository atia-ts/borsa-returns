using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Purification;

public class NonCompliantEarning
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = "";

    /// <summary>Sell date for partial sells; entitlement date for dividends.</summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("total_earning")]
    public double TotalEarning { get; set; }

    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("blocked_reason")]
    public string? BlockedReason { get; set; }

    /// <summary>True when a configured fallback (quarter or percentage) was used because data was missing.</summary>
    [JsonPropertyName("fallback_used")]
    public bool FallbackUsed { get; set; }

    /// <summary>Explains how a computed result was reached when it deviates from the standard rule.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>Quarter the percentage was taken from, e.g. "2024-Q4"; null when the fallback percentage was used.</summary>
    [JsonPropertyName("non_compliant_percentage_quarter")]
    public string? NonCompliantPercentageQuarter { get; set; }

    [JsonPropertyName("non_compliant_percentage")]
    public double? NonCompliantPercentage { get; set; }

    [JsonPropertyName("haram")]
    public double? Haram { get; set; }

    [JsonPropertyName("clean_total")]
    public double? CleanTotal { get; set; }
}
