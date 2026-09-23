using System.Text.Json;
using BorsaReturns.Features.Purification;

namespace BorsaReturns.Features.Persistance;

/// <summary>Writes the generated results into the output folder.</summary>
public class OutputWriter
{
    private static readonly JsonSerializerOptions ReportJsonOptions = new() { WriteIndented = true };

    private readonly string _path;

    public OutputWriter(string path)
    {
        _path = path;
        Directory.CreateDirectory(path);
    }

    public string WriteDashboard(string html)
    {
        return Write("trades_journal_dashboard.html", html);
    }

    public string WriteNonCompliantEarnings(NonCompliantEarningReport report)
    {
        return Write("non_compliant_earnings.json", JsonSerializer.Serialize(report, ReportJsonOptions));
    }

    public string WriteNonCompliantDividends(NonCompliantEarningReport report)
    {
        return Write("non_compliant_dividends.json", JsonSerializer.Serialize(report, ReportJsonOptions));
    }

    /// <summary>Writes the file and returns its full path.</summary>
    private string Write(string fileName, string content)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_path, fileName));
        File.WriteAllText(fullPath, content);
        return fullPath;
    }
}
