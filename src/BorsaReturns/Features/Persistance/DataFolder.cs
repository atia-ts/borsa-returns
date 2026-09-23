namespace BorsaReturns.Features.Persistance;

/// <summary>Locates the input files inside the data folder.</summary>
public class DataFolder(string path)
{
    public string Path { get; } = path;

    public bool Exists => Directory.Exists(Path);

    public string? JournalFile => Latest("trading-journal-egypt-*.csv");

    public string? PartialSellsFile => Latest("partial-sells-egypt-*.csv");

    public string? DividendsFile => Latest("dividends*.csv");

    public string StocksInfoFile => System.IO.Path.Combine(Path, "stocks-info.json");

    /// <summary>The latest file matching the pattern, by filename sort descending.</summary>
    private string? Latest(string pattern)
    {
        return Directory.GetFiles(Path, pattern)
            .OrderByDescending(f => System.IO.Path.GetFileName(f))
            .FirstOrDefault();
    }

}
