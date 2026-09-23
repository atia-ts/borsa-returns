using System.Globalization;

namespace BorsaReturns.Features.Trades;

public static class TradeCsvReader
{
    private const int JournalColumnCount = 9;
    private const int PartialSellColumnCount = 6;

    public static List<Trade> ReadJournal(string path)
    {
        return ReadTrades(path, JournalColumnCount, "journal", fields => new Trade {
            SymbolCode = fields[0],
            ReutersCode = fields[0],
            OpenDate = ParseDate(fields[1]),
            AvgEntryPrice = ParseDouble(fields[2]),
            CloseDate = ParseDate(fields[3]),
            ClosePrice = ParseDouble(fields[4]),
            Volume = ParseInt(fields[5]),
            NetPnl = ParseDouble(fields[6]),
            NetPnlPercentage = ParsePercentage(fields[7]),
            DurationDays = ParseInt(fields[8]),
        });
    }

    public static List<Trade> ReadPartialSells(string path)
    {
        return ReadTrades(path, PartialSellColumnCount, "partial sell", fields => new Trade {
            SymbolCode = fields[0],
            ReutersCode = fields[0],
            CloseDate = ParseDate(fields[1]),
            ClosePrice = ParseDouble(fields[2]),
            Volume = ParseInt(fields[3]),
            NetPnl = ParseDouble(fields[4]),
            NetPnlPercentage = ParsePercentage(fields[5]),
        });
    }

    private static List<Trade> ReadTrades(string path, int columnCount, string rowKind, Func<string[], Trade> map)
    {
        var trades = new List<Trade>();
        foreach (var (line, fields) in CsvParser.ReadRows(path))
        {
            if (fields.Length < columnCount)
            {
                Console.Error.WriteLine($"Skipping malformed {rowKind} row: {line}");
                continue;
            }

            trades.Add(map(fields));
        }
        return trades;
    }

    private static DateTime ParseDate(string value)
    {
        return DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    private static double ParseDouble(string value)
    {
        return double.Parse(value, CultureInfo.InvariantCulture);
    }

    private static int ParseInt(string value)
    {
        return int.Parse(value, CultureInfo.InvariantCulture);
    }

    private static double ParsePercentage(string value)
    {
        return ParseDouble(value.TrimEnd('%'));
    }
}
