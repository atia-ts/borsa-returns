# Borsa Returns — Solution Overview

## Architecture

This is a .NET console application that reads CSV trade data and generates a self-contained HTML dashboard with inline CSS and JavaScript. There is no web framework — the app produces a single HTML file that can be opened directly or served via any static file server.

### Project Structure

Types are grouped by feature under `src/BorsaReturns/Features/`; each folder is its own namespace (`BorsaReturns.Features.<Feature>`). `Program.cs` and `appsettings.json` stay at the project root.

| Feature folder | Types |
|---|---|
| `Trades/` | `Trade`, `TradeCsvReader`, `CsvParser` |
| `Metrics/` | `MetricsCalculator`, `DashboardMetrics`, `KpiMetrics`, `PeriodMetrics`, `ChartMetrics`, `ChartDataSet`, `TopStockMetrics`, `MonthlyCalendarEntry` |
| `Dividends/` | `Dividend`, `DividendCsvReader` |
| `Purification/` | `PurificationCalculator`, `PurificationSettings`, `NonCompliantEarning`, `NonCompliantEarningReport`, `StockInfo`, `QuarterlyEarning`, `StockInfoReader` |
| `Dashboard/` | `DashboardTemplate` |
| `Storage/` | `DataFolder`, `OutputWriter` |

New types go in the folder of the feature they serve; add a new folder when a type belongs to none of these.

### Data Flow

1. **CLI entry** (`Program.cs`) — Thin orchestrator: parses the optional commission argument, then wires together the helpers below. Reads from the fixed `data/` folder and writes to the fixed `output/` folder, both relative to the working directory (run from the repo root).
2. **Input discovery** (`Features/Storage/DataFolder.cs`) — Files are matched by pattern (`trading-journal-egypt-*.csv`, `partial-sells-egypt-*.csv`, `dividends*.csv`) and the latest file (by filename sort descending) is selected; `stocks-info.json` is used for non-compliant earnings.
3. **Parsing** — `Features/Trades/CsvParser.cs` splits quoted CSV rows; `Features/Trades/TradeCsvReader.cs` maps journal and partial sells rows to `Trade` objects (skipping malformed rows); `Features/Dividends/DividendCsvReader.cs` maps dividend rows to `Dividend` objects (skipping malformed rows); `Features/Purification/StockInfoReader.cs` loads `stocks-info.json`.
4. **Metrics calculation** (`Features/Metrics/MetricsCalculator.cs`) — Computes KPIs, period breakdowns, chart datasets, and calendar heatmap entries from the trade list; the Dividends KPI is the sum of the loaded dividends. Partial sells get their own period and calendar calculations.
5. **Non-compliant earnings** (`Features/Purification/PurificationCalculator.cs`) — Computes haram / clean amounts per partial sell (`CalculateSells`) and per dividend (`CalculateDividends`) from `stocks-info.json`. A partial sell uses the higher of the previous quarter and Q4 of the previous year (tie goes to Q4); a dividend uses only Q4 of the year before its entitlement date (the CSV `date`). Sell fallbacks come from `appsettings.json` (loaded by `Features/Purification/PurificationSettings.cs`): when candidate quarters are missing, the `Purification:FallbackYear` / `FallbackQuarter` quarter is used (when both are set); when that is unset or also missing, or the stock is absent from `stocks-info.json`, `Purification:FallbackPercentage` is used (default 10; empty = keep blocked). Dividends have no fallback quarter: when the prior-year Q4 (or the stock) is missing, `Purification:DividendFallbackPercentage` is used (default 0; empty = keep blocked). Fallback results carry `fallback_used` and a `note`. Runs before HTML generation so the results can be embedded in the dashboard.
6. **HTML generation** (`Features/Dashboard/DashboardTemplate.cs`) — A C# raw string literal (`$$"""..."""`) containing the full HTML page. Trade data and metrics are embedded as JSON constants that the client-side JavaScript consumes at load time.
7. **Output** (`Features/Storage/OutputWriter.cs`) — Writes `trades_journal_dashboard.html`, `non_compliant_earnings.json` (partial sells) and `non_compliant_dividends.json` into `output/`.

### Key Data Models

- `Trade` — Unified model for both full trades and partial sells. Partial sells leave `OpenDate`, `AvgEntryPrice`, and `DurationDays` at their defaults (not present in the CSV).
- `DashboardMetrics` — Top-level container holding KPIs, periods, charts, calendar entries, and partial sells data.
- `KpiMetrics`, `PeriodMetrics`, `ChartMetrics`, `ChartDataSet`, `TopStockMetrics` — Typed containers for each dashboard section.

## Dashboard Sections

The generated HTML dashboard contains these sections (in order):

