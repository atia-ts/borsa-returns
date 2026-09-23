using System.Text.Json.Serialization;

namespace BorsaReturns.Features.Metrics;

public class DashboardMetrics
{
    [JsonPropertyName("kpis")]
    public KpiMetrics Kpis { get; set; } = new();

    [JsonPropertyName("topStock")]
    public TopStockMetrics? TopStock { get; set; }

    [JsonPropertyName("periods")]
    public List<PeriodMetrics> Periods { get; set; } = [];

    [JsonPropertyName("totalReturns")]
    public double TotalReturns { get; set; }

    [JsonPropertyName("charts")]
    public ChartMetrics Charts { get; set; } = new();

    [JsonPropertyName("partialSellsPeriods")]
    public List<PeriodMetrics> PartialSellsPeriods { get; set; } = [];

    [JsonPropertyName("tradesCalendar")]
    public List<MonthlyCalendarEntry> TradesCalendar { get; set; } = [];

    [JsonPropertyName("partialSellsCalendar")]
    public List<MonthlyCalendarEntry> PartialSellsCalendar { get; set; } = [];
}
