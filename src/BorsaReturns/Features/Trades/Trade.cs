using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Trades;

public class Trade
{
    [JsonPropertyName("symbol_code")]
    public string SymbolCode { get; set; } = "";

    [JsonPropertyName("open_date")]
    public DateTime OpenDate { get; set; }

    [JsonPropertyName("close_date")]
    public DateTime CloseDate { get; set; }

    [JsonPropertyName("volume")]
    public int Volume { get; set; }

    [JsonPropertyName("avg_entry_price")]
    public double AvgEntryPrice { get; set; }

    [JsonPropertyName("close_price")]
    public double ClosePrice { get; set; }

    [JsonPropertyName("net_pnl")]
    public double NetPnl { get; set; }

    [JsonPropertyName("net_pnl_percentage")]
    public double NetPnlPercentage { get; set; }

    [JsonPropertyName("duration_days")]
    public int DurationDays { get; set; }

    [JsonPropertyName("reuters_code")]
    public string ReutersCode { get; set; } = "";

    [JsonPropertyName("commission")]
    public double Commission { get; set; }
}
