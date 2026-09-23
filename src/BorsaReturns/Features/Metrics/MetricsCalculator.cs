using BorsaReturns.Features.Dividends;
using BorsaReturns.Features.Trades;

namespace BorsaReturns.Features.Metrics;

public static class MetricsCalculator
{
    public static DashboardMetrics Calculate(List<Trade> trades, List<Dividend> dividends, double commission, DateTime referenceDate)
    {
        var metrics = new DashboardMetrics();

        var wins = trades.Where(t => t.NetPnl >= 0).ToList();
        var losses = trades.Where(t => t.NetPnl < 0).ToList();

        var rawPnl = trades.Sum(t => t.NetPnl);
        var totalDividends = dividends.Sum(d => d.Amount);

        var kpis = new KpiMetrics {

            TradeCount = trades.Count,
            TotalCommission = commission,
            TotalGrossPnl = rawPnl,
            Dividends = totalDividends,
            DividendCount = dividends.Count,
            TotalNetPnl = rawPnl + totalDividends - commission,
            WinCount = wins.Count,
            LossCount = losses.Count,
            WinRate = trades.Count > 0 ? (double)wins.Count / trades.Count * 100 : 0,
            AvgPnlPerTrade = trades.Count > 0 ? rawPnl / trades.Count : 0,
            AvgDuration = trades.Count > 0 ? trades.Average(t => (double)t.DurationDays) : 0,
            BestTrade = trades.Count > 0 ? trades.Max(t => t.NetPnl) : 0,
            WorstTrade = trades.Count > 0 ? trades.Min(t => t.NetPnl) : 0,
            AvgProfit = wins.Count > 0 ? wins.Average(t => t.NetPnl) : 0,
            AvgLoss = losses.Count > 0 ? losses.Average(t => t.NetPnl) : 0
        };

        if (losses.Count > 0)
        {
            var totalLossPnl = Math.Abs(losses.Sum(t => t.NetPnl));
            kpis.ProfitFactor = totalLossPnl > 0 ? wins.Sum(t => t.NetPnl) / totalLossPnl : 0;
        }
        else if (wins.Count > 0)
        {
            kpis.ProfitFactorInfinite = true;
        }

        metrics.Kpis = kpis;

        // Top Stock
        metrics.TopStock = trades
            .GroupBy(t => t.ReutersCode)
            .Select(g => new TopStockMetrics
            {
                Code = g.Key,
                Pnl = g.Sum(t => t.NetPnl),
                TradeCount = g.Count()
            })
            .OrderByDescending(x => x.Pnl)
            .FirstOrDefault();

        // Periods
        var periodDefs = new (string Label, int Months, bool Exact, bool AllTime)[]
        {
            ("This Month", 1, true, false),
            ("Past 3 Months", 3, false, false),
            ("Past 6 Months", 6, false, false),
            ("Past 1 Year", 12, false, false),
            ("Past 2 Years", 24, false, false),
            ("All Time", 0, false, true),
        };

        foreach (var (label, months, exact, allTime) in periodDefs)
        {
            List<Trade> periodTrades;
            if (allTime)
            {
                periodTrades = trades;
            }
            else
            {
                var cutoff = exact
                    ? new DateTime(referenceDate.Year, referenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                    : referenceDate.AddMonths(-months);
                periodTrades = trades.Where(t => t.CloseDate >= cutoff && t.CloseDate <= referenceDate).ToList();
            }

            metrics.Periods.Add(new PeriodMetrics
            {
                Label = label,
                Pnl = periodTrades.Sum(t => t.NetPnl),
                TradeCount = periodTrades.Count,
            });
        }

        // Total Returns
        metrics.TotalReturns = kpis.TotalNetPnl;

        // Charts
        var sorted = trades.OrderBy(t => t.CloseDate).ToList();

        // Cumulative P&L
        double cum = 0;
        var cumLabels = new List<string>();
        var cumValues = new List<double>();
        foreach (var t in sorted)
        {
            cum += t.NetPnl;
            cumLabels.Add(t.CloseDate.ToString("yyyy-MM-dd"));
            cumValues.Add(Math.Round(cum, 2));
        }
        metrics.Charts.CumulativePnl = new ChartDataSet { Labels = cumLabels, Values = cumValues };

        // P&L by Stock
        var stockPnl = trades
            .GroupBy(t => t.ReutersCode)
            .Select(g => (Code: g.Key, Pnl: Math.Round(g.Sum(t => t.NetPnl), 2)))
            .OrderByDescending(x => x.Pnl)
            .ToList();
        metrics.Charts.PnlByStock = new ChartDataSet
        {
            Labels = stockPnl.Select(x => x.Code).ToList(),
            Values = stockPnl.Select(x => x.Pnl).ToList(),
        };

        // Monthly P&L
        var monthlyPnl = trades
            .GroupBy(t => t.CloseDate.ToString("yyyy-MM"))
            .Select(g => (Month: g.Key, Pnl: Math.Round(g.Sum(t => t.NetPnl), 2)))
            .OrderBy(x => x.Month)
            .ToList();
        metrics.Charts.MonthlyPnl = new ChartDataSet
        {
            Labels = monthlyPnl.Select(x => x.Month).ToList(),
            Values = monthlyPnl.Select(x => x.Pnl).ToList(),
        };

        // Trades Calendar
        metrics.TradesCalendar = BuildMonthlyCalendar(trades);

        // Win Rate by Stock
        var winRate = trades
            .GroupBy(t => t.ReutersCode)
            .Select(g => (Code: g.Key, Rate: Math.Round((double)g.Count(t => t.NetPnl >= 0) / g.Count() * 100, 1)))
            .OrderByDescending(x => x.Rate)
            .ToList();
        metrics.Charts.WinRateByStock = new ChartDataSet
        {
            Labels = winRate.Select(x => x.Code).ToList(),
            Values = winRate.Select(x => x.Rate).ToList(),
        };

        return metrics;
    }

    public static List<PeriodMetrics> CalculatePartialSellsPeriods(List<Trade> partialSells, DateTime referenceDate)
    {
        var periods = new List<PeriodMetrics>();

        var periodDefs = new (string Label, int Months, bool Exact, bool AllTime)[]
        {
            ("This Month", 1, true, false),
            ("Past 3 Months", 3, false, false),
            ("Past 6 Months", 6, false, false),
            ("Past 1 Year", 12, false, false),
            ("Past 2 Years", 24, false, false),
            ("All Time", 0, false, true),
        };

        foreach (var (label, months, exact, allTime) in periodDefs)
        {
            List<Trade> periodTrades;
            if (allTime)
            {
                periodTrades = partialSells;
            }
            else
            {
                var cutoff = exact
                    ? new DateTime(referenceDate.Year, referenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                    : referenceDate.AddMonths(-months);
                periodTrades = partialSells.Where(t => t.CloseDate >= cutoff && t.CloseDate <= referenceDate).ToList();
            }

            periods.Add(new PeriodMetrics
            {
                Label = label,
                Pnl = periodTrades.Sum(t => t.NetPnl),
                TradeCount = periodTrades.Count,
            });
        }

        return periods;
    }

    public static List<MonthlyCalendarEntry> BuildMonthlyCalendar(List<Trade> trades)
    {
        return trades
            .GroupBy(t => t.CloseDate.ToString("yyyy-MM"))
            .Select(g => new MonthlyCalendarEntry
            {
                Month = g.Key,
                Pnl = Math.Round(g.Sum(t => t.NetPnl), 2),
                TradeCount = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();
    }
}