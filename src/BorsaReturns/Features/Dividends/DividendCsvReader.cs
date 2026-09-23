using System.Globalization;
using BorsaReturns.Features.Trades;

namespace BorsaReturns.Features.Dividends;

public static class DividendCsvReader
{
    private const int ColumnCount = 3;

    /// <summary>Reads dividend rows (<c>symbol,date,amount</c>, date as <c>yyyy-MM-dd</c>), skipping malformed rows.</summary>
    public static List<Dividend> Read(string path)
    {
        var dividends = new List<Dividend>();
        foreach (var (line, fields) in CsvParser.ReadRows(path))
        {
            if (fields.Length < ColumnCount
                || !DateTime.TryParseExact(fields[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var date)
                || !double.TryParse(fields[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var amount))
            {
                Console.Error.WriteLine($"Skipping malformed dividend row: {line}");
                continue;
            }

            dividends.Add(new Dividend
            {
                SymbolCode = fields[0].Trim(),
                Date = date,
                Amount = amount,
            });
        }
        return dividends;
    }
}
