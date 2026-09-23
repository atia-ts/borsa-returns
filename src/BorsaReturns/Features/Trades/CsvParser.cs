namespace BorsaReturns.Features.Trades;

public static class CsvParser
{
    /// <summary>Yields each non-blank data row with its parsed fields, skipping the header.</summary>
    public static IEnumerable<(string Line, string[] Fields)> ReadRows(string path)
    {
        return File.ReadLines(path)
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => (line, ParseLine(line)));
    }


    public static string[] ParseLine(string line)
    {
        var fields = new List<string>();
        var i = 0;

        while (i < line.Length)
        {
            if (line[i] == '"')
            {
                i++;

                var start = i;
                
                while (i < line.Length)
                {
                    if (line[i] == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            i += 2;
                            continue;
                        }
                        break;
                    }

                    i++;
                }

                fields.Add(line[start..i].Replace("\"\"", "\""));
                i++; // closing quote

                if (i < line.Length && line[i] == ',') {
                    i++; // comma
                }
            }
            else
            {
                var start = i;
                while (i < line.Length && line[i] != ',') {
                    i++;
                }

                fields.Add(line[start..i]);

                if (i < line.Length) {
                    i++; // comma
                }

            }
        }

        return [.. fields];
    }
}
