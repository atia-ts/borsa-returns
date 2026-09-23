using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Dividends;

public class Dividend
{
    [JsonPropertyName("symbol_code")]
    public string SymbolCode { get; set; } = "";

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }
}
