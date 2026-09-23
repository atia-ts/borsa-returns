using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using BorsaReturns.Features.Dashboard;
using BorsaReturns.Features.Dividends;
using BorsaReturns.Features.Metrics;
using BorsaReturns.Features.Purification;
using BorsaReturns.Features.Persistance;
using BorsaReturns.Features.Trades;

var totalCommission = args.Length > 0 ? double.Parse(args[0], CultureInfo.InvariantCulture) : 0;

var data = new DataFolder("data");
if (!data.Exists)
{
    Console.Error.WriteLine($"Folder not found: {data.Path}");
    Environment.Exit(1);
}

var journalFile = data.JournalFile;
var partialSellsFile = data.PartialSellsFile;
var dividendsFile = data.DividendsFile;

if (journalFile is null && partialSellsFile is null)
{
    Console.Error.WriteLine($"No trading journal or partial sells file found in: {data.Path}");
    Environment.Exit(1);
}

Console.WriteLine(journalFile != null
    ? $"Using trading journal: {Path.GetFileName(journalFile)}"
    : "No trading journal file found — skipping.");
Console.WriteLine(partialSellsFile != null
    ? $"Using partial sells: {Path.GetFileName(partialSellsFile)}"
    : "No partial sells file found — skipping.");
Console.WriteLine(dividendsFile != null
    ? $"Using dividends: {Path.GetFileName(dividendsFile)}"
    : "No dividends file found — skipping.");

// Trades
var trades = journalFile is null ? [] : TradeCsvReader.ReadJournal(journalFile);
if (journalFile != null && trades.Count == 0)
{
    Console.Error.WriteLine("No trades found in trading journal.");
    Environment.Exit(1);
}
Console.WriteLine($"Loaded {trades.Count} trades");

var partialSells = partialSellsFile is null ? [] : TradeCsvReader.ReadPartialSells(partialSellsFile);
if (partialSellsFile != null)
    Console.WriteLine($"Loaded {partialSells.Count} partial sells");

var dividends = dividendsFile is null ? [] : DividendCsvReader.Read(dividendsFile);
if (dividendsFile != null)
    Console.WriteLine($"Loaded {dividends.Count} dividends totalling {dividends.Sum(d => d.Amount).ToString("N2", CultureInfo.InvariantCulture)}");

// Metrics
var now = DateTime.UtcNow;
var metrics = MetricsCalculator.Calculate(trades, dividends, totalCommission, now);
if (partialSells.Count > 0)
{
    metrics.PartialSellsPeriods = MetricsCalculator.CalculatePartialSellsPeriods(partialSells, now);
    metrics.PartialSellsCalendar = MetricsCalculator.BuildMonthlyCalendar(partialSells);
}

var output = new OutputWriter("output");

// Non-compliant (haram) earnings on partial sells and dividends
var nonCompliantItems = new List<NonCompliantEarning>();
var nonCompliantDividendItems = new List<NonCompliantEarning>();
if (!File.Exists(data.StocksInfoFile))
{
    Console.WriteLine($"No stocks info file found ({Path.GetFileName(data.StocksInfoFile)}) — skipping non-compliant earnings.");
}
else if (partialSells.Count == 0 && dividends.Count == 0)
{
    Console.WriteLine("No partial sells or dividends — skipping non-compliant earnings.");
}
else
{
    var stocks = StockInfoReader.Read(data.StocksInfoFile);
    Console.WriteLine($"Loaded stocks info for {stocks.Count} symbols");

    PurificationSettings settings;
    try
    {
        settings = PurificationSettings.Load(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
    }
    catch (Exception ex) when (ex is InvalidOperationException or System.Text.Json.JsonException)
    {
        Console.Error.WriteLine($"Invalid appsettings.json: {ex.Message}");
        Environment.Exit(1);
        return;
    }

    Console.WriteLine(settings.Fallback is { } fallback
        ? $"Purification fallback quarter: {fallback.Year}-Q{fallback.Quarter}"
        : "No purification fallback quarter configured.");
    Console.WriteLine(settings.FallbackPercentage is { } fallbackPercentage
        ? $"Purification fallback percentage: {fallbackPercentage.ToString(CultureInfo.InvariantCulture)}%"
        : "No purification fallback percentage configured — sells with no usable data stay blocked.");
    Console.WriteLine(settings.DividendFallbackPercentage is { } dividendFallbackPercentage
        ? $"Dividend purification fallback percentage: {dividendFallbackPercentage.ToString(CultureInfo.InvariantCulture)}%"
        : "No dividend purification fallback percentage configured — dividends with no usable data stay blocked.");

    if (partialSells.Count > 0)
    {
        var report = PurificationCalculator.CalculateSells(partialSells, stocks, settings, now);
        nonCompliantItems = report.Items;
        PrintReport("Non-compliant earnings", report, output.WriteNonCompliantEarnings(report));
    }

    if (dividends.Count > 0)
    {
        var report = PurificationCalculator.CalculateDividends(dividends, stocks, settings, now);
        nonCompliantDividendItems = report.Items;
        PrintReport("Non-compliant dividends", report, output.WriteNonCompliantDividends(report));
    }
}

// Dashboard
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false
};

var html = DashboardTemplate.Generate(
    JsonSerializer.Serialize(trades, jsonOptions),
    JsonSerializer.Serialize(metrics, jsonOptions),
    JsonSerializer.Serialize(partialSells, jsonOptions),
    JsonSerializer.Serialize(nonCompliantItems, jsonOptions),
    JsonSerializer.Serialize(dividends, jsonOptions),
    JsonSerializer.Serialize(nonCompliantDividendItems, jsonOptions));

var dashboardPath = output.WriteDashboard(html);
Console.WriteLine($"Dashboard written to: {dashboardPath}");

Process.Start(new ProcessStartInfo(dashboardPath) { UseShellExecute = true });
            Console.WriteLine("Opened in default browser.");

static void PrintReport(string title, NonCompliantEarningReport report, string path)
{
    Console.WriteLine($"{title}: {report.ComputedCount} computed, {report.BlockedCount} blocked, {report.FallbackCount} via fallback — " +
        $"haram {report.TotalHaram.ToString("N2", CultureInfo.InvariantCulture)}, clean {report.TotalClean.ToString("N2", CultureInfo.InvariantCulture)}");
    Console.WriteLine($"{title} written to: {path}");
}
