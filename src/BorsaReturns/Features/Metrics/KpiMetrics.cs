using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class KpiMetrics
{
    [JsonPropertyName("totalCommission")]
    public double TotalCommission { get; set; }

    [JsonPropertyName("totalNetPnl")]
    public double TotalNetPnl { get; set; }

    [JsonPropertyName("totalGrossPnl")]
    public double TotalGrossPnl { get; set; }

    [JsonPropertyName("winRate")]
    public double WinRate { get; set; }

    [JsonPropertyName("winCount")]
    public int WinCount { get; set; }

    [JsonPropertyName("lossCount")]
    public int LossCount { get; set; }

    [JsonPropertyName("avgPnlPerTrade")]
    public double AvgPnlPerTrade { get; set; }

    [JsonPropertyName("avgDuration")]
    public double AvgDuration { get; set; }

    [JsonPropertyName("bestTrade")]
    public double BestTrade { get; set; }

    [JsonPropertyName("worstTrade")]
    public double WorstTrade { get; set; }

    [JsonPropertyName("profitFactor")]
    public double ProfitFactor { get; set; }

    [JsonPropertyName("profitFactorInfinite")]
    public bool ProfitFactorInfinite { get; set; }

    [JsonPropertyName("avgProfit")]
    public double AvgProfit { get; set; }

    [JsonPropertyName("avgLoss")]
    public double AvgLoss { get; set; }

    [JsonPropertyName("tradeCount")]
    public int TradeCount { get; set; }

    [JsonPropertyName("dividends")]
    public double Dividends { get; set; }

    [JsonPropertyName("dividendCount")]
    public int DividendCount { get; set; }
}
