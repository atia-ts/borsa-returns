using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BorsaReturns.Features.Purification;

/// <summary>
/// The "Purification" section of appsettings.json.
/// FallbackYear / FallbackQuarter pick the quarter used for sells whose candidate quarters are missing;
/// both empty (null or "") means no fallback quarter.
/// FallbackPercentage is the last resort, used when neither the candidate quarters nor the fallback quarter
/// have data (including stocks absent from stocks-info.json). It defaults to 10 when the key is absent;
/// set it to empty to keep such sells blocked.
/// DividendFallbackPercentage is used for dividends whose prior-year Q4 (or stock) is missing; dividends have no
/// fallback quarter. It defaults to 0 when the key is absent; set it to empty to keep such dividends blocked.
/// </summary>
public class PurificationSettings
{
    public const double DefaultFallbackPercentage = 10;

    public const double DefaultDividendFallbackPercentage = 0;

    public int? FallbackYear { get; init; }

    public int? FallbackQuarter { get; init; }

    public double? FallbackPercentage { get; init; } = DefaultFallbackPercentage;

    public double? DividendFallbackPercentage { get; init; } = DefaultDividendFallbackPercentage;

    public (int Year, int Quarter)? Fallback =>
        FallbackYear is { } year && FallbackQuarter is { } quarter ? (year, quarter) : null;

    /// <summary>Loads the section from the given appsettings.json; a missing file or section means the defaults.</summary>
    public static PurificationSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return new PurificationSettings();
        }

        var section = JsonNode.Parse(File.ReadAllText(path), documentOptions: new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        })?["Purification"] as JsonObject;

        var settings = new PurificationSettings
        {
            FallbackYear = ReadOptionalInt(section, "FallbackYear"),
            FallbackQuarter = ReadOptionalInt(section, "FallbackQuarter"),
            FallbackPercentage = section?.ContainsKey("FallbackPercentage") == true
                ? ReadOptionalDouble(section, "FallbackPercentage")
                : DefaultFallbackPercentage,
            DividendFallbackPercentage = section?.ContainsKey("DividendFallbackPercentage") == true
                ? ReadOptionalDouble(section, "DividendFallbackPercentage")
                : DefaultDividendFallbackPercentage,
        };

        if (settings.FallbackYear.HasValue != settings.FallbackQuarter.HasValue)
        {
            throw new InvalidOperationException(
                "Purification:FallbackYear and Purification:FallbackQuarter must both be set, or both be empty.");
        }

        if (settings.FallbackQuarter is < 1 or > 4)
        {
            throw new InvalidOperationException(
                $"Purification:FallbackQuarter must be between 1 and 4 (got {settings.FallbackQuarter}).");
        }

        if (settings.FallbackPercentage is < 0 or > 100)
        {
            throw new InvalidOperationException(
                $"Purification:FallbackPercentage must be between 0 and 100 (got {settings.FallbackPercentage}).");
        }

        if (settings.DividendFallbackPercentage is < 0 or > 100)
        {
            throw new InvalidOperationException(
                $"Purification:DividendFallbackPercentage must be between 0 and 100 (got {settings.DividendFallbackPercentage}).");
        }

        return settings;
    }

    private static int? ReadOptionalInt(JsonObject? section, string key)
    {
        return ReadOptionalText(section, key) is { } value
            ? int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
                ? result
                : throw new InvalidOperationException($"Purification:{key} must be a whole number or empty (got {value}).")
            : null;
    }


    private static double? ReadOptionalDouble(JsonObject? section, string key)
    {
        return ReadOptionalText(section, key) is { } value
            ? double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : throw new InvalidOperationException($"Purification:{key} must be a number or empty (got {value}).")
            : null;
    }


    /// <summary>Accepts null, "", a number, or a numeric string; returns null when empty.</summary>
    private static string? ReadOptionalText(JsonObject? section, string key)
    {
        var node = section?[key];
        if (node is null)
        {
            return null;
        }

        var value = node.GetValueKind() == JsonValueKind.String ? node.GetValue<string>().Trim() : node.ToJsonString();
        return value.Length == 0 ? null : value;
    }
}
