using System.Text.Json;

namespace BorsaReturns.Features.Purification;

public static class StockInfoReader
{
    public static List<StockInfo> Read(string path)
    {
        return JsonSerializer.Deserialize<List<StockInfo>>(File.ReadAllText(path)) ?? [];
    }

}
