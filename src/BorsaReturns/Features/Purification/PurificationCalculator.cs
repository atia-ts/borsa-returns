using System.Globalization;
using BorsaReturns.Features.Dividends;
using BorsaReturns.Features.Trades;

namespace BorsaReturns.Features.Purification;

/// <summary>
/// Purification of capital gains and dividends:
///   Haram       = Total Earning × Non-compliant Percentage / 100
///   Clean Total = Total Earning − Haram
/// Partial sells: the non-compliant percentage is the higher of (previous quarter, Q4 of the previous year) relative
/// to the sell date's quarter; a tie goes to prior-year Q4. For a Q1 sell both candidates are the same quarter.
/// If any candidate quarter is missing, the configured fallback quarter is used instead. If that is not
/// configured or also missing (or the stock has no data at all), the configured fallback percentage is
/// used; only when that is empty too is the result blocked.
/// Dividends: the percentage is taken only from Q4 of the year before the entitlement date. There is no fallback
/// quarter; when that Q4 (or the stock) is missing, the dividend fallback percentage is used, or it is blocked if empty.
/// </summary>
public static class PurificationCalculator
{
    public static NonCompliantEarningReport CalculateSells(
        IEnumerable<Trade> sells,
        IEnumerable<StockInfo> stocks,
        PurificationSettings settings,
        DateTime generatedAt)
    {
        var bySymbol = BySymbol(stocks);
        var items = sells
            .OrderByDescending(s => s.CloseDate)
            .Select(s => CalculateOne(s.SymbolCode, s.CloseDate, s.NetPnl, SellCandidates(s.CloseDate),
                bySymbol.GetValueOrDefault(s.SymbolCode), settings.Fallback, settings.FallbackPercentage))
            .ToList();
        return BuildReport(items, generatedAt);
    }

    public static NonCompliantEarningReport CalculateDividends(
        IEnumerable<Dividend> dividends,
        IEnumerable<StockInfo> stocks,
        PurificationSettings settings,
        DateTime generatedAt)
    {
        var bySymbol = BySymbol(stocks);
        var items = dividends
            .OrderByDescending(d => d.Date)
            .Select(d => CalculateOne(d.SymbolCode, d.Date, d.Amount, [(d.Date.Year - 1, 4)],
                bySymbol.GetValueOrDefault(d.SymbolCode), fallbackQuarter: null, settings.DividendFallbackPercentage))
            .ToList();
        return BuildReport(items, generatedAt);
    }

    /// <summary>Prior-year Q4 first (it wins ties), then the previous quarter when that is a different one.</summary>
    private static List<(int Year, int Quarter)> SellCandidates(DateTime sellDate)
    {
        var sellQuarter = (sellDate.Month - 1) / 3 + 1;
        var priorYearQ4 = (sellDate.Year - 1, 4);
        return sellQuarter == 1 ? [priorYearQ4] : [priorYearQ4, (sellDate.Year, sellQuarter - 1)];
    }

    private static Dictionary<string, StockInfo> BySymbol(IEnumerable<StockInfo> stocks)
    {
        return stocks
            .GroupBy(s => s.Symbol, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    private static NonCompliantEarningReport BuildReport(List<NonCompliantEarning> items, DateTime generatedAt)
    {
        var computed = items.Where(i => !i.Blocked).ToList();

        return new NonCompliantEarningReport
        {
            GeneratedAt = generatedAt,
            TotalEarning = Math.Round(computed.Sum(i => i.TotalEarning), 2),
            TotalHaram = Math.Round(computed.Sum(i => i.Haram ?? 0), 2),
            TotalClean = Math.Round(computed.Sum(i => i.CleanTotal ?? 0), 2),
            ComputedCount = computed.Count,
            BlockedCount = items.Count - computed.Count,
            FallbackCount = computed.Count(i => i.FallbackUsed),
            Items = items,
        };
    }

    /// <summary>
    /// Uses the highest percentage among <paramref name="candidates"/>; ties go to the earlier candidate.
    /// When a candidate is missing, <paramref name="fallbackQuarter"/> (if any) and then
    /// <paramref name="fallbackPercentage"/> (if any) are used; otherwise the result is blocked.
    /// </summary>
    public static NonCompliantEarning CalculateOne(
        string symbol,
        DateTime date,
        double totalEarning,
        IReadOnlyList<(int Year, int Quarter)> candidates,
        StockInfo? stock,
        (int Year, int Quarter)? fallbackQuarter,
        double? fallbackPercentage)
    {
        var result = new NonCompliantEarning
        {
            Symbol = symbol,
            Date = date,
            TotalEarning = totalEarning,
        };

        if (stock is null)
        {
            return ApplyFallbackPercentage(result, $"No stock info for {symbol}", fallbackPercentage);
        }

        var found = candidates.Select(q => Find(stock, q)).ToList();
        var missing = candidates.Where((_, i) => found[i] is null).Select(Label).ToList();

        if (missing.Count == 0)
        {
            var chosen = found.Aggregate((best, next) => next!.Value > best!.Value ? next : best)!;
            return Apply(result, chosen.Value, Label((chosen.Year, chosen.Quarter)));
        }

        var missingReason = $"Missing {string.Join(", ", missing)} for {symbol}";

        if (fallbackQuarter is not { } fallback)
        {
            return ApplyFallbackPercentage(result, missingReason, fallbackPercentage);
        }

        if (Find(stock, fallback) is not { } fallbackEarning)
        {
            return ApplyFallbackPercentage(result, $"{missingReason}; configured fallback {Label(fallback)} is also missing", fallbackPercentage);
        }

        result.FallbackUsed = true;
        result.Note = $"{missingReason} — used configured fallback {Label(fallback)}";
        return Apply(result, fallbackEarning.Value, Label(fallback));
    }

    /// <summary>Last resort when no quarterly data applies: the configured percentage, or blocked if none.</summary>
    private static NonCompliantEarning ApplyFallbackPercentage(NonCompliantEarning result, string reason, double? percentage)
    {
        if (percentage is not { } fallbackPercentage)
        {
            return Block(result, reason);
        }

        result.FallbackUsed = true;
        result.Note = $"{reason} — used fallback percentage {fallbackPercentage.ToString(CultureInfo.InvariantCulture)}%";
        return Apply(result, fallbackPercentage, sourceQuarter: null);
    }

    private static NonCompliantEarning Apply(NonCompliantEarning result, double nonCompliantPercentage, string? sourceQuarter)
    {
        // Losses carry nothing to purify.
        var haram = result.TotalEarning > 0 ? result.TotalEarning * nonCompliantPercentage / 100 : 0;

        result.NonCompliantPercentageQuarter = sourceQuarter;
        result.NonCompliantPercentage = nonCompliantPercentage;
        result.Haram = Math.Round(haram, 2);
        result.CleanTotal = Math.Round(result.TotalEarning - haram, 2);
        return result;
    }

    private static QuarterlyEarning? Find(StockInfo stock, (int Year, int Quarter) q)
    {
        return stock.Earnings.FirstOrDefault(e => e.Year == q.Year && e.Quarter == q.Quarter);
    }

    private static string Label((int Year, int Quarter) q)
    {
        return $"{q.Year}-Q{q.Quarter}";
    }

    private static NonCompliantEarning Block(NonCompliantEarning result, string reason)
    {
        result.Blocked = true;
        result.BlockedReason = reason;
        return result;
    }
}