1. **Masthead & sticky navigation** — Serif title, Arabic / English language toggle (saved to `localStorage` as `br-lang`, defaults to Arabic), light/dark theme toggle (saved as `br-theme`, defaults to the OS setting), and a pill nav that highlights the section in view. Trade-only links hide when there is no trading journal; partial sells links hide when there are no partial sells; the Dividends link hides when there are no dividends.
2. **Overview** — Hero card (Total Returns with a cumulative P&L sparkline, or Partial Sells P&L when there is no journal), side stats (Win rate meter, Top stock, Best trade), and a **Purification** strip summarising the non-compliant earnings report (hidden when no report exists).
3. **Summary KPI cards** — 12 cards (Total P&L, Dividends, Commission, Net P&L, Win Rate, Avg P&L, Best/Worst Trade, Profit Factor, Avg Profit/Loss, Avg Holding Period).
4. **P&L by Period** — Time-bucketed cards (This Month, Past 3/6 Months, Past 1/2 Years, All Time) plus a Net P&L summary.
5. **Trades Calendar** — Monthly heatmap grouped by year, color-coded green/red.
6. **Partial Sells P&L by Period** — Same period layout for partial sells (hidden if no data).
7. **Partial Sells Calendar** — Same calendar layout for partial sells (hidden if no data).
8. **Charts** — 2×2 grid of Chart.js charts (Cumulative P&L, P&L by Stock, Monthly P&L, Win Rate by Stock).
9. **Trades table** — Full trade history with filters (Stock, Outcome, Date range, P&L range) and sortable columns (Stock, Open/Close Date, Volume, Entry/Close Price, P&L, Net P&L, P&L %, Duration, Commission).
10. **Partial Sells table** — All partial sell records with the same filter/sort pattern. Columns: Stock, Exit Date, Exit Price, Volume, P/L, P/L %, Non-compliant Earning %, Non-compliant Earning, Net Total. The last three come from the non-compliant earnings report (`non_compliant_percentage`, `haram`, `clean_total`); they show `—` (with the blocked reason as a tooltip) for blocked sells and are omitted when no report was produced. Hidden when no partial sells data exists.
11. **Dividends table** — All dividend payouts from the dividends CSV with filters (Stock, Date range, Amount range) and sortable columns (Stock, Date, Amount, Non-compliant Earning %, Non-compliant Earning, Net Total). The last three come from the non-compliant dividends report and render exactly like the partial sells ones: `—` with the blocked reason as a tooltip, fallback values marked with a dotted underline and the `note` as a tooltip, and omitted when no report was produced. Hidden when no dividends data exists.

## Design Patterns

- **Localization** — The dashboard is bilingual: Arabic (RTL, the default) and English. All UI text lives in the `I18N` dictionary (count phrases in `PLURALS`, using `Intl.PluralRules`) and is read through `tl(key, vars)`. Static markup carries `data-i18n` / `data-i18n-title` / `data-i18n-aria` / `data-i18n-placeholder` keys filled by `applyStaticText()`; switching language re-runs `renderAll()`. Period labels from `MetricsCalculator` are mapped via `AR_PERIODS`. Stock symbols, and the purification report's `blocked_reason` / `note`, stay in English. Numbers keep Latin digits (`ar-EG-u-nu-latn`) and are wrapped in an LTR isolate (`ltr()`) so signs and `%` stay in place inside Arabic text. CSS uses logical properties (`margin-inline-start`, `text-align: start`, …) so layout flips with `dir`; charts reverse their x-axis and move the y-axis to the right in RTL. New UI text must be added to both languages.
- **Theming** — Colors are CSS custom properties on `:root`, overridden under `:root[data-theme="light"]`; charts read them via `cssVar()` and re-render on theme change.
- **Conditional visibility** — Elements marked `data-trades` are hidden when `ALL_TRADES` is empty. Partial sells sections use `style="display:none"` in HTML and are shown via JavaScript when `ALL_PARTIAL_SELLS.length > 0`.
- **Reusable rendering** — `renderPeriodCards()` and `renderCalendar()` are shared between main trades and partial sells sections.
- **Client-side filtering/sorting** — Each table has its own independent state (`sortCol`/`sortDir` for trades, `psSortCol`/`psSortDir` for partial sells, `divSortCol`/`divSortDir` for dividends) and filter controls. Filters are applied in `getFiltered()` / `getPsFiltered()` / `getDivFiltered()` and the table re-renders on any change.
- **JSON property naming** — C# properties use `[JsonPropertyName("snake_case")]` attributes so the JavaScript side receives consistent snake_case keys.

## Partial Sells CSV Format

| Column | Type | Notes |
|--------|------|-------|
| Symbol | string | Stock ticker |
| Exit date | string | `dd/MM/yyyy` format |
| Exit Price | double | |
| Volume | int | |
| P/L | double | |
| P/L % | double | Trailing `%` stripped during parse |

## Dividends CSV Format

| Column | Type | Notes |
|--------|------|-------|
| symbol | string | Stock ticker |
| date | string | `yyyy-MM-dd` format |
| amount | double | Payout amount in EGP |
