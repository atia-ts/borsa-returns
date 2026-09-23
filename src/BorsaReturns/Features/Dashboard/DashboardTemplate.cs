namespace BorsaReturns.Features.Dashboard;

public static class DashboardTemplate
{
    public static string Generate(string tradesJson, string metricsJson, string partialSellsJson, string nonCompliantJson, string dividendsJson, string nonCompliantDividendsJson)
    {
        return $$"""
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Borsa Returns · عوائد البورصة</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Instrument+Serif:ital@0;1&family=IBM+Plex+Sans+Arabic:wght@400;500;600;700&family=Amiri:ital@0;1&display=swap" rel="stylesheet">
<style>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

/* ── Tokens ─────────────────────────────────────────── */
:root {
  color-scheme: dark;
  --bg: #0b0c10; --bg-glow: rgba(201,164,92,0.07);
  --surface: #13151b; --surface2: #1a1d25; --surface3: #232733;
  --border: rgba(255,255,255,0.07); --border-strong: rgba(255,255,255,0.13);
  --text: #ecedf1; --text2: #9a9daa; --text3: #6b6e7a;
  --pos: #34c98e; --neg: #f0646f;
  --pos-soft: rgba(52,201,142,0.13); --neg-soft: rgba(240,100,111,0.13);
  --accent: #d4ae68; --accent-soft: rgba(212,174,104,0.12); --accent-ink: #1a1408;
  --shadow: 0 1px 0 rgba(255,255,255,0.03) inset, 0 12px 32px -16px rgba(0,0,0,0.6);
  --radius: 16px;
  --serif: 'Instrument Serif', 'Amiri', Georgia, serif;
  --sans: 'Inter', 'IBM Plex Sans Arabic', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}
:root[data-theme="light"] {
  color-scheme: light;
  --bg: #f6f4ef; --bg-glow: rgba(166,124,46,0.08);
  --surface: #ffffff; --surface2: #f3f1ec; --surface3: #e9e6df;
  --border: rgba(24,22,18,0.08); --border-strong: rgba(24,22,18,0.16);
  --text: #17181c; --text2: #5a5c64; --text3: #8b8d95;
  --pos: #0e8f5a; --neg: #cf3b48;
  --pos-soft: rgba(14,143,90,0.10); --neg-soft: rgba(207,59,72,0.10);
  --accent: #9a7129; --accent-soft: rgba(154,113,41,0.10); --accent-ink: #ffffff;
  --shadow: 0 1px 2px rgba(24,22,18,0.04), 0 12px 32px -18px rgba(24,22,18,0.18);
}

html { scroll-behavior: smooth; scroll-padding-top: 88px; }
body {
  font-family: var(--sans); color: var(--text); line-height: 1.5; min-height: 100vh;
  background: radial-gradient(1200px 600px at 85% -10%, var(--bg-glow), transparent 60%), var(--bg);
  background-attachment: fixed;
  -webkit-font-smoothing: antialiased;
  transition: background-color 0.3s, color 0.3s;
}
.container { max-width: 1320px; margin: 0 auto; padding: 0 28px 64px; }
.num { font-variant-numeric: tabular-nums; }
.positive { color: var(--pos); }
.negative { color: var(--neg); }
.muted { color: var(--text2); }
.blocked { color: var(--text3); cursor: help; }
.fallback { cursor: help; }
.fallback-mark { display: inline-block; width: 6px; height: 6px; border-radius: 50%; background: var(--neg); margin-inline-end: 7px; vertical-align: middle; }
.fallback .fallback-value { text-decoration: underline dotted var(--neg); text-underline-offset: 3px; }

/* ── Masthead ───────────────────────────────────────── */
.masthead { display: flex; justify-content: space-between; align-items: flex-end; gap: 24px; padding: 48px 0 28px; }
.eyebrow { font-size: 0.72rem; font-weight: 600; letter-spacing: 0.18em; text-transform: uppercase; color: var(--accent); margin-bottom: 10px; display: flex; align-items: center; gap: 10px; }
.eyebrow::before { content: ''; width: 24px; height: 1px; background: var(--accent); }
h1 { font-family: var(--serif); font-weight: 400; font-size: clamp(2.6rem, 5vw, 3.8rem); line-height: 1; letter-spacing: -0.01em; }
h1 em { color: var(--accent); }
.masthead-meta { display: flex; align-items: center; gap: 14px; color: var(--text2); font-size: 0.82rem; }
.theme-toggle {
  width: 40px; height: 40px; border-radius: 50%; border: 1px solid var(--border-strong); background: var(--surface);
  color: var(--text); cursor: pointer; display: grid; place-items: center; transition: transform 0.2s, border-color 0.2s;
}
.theme-toggle:hover { border-color: var(--accent); transform: rotate(-15deg); }
.lang-toggle { font-family: var(--sans); font-size: 0.8rem; font-weight: 600; }
.lang-toggle:hover { transform: none; }
.theme-toggle svg { width: 18px; height: 18px; }
.theme-toggle .sun { display: none; }
:root[data-theme="light"] .theme-toggle .sun { display: block; }
:root[data-theme="light"] .theme-toggle .moon { display: none; }

/* ── Navigation ─────────────────────────────────────── */
.nav-wrap { position: sticky; top: 12px; z-index: 100; margin-bottom: 36px; }
.nav {
  display: flex; gap: 4px; padding: 6px; overflow-x: auto; scrollbar-width: none;
  background: color-mix(in srgb, var(--surface) 72%, transparent);
  backdrop-filter: blur(16px) saturate(140%); -webkit-backdrop-filter: blur(16px) saturate(140%);
  border: 1px solid var(--border); border-radius: 999px; box-shadow: var(--shadow); width: fit-content; max-width: 100%;
}
.nav::-webkit-scrollbar { display: none; }
.nav a {
  color: var(--text2); text-decoration: none; font-size: 0.82rem; font-weight: 500; padding: 7px 16px;
  border-radius: 999px; white-space: nowrap; transition: color 0.2s, background 0.2s;
}
.nav a:hover { color: var(--text); }
.nav a.active { color: var(--accent-ink); background: var(--accent); }

/* ── Sections ───────────────────────────────────────── */
section { margin-bottom: 56px; }
.section-head { display: flex; align-items: baseline; gap: 14px; margin-bottom: 20px; padding-bottom: 12px; border-bottom: 1px solid var(--border); }
.section-index { font-family: var(--serif); font-style: italic; color: var(--accent); font-size: 1.1rem; }
.section-title { font-family: var(--serif); font-weight: 400; font-size: 1.85rem; line-height: 1.1; }
.section-note { margin-inline-start: auto; font-size: 0.8rem; color: var(--text3); }
.hero > *, .hero-side > *, .charts-grid > *, .purify > *, .period-grid > *, .kpi-grid > * { min-width: 0; }
.card { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow); }

/* ── Hero ───────────────────────────────────────────── */
.hero { display: grid; grid-template-columns: 1.55fr 1fr; gap: 20px; margin-bottom: 20px; }
.hero-main { padding: 32px; position: relative; overflow: hidden; }
.hero-main::after {
  content: ''; position: absolute; inset: 0; pointer-events: none; border-radius: inherit;
  background: linear-gradient(135deg, var(--accent-soft), transparent 45%);
}
.hero-label { font-size: 0.78rem; font-weight: 600; letter-spacing: 0.12em; text-transform: uppercase; color: var(--text2); }
.hero-value { font-size: clamp(2.8rem, 6vw, 4.2rem); font-weight: 600; letter-spacing: -0.03em; line-height: 1.05; margin: 10px 0 8px; }
.hero-sub { color: var(--text2); font-size: 0.88rem; }
.hero-sub b { color: var(--text); font-weight: 600; }
.hero-spark { position: relative; height: 110px; margin-top: 24px; }
.hero-side { display: grid; grid-template-rows: repeat(3, 1fr); gap: 20px; }
.stat { padding: 20px 22px; display: flex; flex-direction: column; justify-content: center; }
.stat-label { font-size: 0.75rem; font-weight: 500; color: var(--text2); display: flex; justify-content: space-between; }
.stat-value { font-size: 1.55rem; font-weight: 600; letter-spacing: -0.02em; margin-top: 2px; }
.stat-value small { font-size: 0.85rem; font-weight: 500; color: var(--text2); margin-inline-start: 6px; letter-spacing: 0; }
.meter { height: 6px; border-radius: 99px; background: var(--neg-soft); margin-top: 10px; overflow: hidden; }
.meter > span { display: block; height: 100%; border-radius: 99px; background: var(--pos); transition: width 1s cubic-bezier(.2,.8,.2,1); }
.ticker {
  display: inline-block; font-size: 0.72rem; font-weight: 600; letter-spacing: 0.06em; padding: 3px 8px; border-radius: 6px;
  background: var(--surface3); color: var(--text); border: 1px solid var(--border);
}

/* ── Purification strip ─────────────────────────────── */
.purify { display: grid; grid-template-columns: auto 1fr 1fr 1.2fr; align-items: center; gap: 0; margin-bottom: 20px; padding: 6px; border-color: color-mix(in srgb, var(--accent) 35%, var(--border)); }
.purify > div { padding: 16px 22px; }
.purify > div + div { border-inline-start: 1px solid var(--border); }
.purify-badge { display: flex; align-items: center; gap: 12px; }
.purify-icon { width: 40px; height: 40px; border-radius: 12px; background: var(--accent-soft); color: var(--accent); display: grid; place-items: center; }
.purify-icon svg { width: 20px; height: 20px; }
.purify-title { font-family: var(--serif); font-size: 1.35rem; line-height: 1.1; }
.purify-caption { font-size: 0.75rem; color: var(--text3); }

/* ── KPI grid ───────────────────────────────────────── */
.kpi-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(190px, 1fr)); gap: 14px; }
.kpi { padding: 18px 20px; transition: transform 0.25s, border-color 0.25s; }
.kpi:hover { transform: translateY(-2px); border-color: var(--border-strong); }
.kpi-label { font-size: 0.75rem; font-weight: 500; color: var(--text2); margin-bottom: 6px; }
.kpi-value { font-size: 1.3rem; font-weight: 600; letter-spacing: -0.02em; }
.kpi-sub { font-size: 0.75rem; color: var(--text3); margin-top: 4px; }

/* ── Periods ────────────────────────────────────────── */
.period-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(170px, 1fr)); gap: 14px; }
.period-card { padding: 18px 20px; position: relative; overflow: hidden; }
.period-card::before { content: ''; position: absolute; inset-inline-start: 0; top: 18px; bottom: 18px; width: 3px; border-start-end-radius: 3px; border-end-end-radius: 3px; background: var(--bar, var(--border)); }
.period-label { font-size: 0.75rem; font-weight: 500; color: var(--text2); margin-bottom: 6px; }
.period-value { font-size: 1.3rem; font-weight: 600; letter-spacing: -0.02em; }
.period-trades { font-size: 0.75rem; color: var(--text3); margin-top: 4px; }
.period-summary { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; margin-top: 14px; }
.period-card.featured { background: linear-gradient(135deg, var(--accent-soft), transparent 70%), var(--surface); border-color: color-mix(in srgb, var(--accent) 40%, var(--border)); }
.period-card.featured .period-value { font-size: 1.6rem; }

/* ── Calendar ───────────────────────────────────────── */
.calendar-year { padding: 20px 22px; margin-bottom: 14px; }
.calendar-year-label { display: flex; align-items: baseline; justify-content: space-between; margin-bottom: 14px; }
.calendar-year-label > span:first-child { font-family: var(--serif); font-size: 1.5rem; }
.calendar-year-total { font-size: 0.95rem; font-weight: 600; }
.calendar-grid { display: grid; grid-template-columns: repeat(12, 1fr); gap: 6px; }
.calendar-cell { border-radius: 10px; padding: 10px 6px; text-align: center; background: var(--surface2); border: 1px solid transparent; transition: transform 0.2s; }
.calendar-cell:not(.empty):hover { transform: scale(1.05); border-color: var(--border-strong); }
.calendar-cell.profitable { background: color-mix(in srgb, var(--pos) calc(var(--i) * 1%), var(--surface2)); }
.calendar-cell.losing { background: color-mix(in srgb, var(--neg) calc(var(--i) * 1%), var(--surface2)); }
.calendar-cell.empty { opacity: 0.45; }
.calendar-month { font-size: 0.66rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.08em; color: var(--text2); margin-bottom: 4px; }
.calendar-pnl { font-size: 0.82rem; font-weight: 600; font-variant-numeric: tabular-nums; }
.calendar-trades { font-size: 0.64rem; color: var(--text3); margin-top: 2px; }

/* ── Charts ─────────────────────────────────────────── */
.charts-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.chart-card { padding: 22px 22px 16px; }
.chart-card h3 { font-size: 0.9rem; font-weight: 600; margin-bottom: 2px; }
.chart-card p { font-size: 0.75rem; color: var(--text3); margin-bottom: 16px; }
.chart-box { position: relative; height: 260px; }

/* ── Filters ────────────────────────────────────────── */
.controls { padding: 16px 18px; margin-bottom: 14px; display: flex; flex-wrap: wrap; gap: 12px; align-items: flex-end; }
.control-group { display: flex; flex-direction: column; gap: 5px; }
.control-group label { font-size: 0.7rem; font-weight: 600; letter-spacing: 0.06em; text-transform: uppercase; color: var(--text3); }
select, input[type="date"], input[type="number"] {
  font-family: inherit; background: var(--surface2); border: 1px solid var(--border); color: var(--text);
  padding: 8px 12px; border-radius: 10px; font-size: 0.85rem; min-width: 140px; transition: border-color 0.2s, box-shadow 0.2s;
}
select:focus, input:focus { outline: none; border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-soft); }
.btn { font-family: inherit; background: var(--accent); color: var(--accent-ink); border: none; padding: 9px 20px; border-radius: 10px; cursor: pointer; font-size: 0.85rem; font-weight: 600; transition: all 0.2s; }
.btn-outline { background: transparent; border: 1px solid var(--border-strong); color: var(--text2); }
.btn-outline:hover { border-color: var(--accent); color: var(--accent); }

/* ── Tables ─────────────────────────────────────────── */
.table-wrapper { overflow: hidden; }
.table-scroll { overflow-x: auto; }
table { width: 100%; border-collapse: collapse; }
th {
  background: var(--surface2); padding: 12px 16px; text-align: start; font-size: 0.7rem; font-weight: 600;
  text-transform: uppercase; letter-spacing: 0.06em; color: var(--text3); cursor: pointer; user-select: none; white-space: nowrap;
  transition: color 0.2s;
}
th:hover, th.sorted { color: var(--text); }
th.sorted { box-shadow: inset 0 -2px 0 var(--accent); }
th .sort-icon { margin-inline-start: 5px; font-size: 0.6rem; opacity: 0.6; }
th.n, td.n { text-align: end; }
td { padding: 12px 16px; border-top: 1px solid var(--border); font-size: 0.86rem; white-space: nowrap; font-variant-numeric: tabular-nums; }
tbody tr { transition: background 0.15s; }
tbody tr:hover td { background: var(--surface2); }
.table-footer { padding: 14px 18px; border-top: 1px solid var(--border); color: var(--text2); font-size: 0.8rem; display: flex; flex-wrap: wrap; gap: 8px 28px; }
.table-footer span:first-child { margin-inline-end: auto; }
.table-footer strong { font-variant-numeric: tabular-nums; }
.empty-row td { text-align: center; color: var(--text3); padding: 32px; }

/* ── Footer & misc ──────────────────────────────────── */
.page-footer { text-align: center; color: var(--text3); font-size: 0.78rem; padding-top: 24px; border-top: 1px solid var(--border); }
.page-footer .mark { font-family: var(--serif); font-style: italic; color: var(--accent); font-size: 1rem; }
.scroll-top {
  position: fixed; bottom: 28px; inset-inline-end: 28px; width: 44px; height: 44px; background: var(--accent); color: var(--accent-ink);
  border-radius: 50%; display: grid; place-items: center; text-decoration: none; font-size: 1.1rem; opacity: 0; transform: translateY(8px);
  pointer-events: none; transition: opacity 0.25s, transform 0.25s; z-index: 100; box-shadow: 0 8px 24px -8px rgba(0,0,0,0.5);
}
.scroll-top.visible { opacity: 1; transform: none; pointer-events: auto; }

/* ── RTL / Arabic ───────────────────────────────────── */
:root[dir="rtl"] body { background: radial-gradient(1200px 600px at 15% -10%, var(--bg-glow), transparent 60%), var(--bg); background-attachment: fixed; }
:root[dir="rtl"] .hero-main::after { background: linear-gradient(225deg, var(--accent-soft), transparent 45%); }
:root[dir="rtl"] .theme-toggle:not(.lang-toggle):hover { transform: rotate(15deg); }
/* Arabic script is cursive: letter-spacing breaks the joins, and it needs more line height and a touch more size. */
:root[lang="ar"] { font-size: 106.25%; }
:root[lang="ar"] :is(.eyebrow, h1, .section-title, .hero-label, .calendar-month, .control-group label, th, .purify-title) { letter-spacing: 0; }
:root[lang="ar"] h1 { line-height: 1.3; }
:root[lang="ar"] .section-title, :root[lang="ar"] .purify-title { line-height: 1.4; }

/* Reveal on scroll (only when JS enabled it and motion is welcome) */
.anim .reveal { opacity: 0; transform: translateY(14px); transition: opacity 0.7s cubic-bezier(.2,.8,.2,1), transform 0.7s cubic-bezier(.2,.8,.2,1); }
.anim .reveal.in { opacity: 1; transform: none; }
@media (prefers-reduced-motion: reduce) {
  html { scroll-behavior: auto; }
  *, *::before, *::after { transition: none !important; }
}

@media (max-width: 960px) {
  .hero { grid-template-columns: 1fr; }
  .hero-side { grid-template-rows: none; grid-template-columns: repeat(3, 1fr); }
  .purify { grid-template-columns: 1fr 1fr; }
  .purify > div + div { border-inline-start: none; }
  .calendar-grid { grid-template-columns: repeat(6, 1fr); }
}
@media (max-width: 720px) {
  .container { padding: 0 16px 48px; }
  .masthead { padding-top: 32px; align-items: flex-start; }
  .masthead-meta .generated { display: none; }
  .hero-main { padding: 24px; }
  .hero-side { grid-template-columns: minmax(0, 1fr); }
  .charts-grid, .period-summary, .purify { grid-template-columns: minmax(0, 1fr); }
  .kpi-grid, .period-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
  .hero-value { font-size: 2.4rem; }
  .controls { flex-direction: column; align-items: stretch; }
  .calendar-grid { grid-template-columns: repeat(4, 1fr); }
  .section-note { display: none; }
}
</style>
<script>
  // Apply the saved/OS theme and the saved language (Arabic by default) before first paint to avoid a flash.
  (function () {
    let t = null, l = null;
    try { t = localStorage.getItem('br-theme'); l = localStorage.getItem('br-lang'); } catch (e) {}
    if (!t) t = window.matchMedia && matchMedia('(prefers-color-scheme: light)').matches ? 'light' : 'dark';
    document.documentElement.dataset.theme = t;
    if (l !== 'en') l = 'ar';
    document.documentElement.lang = l;
    document.documentElement.dir = l === 'ar' ? 'rtl' : 'ltr';
    if (!matchMedia('(prefers-reduced-motion: reduce)').matches) document.documentElement.classList.add('anim');
  })();
</script>
</head>
<body>
<div class="container" id="top">
  <header class="masthead">
    <div>
      <div class="eyebrow" data-i18n="eyebrow"></div>
      <h1 data-i18n="appName"></h1>
    </div>
    <div class="masthead-meta">
      <span class="generated" id="generatedAt"></span>
      <button class="theme-toggle lang-toggle" id="langToggle" data-i18n="langSwitch" data-i18n-title="langSwitchTitle"></button>
      <button class="theme-toggle" id="themeToggle" data-i18n-title="themeToggle">
        <svg class="moon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8z"/></svg>
        <svg class="sun" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"><circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/></svg>
      </button>
    </div>
  </header>

  <!-- Navigation -->
  <div class="nav-wrap">
    <nav class="nav" id="nav">
      <a href="#overview" data-i18n="nav.overview"></a>
      <a href="#summary" data-trades data-i18n="summary"></a>
      <a href="#pnl-by-period" data-trades data-i18n="pnlByPeriod"></a>
      <a href="#partial-sells" id="navPartialSells" style="display:none" data-i18n="psByPeriod"></a>
      <a href="#charts" data-trades data-i18n="charts"></a>
      <a href="#trades" data-trades data-i18n="trades"></a>
      <a href="#partial-sells-table" id="navPartialSellsTable" style="display:none" data-i18n="partialSells"></a>
      <a href="#dividends" id="navDividends" style="display:none" data-i18n="dividends"></a>
    </nav>
  </div>

  <!-- Overview: hero + purification -->
  <section id="overview">
    <div class="hero">
      <div class="card hero-main reveal">
        <div class="hero-label" id="heroLabel"></div>
        <div class="hero-value num" id="heroValue"></div>
        <div class="hero-sub" id="heroSub"></div>
        <div class="hero-spark"><canvas id="heroSpark" data-i18n-aria="sparkline"></canvas></div>
      </div>
      <div class="hero-side" id="heroSide"></div>
    </div>
    <div class="card purify reveal" id="purify" style="display:none"></div>
  </section>

  <!-- KPI Cards -->
  <section id="summary" data-trades>
    <div class="section-head"><span class="section-index">i.</span><h2 class="section-title" data-i18n="summary"></h2></div>
    <div class="kpi-grid" id="kpiGrid"></div>
  </section>

  <!-- Period P&L -->
  <section id="pnl-by-period" data-trades>
    <div class="section-head"><span class="section-index">ii.</span><h2 class="section-title" data-i18n="pnlByPeriod"></h2></div>
    <div class="period-grid" id="periodGrid"></div>
    <div class="period-summary" id="periodSummary"></div>
  </section>

  <!-- Trades Calendar -->
  <section id="tradesCalendarSection" data-trades>
    <div class="section-head"><span class="section-index">iii.</span><h2 class="section-title" data-i18n="tradesCalendar"></h2><span class="section-note" data-i18n="calendarNote"></span></div>
    <div id="tradesCalendar"></div>
  </section>

  <!-- Partial Sells P&L -->
  <section id="partial-sells" style="display:none">
    <div class="section-head"><span class="section-index">iv.</span><h2 class="section-title" data-i18n="psByPeriod"></h2></div>
    <div class="period-grid" id="partialSellsGrid"></div>
    <div class="period-summary" id="partialSellsSummary"></div>
  </section>

  <!-- Partial Sells Calendar -->
  <section id="partialSellsCalendarSection" style="display:none">
    <div class="section-head"><span class="section-index">v.</span><h2 class="section-title" data-i18n="psCalendar"></h2></div>
    <div id="partialSellsCalendar"></div>
  </section>

  <!-- Charts -->
  <section id="charts" data-trades>
    <div class="section-head"><span class="section-index">vi.</span><h2 class="section-title" data-i18n="charts"></h2></div>
    <div class="charts-grid">
      <div class="card chart-card reveal"><h3 data-i18n="chart.cum"></h3><p data-i18n="chart.cumSub"></p><div class="chart-box"><canvas id="pnlChart"></canvas></div></div>
      <div class="card chart-card reveal"><h3 data-i18n="chart.stock"></h3><p data-i18n="chart.stockSub"></p><div class="chart-box"><canvas id="stockChart"></canvas></div></div>
      <div class="card chart-card reveal"><h3 data-i18n="chart.monthly"></h3><p data-i18n="chart.monthlySub"></p><div class="chart-box"><canvas id="monthlyChart"></canvas></div></div>
      <div class="card chart-card reveal"><h3 data-i18n="chart.winRate"></h3><p data-i18n="chart.winRateSub"></p><div class="chart-box"><canvas id="winRateChart"></canvas></div></div>
    </div>
  </section>

  <!-- Trades -->
  <section id="trades" data-trades>
    <div class="section-head"><span class="section-index">vii.</span><h2 class="section-title" data-i18n="trades"></h2></div>
    <div class="card controls" id="controls">
      <div class="control-group">
        <label data-i18n="col.stock"></label>
        <select id="filterStock"><option value="" data-i18n="filter.allStocks"></option></select>
      </div>
      <div class="control-group">
        <label data-i18n="filter.outcome"></label>
        <select id="filterOutcome">
          <option value="" data-i18n="filter.all"></option>
          <option value="win" data-i18n="filter.winners"></option>
          <option value="loss" data-i18n="filter.losers"></option>
        </select>
      </div>
      <div class="control-group">
        <label data-i18n="filter.from"></label>
        <input type="date" id="filterFrom">
      </div>
      <div class="control-group">
        <label data-i18n="filter.to"></label>
        <input type="date" id="filterTo">
      </div>
      <div class="control-group">
        <label data-i18n="filter.minPnl"></label>
        <input type="number" id="filterMinPnl" data-i18n-placeholder="filter.min">
      </div>
      <div class="control-group">
        <label data-i18n="filter.maxPnl"></label>
        <input type="number" id="filterMaxPnl" data-i18n-placeholder="filter.max">
      </div>
      <div class="control-group">
        <label>&nbsp;</label>
        <button class="btn-outline btn" onclick="resetFilters()" data-i18n="filter.reset"></button>
      </div>
    </div>

    <div class="card table-wrapper">
      <div class="table-scroll">
        <table>
          <thead><tr id="tableHead"></tr></thead>
          <tbody id="tableBody"></tbody>
        </table>
      </div>
      <div class="table-footer" id="tableFooter"></div>
    </div>
  </section>

  <!-- Partial Sells Table -->
  <section id="partial-sells-table" style="display:none">
    <div class="section-head"><span class="section-index">viii.</span><h2 class="section-title" data-i18n="partialSells"></h2><span class="section-note" id="psNote"></span></div>
    <div class="card controls" id="psControls">
      <div class="control-group">
        <label data-i18n="col.stock"></label>
        <select id="psFilterStock"><option value="" data-i18n="filter.allStocks"></option></select>
      </div>
      <div class="control-group">
        <label data-i18n="filter.outcome"></label>
        <select id="psFilterOutcome">
          <option value="" data-i18n="filter.all"></option>
          <option value="win" data-i18n="filter.winners"></option>
          <option value="loss" data-i18n="filter.losers"></option>
        </select>
      </div>
      <div class="control-group">
        <label data-i18n="filter.from"></label>
        <input type="date" id="psFilterFrom">
      </div>
      <div class="control-group">
        <label data-i18n="filter.to"></label>
        <input type="date" id="psFilterTo">
      </div>
      <div class="control-group">
        <label data-i18n="filter.minPnl"></label>
        <input type="number" id="psFilterMinPnl" data-i18n-placeholder="filter.min">
      </div>
      <div class="control-group">
        <label data-i18n="filter.maxPnl"></label>
        <input type="number" id="psFilterMaxPnl" data-i18n-placeholder="filter.max">
      </div>
      <div class="control-group">
        <label>&nbsp;</label>
        <button class="btn-outline btn" onclick="resetPsFilters()" data-i18n="filter.reset"></button>
      </div>
    </div>

    <div class="card table-wrapper">
      <div class="table-scroll">
        <table>
          <thead><tr id="psTableHead"></tr></thead>
          <tbody id="psTableBody"></tbody>
        </table>
      </div>
      <div class="table-footer" id="psTableFooter"></div>
    </div>
  </section>

  <!-- Dividends Table -->
  <section id="dividends" style="display:none">
    <div class="section-head"><span class="section-index">ix.</span><h2 class="section-title" data-i18n="dividends"></h2><span class="section-note" id="divNote"></span></div>
    <div class="card controls" id="divControls">
      <div class="control-group">
        <label data-i18n="col.stock"></label>
        <select id="divFilterStock"><option value="" data-i18n="filter.allStocks"></option></select>
      </div>
      <div class="control-group">
        <label data-i18n="filter.from"></label>
        <input type="date" id="divFilterFrom">
      </div>
      <div class="control-group">
        <label data-i18n="filter.to"></label>
        <input type="date" id="divFilterTo">
      </div>
      <div class="control-group">
        <label data-i18n="filter.minAmount"></label>
        <input type="number" id="divFilterMin" data-i18n-placeholder="filter.min">
      </div>
      <div class="control-group">
        <label data-i18n="filter.maxAmount"></label>
        <input type="number" id="divFilterMax" data-i18n-placeholder="filter.max">
      </div>
      <div class="control-group">
        <label>&nbsp;</label>
        <button class="btn-outline btn" onclick="resetDivFilters()" data-i18n="filter.reset"></button>
      </div>
    </div>

    <div class="card table-wrapper">
      <div class="table-scroll">
        <table>
          <thead><tr id="divTableHead"></tr></thead>
          <tbody id="divTableBody"></tbody>
        </table>
      </div>
      <div class="table-footer" id="divTableFooter"></div>
    </div>
  </section>

  <footer class="page-footer">
    <span class="mark" data-i18n="appName"></span> · <span data-i18n="figuresInEgp"></span>
  </footer>
</div>

<!-- Scroll to Top -->
<a href="#top" class="scroll-top" id="scrollTop" data-i18n-title="scrollTop">&#8593;</a>

<script src="https://cdn.jsdelivr.net/npm/chart.js@4"></script>
<script>
const ALL_TRADES = {{tradesJson}};
const ALL_PARTIAL_SELLS = {{partialSellsJson}};
const NON_COMPLIANT = {{nonCompliantJson}};

const ALL_DIVIDENDS = {{dividendsJson}};
const NON_COMPLIANT_DIVIDENDS = {{nonCompliantDividendsJson}};

// Attach non-compliant earnings (non_compliant_earnings.json / non_compliant_dividends.json) to each partial sell
// and dividend by symbol, date and earning.
{
  const ncKey = (symbol, date, earning) => `${symbol}|${date}|${earning}`;
  const attach = (report, rows, key) => {
    const byKey = new Map(report.map(n => [ncKey(n.symbol, n.date, n.total_earning), n]));
    rows.forEach(r => { r.non_compliant = byKey.get(key(r)) ?? null; });
  };
  attach(NON_COMPLIANT, ALL_PARTIAL_SELLS, t => ncKey(t.symbol_code, t.close_date, t.net_pnl));
  attach(NON_COMPLIANT_DIVIDENDS, ALL_DIVIDENDS, d => ncKey(d.symbol_code, d.date, d.amount));
}
const METRICS = {{metricsJson}};
const HAS_TRADES = ALL_TRADES.length > 0;

let sortCol = 'close_date';
let sortDir = -1;
let psSortCol = 'close_date';
let psSortDir = -1;
let divSortCol = 'date';
let divSortDir = -1;
let charts = {};

// ── Localization ────────────────────────────────────────
// Arabic (RTL, the default) and English. Stock symbols and the purification report's blocked reasons / notes stay
// in English. Numbers keep Latin digits in both languages.
const I18N = {
  en: {
    appName: 'Borsa Returns', eyebrow: 'Egyptian Exchange · Net & purified returns',
    langSwitch: 'ع', langSwitchTitle: 'التبديل إلى العربية', themeToggle: 'Toggle light / dark',
    updated: 'Updated {date}', figuresInEgp: 'figures in EGP', scrollTop: 'Scroll to top', sparkline: 'Cumulative P&L sparkline',
    'nav.overview': 'Overview', summary: 'Summary', pnlByPeriod: 'P&L by Period', psByPeriod: 'Partial Sells by Period',
    charts: 'Charts', trades: 'Trades', partialSells: 'Partial Sells', dividends: 'Dividends',
    tradesCalendar: 'Trades Calendar', psCalendar: 'Partial Sells Calendar', calendarNote: "Shade deepens with the size of the month's result",
    'chart.cum': 'Cumulative P&L', 'chart.cumSub': 'Running total by close date',
    'chart.stock': 'P&L by Stock', 'chart.stockSub': 'Net result per symbol, best to worst',
    'chart.monthly': 'Monthly P&L', 'chart.monthlySub': 'Net result per calendar month',
    'chart.winRate': 'Win Rate by Stock', 'chart.winRateSub': 'Share of winning trades per symbol',
    'filter.allStocks': 'All Stocks', 'filter.outcome': 'Outcome', 'filter.all': 'All', 'filter.winners': 'Winners', 'filter.losers': 'Losers',
    'filter.from': 'From', 'filter.to': 'To', 'filter.minPnl': 'Min P&L', 'filter.maxPnl': 'Max P&L',
    'filter.minAmount': 'Min Amount', 'filter.maxAmount': 'Max Amount', 'filter.min': 'Min', 'filter.max': 'Max', 'filter.reset': 'Reset',
    'col.stock': 'Stock', 'col.openDate': 'Open Date', 'col.closeDate': 'Close Date', 'col.volume': 'Volume',
    'col.entryPrice': 'Entry Price', 'col.closePrice': 'Close Price', 'col.pnl': 'P&L', 'col.netPnl': 'Net P&L', 'col.pnlPct': 'P&L %',
    'col.duration': 'Duration', 'col.commission': 'Commission', 'col.exitDate': 'Exit Date', 'col.exitPrice': 'Exit Price',
    'col.pl': 'P/L', 'col.plPct': 'P/L %', 'col.date': 'Date', 'col.amount': 'Amount',
    'col.ncPct': 'Non-compliant Earning %', 'col.nc': 'Non-compliant Earning', 'col.netTotal': 'Net Total',
    durationShort: '{n}d',
    heroTotal: 'Total returns', heroPs: 'Partial sells P&L', pnl: 'P&L', dividend: 'Dividends', commission: 'Commission',
    since: '{count} since {date}',
    winRate: 'Win rate', winLoss: '{w}W · {l}L', winLossSlash: '{w}W / {l}L', winnersPct: '{v} winners',
    topStock: 'Top stock', bestTrade: 'Best trade', bestSell: 'Best sell',
    purification: 'Purification', purifyCaption: 'Partial sells, quarterly screening', ncEarnings: 'Non-compliant earnings',
    ofScreened: '{v} of screened gains', nothingToPurify: 'Nothing to purify', netClean: 'Net total (clean)',
    afterPurification: 'after purification', coverage: 'Screening coverage', blocked: '{count} blocked — missing screening data',
    allScreened: 'Every sell screened', viaFallback: '{n} via fallback',
    'kpi.totalPnl': 'Total P&L', 'kpi.dividends': 'Dividends', 'kpi.commission': 'Total Commission', 'kpi.netPnl': 'Total Net P&L',
    netFormula: 'P&L + Dividends − Commission', 'kpi.winRate': 'Win Rate', 'kpi.avgPnl': 'Avg P&L per Trade',
    'kpi.best': 'Best Trade', 'kpi.worst': 'Worst Trade', 'kpi.profitFactor': 'Profit Factor', 'kpi.avgProfit': 'Avg Profit',
    'kpi.avgLoss': 'Avg Loss', 'kpi.avgHolding': 'Avg Holding Period',
    noTrades: 'No trades match these filters', noPs: 'No partial sells match these filters', noDiv: 'No dividends match these filters',
    showingTrades: 'Showing {n} of {total} trades', showingPs: 'Showing {n} of {total} partial sells', showingDiv: 'Showing {n} of {total} dividends',
    filteredPnl: 'Filtered P&L:', filteredTotal: 'Filtered total:', stocks: 'Stocks:',
    ncShort: 'Non-compliant:', netTotal: 'Net Total:', computedOf: '({c} of {n} computed)', noNcData: 'No non-compliant earnings data',
    ncNoteFallback: 'Hover a — for why a {noun} was not screened, or a {mark}value computed from a fallback',
    ncNote: 'Hover a — to see why a {noun} was not screened', 'noun.sell': 'sell', 'noun.dividend': 'dividend',
  },
  ar: {
    appName: 'عوائد البورصة', eyebrow: 'البورصة المصرية · عوائد صافية ومطهّرة',
    langSwitch: 'EN', langSwitchTitle: 'Switch to English', themeToggle: 'تبديل الوضع الفاتح / الداكن',
    updated: 'آخر تحديث {date}', figuresInEgp: 'المبالغ بالجنيه المصري', scrollTop: 'العودة إلى الأعلى', sparkline: 'منحنى الأرباح والخسائر التراكمية',
    'nav.overview': 'نظرة عامة', summary: 'الملخص', pnlByPeriod: 'الأرباح والخسائر حسب الفترة', psByPeriod: 'البيع الجزئي حسب الفترة',
    charts: 'الرسوم البيانية', trades: 'الصفقات', partialSells: 'البيع الجزئي', dividends: 'التوزيعات',
    tradesCalendar: 'تقويم الصفقات', psCalendar: 'تقويم البيع الجزئي', calendarNote: 'يزداد عمق اللون بحجم نتيجة الشهر',
    'chart.cum': 'الأرباح والخسائر التراكمية', 'chart.cumSub': 'الإجمالي المتراكم حسب تاريخ الإغلاق',
    'chart.stock': 'الأرباح والخسائر حسب السهم', 'chart.stockSub': 'صافي النتيجة لكل سهم، من الأفضل إلى الأسوأ',
    'chart.monthly': 'الأرباح والخسائر الشهرية', 'chart.monthlySub': 'صافي النتيجة لكل شهر',
    'chart.winRate': 'نسبة الربح حسب السهم', 'chart.winRateSub': 'نسبة الصفقات الرابحة لكل سهم',
    'filter.allStocks': 'كل الأسهم', 'filter.outcome': 'النتيجة', 'filter.all': 'الكل', 'filter.winners': 'الرابحة', 'filter.losers': 'الخاسرة',
    'filter.from': 'من', 'filter.to': 'إلى', 'filter.minPnl': 'أدنى ربح/خسارة', 'filter.maxPnl': 'أقصى ربح/خسارة',
    'filter.minAmount': 'أدنى مبلغ', 'filter.maxAmount': 'أقصى مبلغ', 'filter.min': 'الأدنى', 'filter.max': 'الأقصى', 'filter.reset': 'إعادة تعيين',
    'col.stock': 'السهم', 'col.openDate': 'تاريخ الفتح', 'col.closeDate': 'تاريخ الإغلاق', 'col.volume': 'الكمية',
    'col.entryPrice': 'سعر الدخول', 'col.closePrice': 'سعر الإغلاق', 'col.pnl': 'الربح/الخسارة', 'col.netPnl': 'صافي الربح/الخسارة', 'col.pnlPct': 'الربح/الخسارة %',
    'col.duration': 'المدة', 'col.commission': 'العمولة', 'col.exitDate': 'تاريخ الخروج', 'col.exitPrice': 'سعر الخروج',
    'col.pl': 'الربح/الخسارة', 'col.plPct': 'الربح/الخسارة %', 'col.date': 'التاريخ', 'col.amount': 'المبلغ',
    'col.ncPct': 'نسبة الأرباح غير المتوافقة', 'col.nc': 'الأرباح غير المتوافقة', 'col.netTotal': 'الإجمالي الصافي',
    durationShort: '{n} يوم',
    heroTotal: 'إجمالي العوائد', heroPs: 'أرباح وخسائر البيع الجزئي', pnl: 'الربح/الخسارة', dividend: 'التوزيعات', commission: 'العمولة',
    since: '{count} منذ {date}',
    winRate: 'نسبة الربح', winLoss: '{w} رابحة · {l} خاسرة', winLossSlash: '{w} رابحة / {l} خاسرة', winnersPct: '{v} رابحة',
    topStock: 'السهم الأفضل', bestTrade: 'أفضل صفقة', bestSell: 'أفضل عملية بيع',
    purification: 'التطهير', purifyCaption: 'البيع الجزئي، فحص ربع سنوي', ncEarnings: 'الأرباح غير المتوافقة',
    ofScreened: '{v} من الأرباح المفحوصة', nothingToPurify: 'لا يوجد ما يلزم تطهيره', netClean: 'الإجمالي الصافي (بعد التطهير)',
    afterPurification: 'بعد التطهير', coverage: 'تغطية الفحص', blocked: '{count} بلا فحص — بيانات الفحص غير متوفرة',
    allScreened: 'تم فحص كل عمليات البيع', viaFallback: '{n} بقيمة احتياطية',
    'kpi.totalPnl': 'إجمالي الربح/الخسارة', 'kpi.dividends': 'التوزيعات', 'kpi.commission': 'إجمالي العمولة', 'kpi.netPnl': 'صافي الربح/الخسارة الإجمالي',
    netFormula: 'الربح/الخسارة + التوزيعات − العمولة', 'kpi.winRate': 'نسبة الربح', 'kpi.avgPnl': 'متوسط الربح/الخسارة للصفقة',
    'kpi.best': 'أفضل صفقة', 'kpi.worst': 'أسوأ صفقة', 'kpi.profitFactor': 'معامل الربح', 'kpi.avgProfit': 'متوسط الربح',
    'kpi.avgLoss': 'متوسط الخسارة', 'kpi.avgHolding': 'متوسط مدة الاحتفاظ',
    noTrades: 'لا توجد صفقات مطابقة لعوامل التصفية', noPs: 'لا توجد عمليات بيع جزئي مطابقة لعوامل التصفية', noDiv: 'لا توجد توزيعات مطابقة لعوامل التصفية',
    showingTrades: 'عرض {n} من {total}', showingPs: 'عرض {n} من {total}', showingDiv: 'عرض {n} من {total}',
    filteredPnl: 'إجمالي المعروض:', filteredTotal: 'إجمالي المعروض:', stocks: 'الأسهم:',
    ncShort: 'غير المتوافق:', netTotal: 'الإجمالي الصافي:', computedOf: '(تم حساب {c} من {n})', noNcData: 'لا توجد بيانات عن الأرباح غير المتوافقة',
    ncNoteFallback: 'مرّر المؤشر على — لمعرفة سبب عدم فحص {noun}، أو على {mark}قيمة محسوبة احتياطيًا',
    ncNote: 'مرّر المؤشر على — لمعرفة سبب عدم فحص {noun}', 'noun.sell': 'عملية البيع', 'noun.dividend': 'التوزيع',
  },
};

// Count phrases per plural category (Intl.PluralRules); {n} is the formatted count.
const PLURALS = {
  en: {
    trade: { one: '{n} trade', other: '{n} trades' },
    payout: { one: '{n} payout', other: '{n} payouts' },
    partialSell: { one: '{n} partial sell', other: '{n} partial sells' },
    sell: { one: '{n} sell', other: '{n} sells' },
    winningTrade: { one: '{n} winning trade', other: '{n} winning trades' },
    losingTrade: { one: '{n} losing trade', other: '{n} losing trades' },
    day: { one: '{n} day', other: '{n} days' },
  },
  ar: {
    trade: { one: 'صفقة واحدة', two: 'صفقتان', few: '{n} صفقات', other: '{n} صفقة' },
    payout: { one: 'توزيع واحد', two: 'توزيعان', few: '{n} توزيعات', other: '{n} توزيع' },
    partialSell: { one: 'عملية بيع جزئي واحدة', two: 'عمليتا بيع جزئي', few: '{n} عمليات بيع جزئي', other: '{n} عملية بيع جزئي' },
    sell: { one: 'عملية بيع واحدة', two: 'عمليتا بيع', few: '{n} عمليات بيع', other: '{n} عملية بيع' },
    winningTrade: { one: 'صفقة رابحة واحدة', two: 'صفقتان رابحتان', few: '{n} صفقات رابحة', other: '{n} صفقة رابحة' },
    losingTrade: { one: 'صفقة خاسرة واحدة', two: 'صفقتان خاسرتان', few: '{n} صفقات خاسرة', other: '{n} صفقة خاسرة' },
    day: { one: 'يوم واحد', two: 'يومان', few: '{n} أيام', other: '{n} يومًا' },
  },
};

// Period labels come from MetricsCalculator in English.
const AR_PERIODS = { 'This Month': 'هذا الشهر', 'Past 3 Months': 'آخر 3 أشهر', 'Past 6 Months': 'آخر 6 أشهر', 'Past 1 Year': 'آخر سنة', 'Past 2 Years': 'آخر سنتين', 'All Time': 'منذ البداية' };
// Section indices: Roman numerals in English, the abjad letter order (أ ب ج …) in Arabic.
const SECTION_INDEX = { en: ['i', 'ii', 'iii', 'iv', 'v', 'vi', 'vii', 'viii', 'ix'], ar: ['أ', 'ب', 'ج', 'د', 'هـ', 'و', 'ز', 'ح', 'ط'] };

let LANG = document.documentElement.lang === 'en' ? 'en' : 'ar';
const isRtl = () => LANG === 'ar';
const locale = () => LANG === 'ar' ? 'ar-EG-u-nu-latn' : undefined;

function tl(key, vars) {
  let s = I18N[LANG][key] ?? I18N.en[key] ?? key;
  if (vars) for (const [k, v] of Object.entries(vars)) s = s.replaceAll('{' + k + '}', v);
  return s;
}
function plural(n, key) {
  const forms = PLURALS[LANG][key];
  const form = forms[new Intl.PluralRules(LANG).select(n)] ?? forms.other;
  return form.replace('{n}', n.toLocaleString(locale()));
}
const periodLabel = label => LANG === 'ar' ? (AR_PERIODS[label] ?? label) : label;

// Wrap numbers in a left-to-right isolate so signs and % stay attached on the correct side inside Arabic text.
const ltr = s => '⁦' + s + '⁩';

const signCls = v => v >= 0 ? 'positive' : 'negative';
const ticker = v => `<span class="ticker">${v}</span>`;
const fmtDate = v => new Date(v).toLocaleDateString(locale(), { day: '2-digit', month: 'short', year: 'numeric' });
const fmtPct = v => ltr(v.toFixed(2) + '%');

const COLUMNS = [
  { key: 'reuters_code', label: 'col.stock', fmt: ticker },
  { key: 'open_date', label: 'col.openDate', fmt: fmtDate },
  { key: 'close_date', label: 'col.closeDate', fmt: fmtDate },
  { key: 'volume', label: 'col.volume', fmt: v => v.toLocaleString(locale()), num: true },
  { key: 'avg_entry_price', label: 'col.entryPrice', fmt: v => v.toFixed(2), num: true },
  { key: 'close_price', label: 'col.closePrice', fmt: v => v.toFixed(2), num: true },
  { key: 'gross_pnl', label: 'col.pnl', fmt: v => fmtCurrency(v), cls: signCls, computed: t => t.net_pnl + t.commission, num: true },
  { key: 'net_pnl', label: 'col.netPnl', fmt: v => fmtCurrency(v), cls: signCls, num: true },
  { key: 'net_pnl_percentage', label: 'col.pnlPct', fmt: fmtPct, cls: signCls, num: true },
  { key: 'duration_days', label: 'col.duration', fmt: v => tl('durationShort', { n: v }), num: true },
  { key: 'commission', label: 'col.commission', fmt: fmtCost, cls: v => v === 0 ? 'muted' : 'negative', num: true },
];

function amount(v) { return v.toLocaleString(locale(), { minimumFractionDigits: 2, maximumFractionDigits: 2 }); }
function fmtAmount(v) { return ltr(amount(v)); }
function fmtCurrency(v) { return ltr((v >= 0 ? '+' : '') + amount(v)); }
// Costs (commission) are always outflows: show as a negative amount, or a plain 0.00 when nothing was paid.
function fmtCost(v) { return ltr(v === 0 ? amount(0) : '−' + amount(Math.abs(v))); }
function fmtCompact(v) { return v.toLocaleString(locale(), { notation: 'compact', maximumFractionDigits: 1 }); }
function cssVar(name) { return getComputedStyle(document.documentElement).getPropertyValue(name).trim(); }
function rgba(hex, a) {
  const h = hex.replace('#', '');
  const n = parseInt(h.length === 3 ? h.split('').map(c => c + c).join('') : h, 16);
  return `rgba(${(n >> 16) & 255},${(n >> 8) & 255},${n & 255},${a})`;
}

function getFiltered() {
  let trades = [...ALL_TRADES];
  const stock = document.getElementById('filterStock').value;
  const outcome = document.getElementById('filterOutcome').value;
  const from = document.getElementById('filterFrom').value;
  const to = document.getElementById('filterTo').value;
  const minPnl = document.getElementById('filterMinPnl').value;
  const maxPnl = document.getElementById('filterMaxPnl').value;

  if (stock) trades = trades.filter(t => t.reuters_code === stock);
  if (outcome === 'win') trades = trades.filter(t => t.net_pnl >= 0);
  if (outcome === 'loss') trades = trades.filter(t => t.net_pnl < 0);
  if (from) trades = trades.filter(t => t.close_date >= from);
  if (to) trades = trades.filter(t => t.close_date <= to + 'T23:59:59Z');
  if (minPnl !== '') trades = trades.filter(t => t.net_pnl >= +minPnl);
  if (maxPnl !== '') trades = trades.filter(t => t.net_pnl <= +maxPnl);

  const sortDef = COLUMNS.find(c => c.key === sortCol);
  trades.sort((a, b) => {
    let va = sortDef?.computed ? sortDef.computed(a) : a[sortCol];
    let vb = sortDef?.computed ? sortDef.computed(b) : b[sortCol];
    if (typeof va === 'string') return va.localeCompare(vb) * sortDir;
    return (va - vb) * sortDir;
  });
  return trades;
}

// ── Overview ────────────────────────────────────────────
function countUp(el, target, render) {
  if (!document.documentElement.classList.contains('anim')) { el.textContent = render(target); return; }
  const start = performance.now(), dur = 1100;
  const step = now => {
    const p = Math.min(1, (now - start) / dur);
    el.textContent = render(target * (1 - Math.pow(1 - p, 3)));
    if (p < 1) requestAnimationFrame(step);
  };
  requestAnimationFrame(step);
}

function renderOverview() {
  const records = HAS_TRADES ? ALL_TRADES : ALL_PARTIAL_SELLS;
  const d = METRICS.kpis;
  const heroValue = HAS_TRADES ? METRICS.totalReturns : records.reduce((s, t) => s + t.net_pnl, 0);

  document.getElementById('heroLabel').textContent = tl(HAS_TRADES ? 'heroTotal' : 'heroPs');
  const valueEl = document.getElementById('heroValue');
  valueEl.className = 'hero-value num ' + signCls(heroValue);
  countUp(valueEl, heroValue, fmtCurrency);

  const dates = records.map(t => t.close_date).sort();
  const since = dates.length ? fmtDate(dates[0]) : '—';
  document.getElementById('heroSub').innerHTML = HAS_TRADES
    ? `${tl('pnl')} <b class="num">${fmtCurrency(d.totalGrossPnl)}</b> + ${tl('dividend')} <b class="num" title="${plural(d.dividendCount, 'payout')}">${fmtAmount(d.dividends)}</b> − ${tl('commission')} <b class="num">${fmtAmount(Math.abs(d.totalCommission))}</b>`
    : tl('since', { count: plural(records.length, 'partialSell'), date: `<b>${since}</b>` }) +
      (d.dividendCount ? ` · ${tl('dividend')} <b class="num">${fmtAmount(d.dividends)}</b>` : '');

  // Side stats
  const wins = records.filter(t => t.net_pnl >= 0).length;
  const winRate = records.length ? wins / records.length * 100 : 0;
  const byStock = {};
  records.forEach(t => { byStock[t.reuters_code] = (byStock[t.reuters_code] || 0) + t.net_pnl; });
  const top = Object.entries(byStock).sort((a, b) => b[1] - a[1])[0];
  const best = records.reduce((m, t) => (!m || t.net_pnl > m.net_pnl) ? t : m, null);
  const unit = HAS_TRADES ? 'trade' : 'sell';
  const winPct = ltr(winRate.toFixed(1) + '%');

  document.getElementById('heroSide').innerHTML = `
    <div class="card stat reveal">
      <div class="stat-label"><span>${tl('winRate')}</span><span class="num">${tl('winLoss', { w: wins, l: records.length - wins })}</span></div>
      <div class="stat-value num">${winPct}</div>
      <div class="meter" title="${tl('winnersPct', { v: winPct })}"><span id="winMeter" style="width:0"></span></div>
    </div>
    <div class="card stat reveal">
      <div class="stat-label"><span>${tl('topStock')}</span><span>${top ? plural(records.filter(t => t.reuters_code === top[0]).length, unit) : ''}</span></div>
      <div class="stat-value num">${top ? `${ticker(top[0])} <span class="${signCls(top[1])}">${fmtCurrency(top[1])}</span>` : '—'}</div>
    </div>
    <div class="card stat reveal">
      <div class="stat-label"><span>${tl(HAS_TRADES ? 'bestTrade' : 'bestSell')}</span><span>${best ? fmtDate(best.close_date) : ''}</span></div>
      <div class="stat-value num">${best ? `${ticker(best.reuters_code)} <span class="${signCls(best.net_pnl)}">${fmtCurrency(best.net_pnl)}</span>` : '—'}</div>
    </div>`;
  requestAnimationFrame(() => requestAnimationFrame(() => { document.getElementById('winMeter').style.width = winRate + '%'; }));

  renderPurification();
}

function renderPurification() {
  if (NON_COMPLIANT.length === 0) return;
  const computed = NON_COMPLIANT.filter(n => !n.blocked);
  const haram = computed.reduce((s, n) => s + n.haram, 0);
  const clean = computed.reduce((s, n) => s + n.clean_total, 0);
  const earning = computed.reduce((s, n) => s + n.total_earning, 0);
  const coverage = computed.length / NON_COMPLIANT.length * 100;
  const fallbacks = computed.filter(n => n.fallback_used).length;
  const el = document.getElementById('purify');
  el.style.display = '';
  el.innerHTML = `
    <div class="purify-badge">
      <div class="purify-icon"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M12 2.7C9 6.5 6 9.8 6 13.5a6 6 0 0 0 12 0c0-3.7-3-7-6-10.8z"/></svg></div>
      <div><div class="purify-title">${tl('purification')}</div><div class="purify-caption">${tl('purifyCaption')}</div></div>
    </div>
    <div>
      <div class="stat-label">${tl('ncEarnings')}</div>
      <div class="stat-value num">${fmtAmount(haram)}</div>
      <div class="purify-caption">${earning > 0 ? tl('ofScreened', { v: fmtPct(haram / earning * 100) }) : tl('nothingToPurify')}</div>
    </div>
    <div>
      <div class="stat-label">${tl('netClean')}</div>
      <div class="stat-value num ${signCls(clean)}">${fmtCurrency(clean)}</div>
      <div class="purify-caption">${tl('afterPurification')}</div>
    </div>
    <div>
      <div class="stat-label"><span>${tl('coverage')}</span><span class="num">${ltr(computed.length + ' / ' + NON_COMPLIANT.length)}</span></div>
      <div class="meter" style="background:var(--surface3)"><span style="width:${coverage}%;background:var(--accent)"></span></div>
      <div class="purify-caption" style="margin-top:6px">${NON_COMPLIANT.length - computed.length ? tl('blocked', { count: plural(NON_COMPLIANT.length - computed.length, 'sell') }) : tl('allScreened')}${fallbacks ? ` · <span class="fallback-mark"></span>${tl('viaFallback', { n: fallbacks })}` : ''}</div>
    </div>`;
}

function renderSparkline() {
  let values;
  if (HAS_TRADES) values = METRICS.charts.cumulativePnl.values;
  else {
    let cum = 0;
    values = [...ALL_PARTIAL_SELLS].sort((a, b) => a.close_date.localeCompare(b.close_date)).map(t => (cum += t.net_pnl));
  }
  if (values.length < 2) { document.querySelector('.hero-spark').style.display = 'none'; return; }
  const color = cssVar('--accent');
  if (charts.heroSpark) charts.heroSpark.destroy();
  charts.heroSpark = new Chart(document.getElementById('heroSpark'), {
    type: 'line',
    data: { labels: values.map((_, i) => i), datasets: [{ data: values, borderColor: color, borderWidth: 2, pointRadius: 0, tension: 0.35, fill: true, backgroundColor: areaGradient(color) }] },
    options: { responsive: true, maintainAspectRatio: false, animation: { duration: 900 }, plugins: { legend: { display: false }, tooltip: { enabled: false } }, scales: { x: { display: false, reverse: isRtl() }, y: { display: false } } }
  });
}

// ── Summary & periods ───────────────────────────────────
function renderKpis() {
  const d = METRICS.kpis;
  const kpis = [
    { label: 'kpi.totalPnl', value: fmtCurrency(d.totalGrossPnl), cls: signCls(d.totalGrossPnl), sub: plural(d.tradeCount, 'trade') },
    { label: 'kpi.dividends', value: fmtCurrency(d.dividends), cls: signCls(d.dividends), sub: plural(d.dividendCount, 'payout') },
    { label: 'kpi.commission', value: fmtCost(d.totalCommission), cls: d.totalCommission === 0 ? '' : 'negative' },
    { label: 'kpi.netPnl', value: fmtCurrency(d.totalNetPnl), cls: signCls(d.totalNetPnl), sub: tl('netFormula') },
    { label: 'kpi.winRate', value: ltr(d.winRate.toFixed(1) + '%'), cls: d.winRate >= 50 ? 'positive' : 'negative', sub: tl('winLossSlash', { w: d.winCount, l: d.lossCount }) },
    { label: 'kpi.avgPnl', value: fmtCurrency(d.avgPnlPerTrade), cls: signCls(d.avgPnlPerTrade) },
    { label: 'kpi.best', value: fmtCurrency(d.bestTrade), cls: 'positive' },
    { label: 'kpi.worst', value: fmtCurrency(d.worstTrade), cls: 'negative' },
    { label: 'kpi.profitFactor', value: d.profitFactorInfinite ? '∞' : d.profitFactor.toFixed(2), cls: (d.profitFactorInfinite || d.profitFactor >= 1) ? 'positive' : 'negative' },
    { label: 'kpi.avgProfit', value: fmtCurrency(d.avgProfit), cls: 'positive', sub: plural(d.winCount, 'winningTrade') },
    { label: 'kpi.avgLoss', value: fmtCurrency(d.avgLoss), cls: 'negative', sub: plural(d.lossCount, 'losingTrade') },
    { label: 'kpi.avgHolding', value: plural(Math.round(d.avgDuration), 'day') },
  ];

  document.getElementById('kpiGrid').innerHTML = kpis.map(k =>
    `<div class="card kpi reveal"><div class="kpi-label">${tl(k.label)}</div><div class="kpi-value num ${k.cls || ''}">${k.value}</div>${k.sub ? `<div class="kpi-sub">${k.sub}</div>` : ''}</div>`
  ).join('');
}

function renderPeriodCards(periods, gridId, summaryId) {
  const regular = periods.slice(0, -1);
  const allTime = periods[periods.length - 1];
  const bar = v => v === 0 ? 'var(--border)' : (v > 0 ? 'var(--pos)' : 'var(--neg)');

  document.getElementById(gridId).innerHTML = regular.map(p =>
    `<div class="card period-card reveal" style="--bar:${bar(p.pnl)}">
      <div class="period-label">${periodLabel(p.label)}</div>
      <div class="period-value num ${signCls(p.pnl)}">${fmtCurrency(p.pnl)}</div>
      <div class="period-trades">${plural(p.tradeCount, 'trade')}</div>
    </div>`
  ).join('');

  const d = METRICS.kpis;
  const totalNetPnl = allTime.pnl + d.dividends - d.totalCommission;

  document.getElementById(summaryId).innerHTML = `
    <div class="card period-card reveal" style="--bar:${bar(allTime.pnl)}">
      <div class="period-label">${periodLabel(allTime.label)}</div>
      <div class="period-value num ${signCls(allTime.pnl)}">${fmtCurrency(allTime.pnl)}</div>
      <div class="period-trades">${plural(allTime.tradeCount, 'trade')}</div>
    </div>
    <div class="card period-card featured reveal" style="--bar:var(--accent)">
      <div class="period-label">${tl('kpi.netPnl')}</div>
      <div class="period-value num ${signCls(totalNetPnl)}">${fmtCurrency(totalNetPnl)}</div>
      <div class="period-trades">${tl('netFormula')}</div>
    </div>`;
}

function renderPeriods() {
  renderPeriodCards(METRICS.periods, 'periodGrid', 'periodSummary');
}

function renderCalendar(containerId, entries, sectionId) {
  const container = document.getElementById(containerId);
  if (!entries || entries.length === 0) {
    if (sectionId) document.getElementById(sectionId).style.display = 'none';
    return;
  }
  if (sectionId) document.getElementById(sectionId).style.display = '';

  const map = {};
  entries.forEach(e => { map[e.month] = e; });
  const maxAbs = Math.max(...entries.map(e => Math.abs(e.pnl)), 1);

  const years = [...new Set(entries.map(e => parseInt(e.month.split('-')[0])))].sort();
  const monthFmt = new Intl.DateTimeFormat(locale(), { month: 'short', timeZone: 'UTC' });
  const monthNames = Array.from({ length: 12 }, (_, m) => monthFmt.format(Date.UTC(2000, m, 1)));

  let html = '';
  for (const year of years) {
    const yearTotal = entries.filter(e => e.month.startsWith(year + '-')).reduce((s, e) => s + e.pnl, 0);
    html += `<div class="card calendar-year reveal">`;
    html += `<div class="calendar-year-label"><span>${year}</span><span class="calendar-year-total num ${signCls(yearTotal)}">${fmtCurrency(yearTotal)}</span></div>`;
    html += `<div class="calendar-grid">`;
    for (let m = 0; m < 12; m++) {
      const key = `${year}-${String(m + 1).padStart(2, '0')}`;
      const data = map[key];
      if (data) {
        const cls = data.pnl >= 0 ? 'profitable' : 'losing';
        const intensity = Math.round(12 + 30 * Math.sqrt(Math.abs(data.pnl) / maxAbs));
        html += `<div class="calendar-cell ${cls}" style="--i:${intensity}" title="${monthNames[m]} ${year}: ${fmtCurrency(data.pnl)} · ${plural(data.tradeCount, 'trade')}">
          <div class="calendar-month">${monthNames[m]}</div>
          <div class="calendar-pnl ${signCls(data.pnl)}">${ltr(fmtCompact(data.pnl))}</div>
          <div class="calendar-trades">${plural(data.tradeCount, 'trade')}</div>
        </div>`;
      } else {
        html += `<div class="calendar-cell empty">
          <div class="calendar-month">${monthNames[m]}</div>
          <div class="calendar-pnl muted">—</div>
        </div>`;
      }
    }
    html += `</div></div>`;
  }
  container.innerHTML = html;
}

function renderPartialSells() {
  if (!METRICS.partialSellsPeriods || METRICS.partialSellsPeriods.length === 0) return;
  document.getElementById('partial-sells').style.display = '';
  document.getElementById('navPartialSells').style.display = '';
  renderPeriodCards(METRICS.partialSellsPeriods, 'partialSellsGrid', 'partialSellsSummary');
}

// ── Tables ──────────────────────────────────────────────
function headerHtml(columns, activeCol, dir, setter) {
  return columns.map(c =>
    `<th class="${c.num ? 'n' : ''} ${activeCol === c.key ? 'sorted' : ''}" onclick="${setter}('${c.key}')">${tl(c.label)}<span class="sort-icon">${activeCol === c.key ? (dir === 1 ? '▲' : '▼') : '⬍'}</span></th>`
  ).join('');
}

function renderTable(trades) {
  document.getElementById('tableHead').innerHTML = headerHtml(COLUMNS, sortCol, sortDir, 'setSort');

  document.getElementById('tableBody').innerHTML = trades.length === 0
    ? `<tr class="empty-row"><td colspan="${COLUMNS.length}">${tl('noTrades')}</td></tr>`
    : trades.map(t =>
      '<tr>' + COLUMNS.map(c => {
        const v = c.computed ? c.computed(t) : t[c.key];
        const cls = (c.cls ? c.cls(v) : '') + (c.num ? ' n' : '');
        const display = c.fmt ? c.fmt(v) : v;
        return `<td class="${cls}">${display}</td>`;
      }).join('') + '</tr>'
    ).join('');

  const totalPnl = trades.reduce((s, t) => s + t.net_pnl, 0);
  document.getElementById('tableFooter').innerHTML = `
    <span>${tl('showingTrades', { n: trades.length, total: ALL_TRADES.length })}</span>
    <span>${tl('filteredPnl')} <strong class="${signCls(totalPnl)}">${fmtCurrency(totalPnl)}</strong></span>`;
}

// ── Charts ──────────────────────────────────────────────
function areaGradient(color) {
  return ctx => {
    const { chart } = ctx;
    const area = chart.chartArea;
    if (!area) return null;
    const g = chart.ctx.createLinearGradient(0, area.top, 0, area.bottom);
    g.addColorStop(0, rgba(color, 0.22));
    g.addColorStop(1, rgba(color, 0));
    return g;
  };
}

function renderCharts() {
  const c = METRICS.charts;
  const accent = cssVar('--accent'), pos = cssVar('--pos'), neg = cssVar('--neg');
  const signColors = values => values.map(v => v >= 0 ? rgba(pos, 0.85) : rgba(neg, 0.85));

  updateChart('pnlChart', 'line', c.cumulativePnl.labels, c.cumulativePnl.values, {
    borderColor: accent, backgroundColor: areaGradient(accent), fill: true, tension: 0.3,
    pointBackgroundColor: accent, pointBorderColor: cssVar('--surface'), pointBorderWidth: 2, pointHoverRadius: 5,
  }, { money: true, dateLabels: true });

  updateChart('stockChart', 'bar', c.pnlByStock.labels, c.pnlByStock.values, { backgroundColor: signColors(c.pnlByStock.values) }, { money: true });
  updateChart('monthlyChart', 'bar', c.monthlyPnl.labels, c.monthlyPnl.values, { backgroundColor: signColors(c.monthlyPnl.values) }, { money: true });
  updateChart('winRateChart', 'bar', c.winRateByStock.labels, c.winRateByStock.values, { backgroundColor: rgba(accent, 0.85) },
    { suggestedMax: 100, callback: v => v + '%', tooltip: v => tl('winnersPct', { v: ltr(v.toFixed(1) + '%') }) });
}

function updateChart(id, type, labels, data, dsOpts, yOpts) {
  if (charts[id]) charts[id].destroy();
  const text2 = cssVar('--text3'), grid = cssVar('--border');
  const isLine = type === 'line';
  const tooltipFmt = yOpts?.tooltip ?? (yOpts?.money ? fmtCurrency : v => v);

  charts[id] = new Chart(document.getElementById(id), {
    type,
    data: { labels, datasets: [{
      data, ...dsOpts,
      borderWidth: isLine ? 2 : 0,
      pointRadius: isLine ? (data.length > 40 ? 0 : 3) : 0,
      ...(isLine ? {} : { borderRadius: 4, borderSkipped: 'start', maxBarThickness: 24 }),
    }] },
    options: {
      responsive: true, maintainAspectRatio: false,
      // Room for the outermost x label on the side without the y-axis, so it isn't clipped.
      layout: { padding: { [isRtl() ? 'left' : 'right']: isLine ? 28 : 12 } },
      interaction: { mode: 'index', intersect: false },
      plugins: {
        legend: { display: false },
        tooltip: {
          rtl: isRtl(), backgroundColor: cssVar('--surface3'), titleColor: cssVar('--text'), bodyColor: cssVar('--text2'),
          borderColor: cssVar('--border-strong'), borderWidth: 1, padding: 12, cornerRadius: 10, displayColors: false,
          titleFont: { weight: '600' },
          callbacks: {
            title: items => yOpts?.dateLabels ? fmtDate(items[0].label) : items[0].label,
            label: item => tooltipFmt(item.parsed.y),
          }
        }
      },
      scales: {
        x: { reverse: isRtl(), ticks: { color: text2, maxRotation: 0, autoSkipPadding: 16, callback: function (v) { const l = this.getLabelForValue(v); return yOpts?.dateLabels ? l.slice(0, 7) : l; } }, grid: { display: false }, border: { color: grid } },
        y: {
          position: isRtl() ? 'right' : 'left',
          ticks: { color: text2, callback: yOpts?.callback ?? (v => fmtCompact(v)) },
          grid: { color: grid }, border: { display: false },
          ...(yOpts?.suggestedMax ? { suggestedMax: yOpts.suggestedMax, beginAtZero: true } : {})
        }
      }
    }
  });
}

function setSort(col) {
  if (sortCol === col) sortDir *= -1;
  else { sortCol = col; sortDir = col === 'reuters_code' ? 1 : -1; }
  refresh();
}

function resetFilters() {
  document.getElementById('filterStock').value = '';
  document.getElementById('filterOutcome').value = '';
  document.getElementById('filterFrom').value = '';
  document.getElementById('filterTo').value = '';
  document.getElementById('filterMinPnl').value = '';
  document.getElementById('filterMaxPnl').value = '';
  refresh();
}

function refresh() {
  const trades = getFiltered();
  renderTable(trades);
}

// ── Non-compliant earnings columns (shared by partial sells and dividends) ──
const NC_COLUMNS = [
  { key: 'nc_percentage', label: 'col.ncPct', fmt: fmtPct, computed: r => r.non_compliant?.non_compliant_percentage ?? null, num: true },
  { key: 'nc_haram', label: 'col.nc', fmt: fmtAmount, computed: r => r.non_compliant?.haram ?? null, num: true },
  { key: 'nc_clean', label: 'col.netTotal', fmt: v => fmtCurrency(v), cls: signCls, computed: r => r.non_compliant?.clean_total ?? null, num: true },
];

function sortRows(rows, columns, col, dir) {
  const sortDef = columns.find(c => c.key === col);
  return rows.sort((a, b) => {
    const va = sortDef?.computed ? sortDef.computed(a) : a[col];
    const vb = sortDef?.computed ? sortDef.computed(b) : b[col];
    if (va === null || vb === null) return (va === null) - (vb === null); // blocked / missing values always last
    if (typeof va === 'string') return va.localeCompare(vb) * dir;
    return (va - vb) * dir;
  });
}

// A null value is a blocked row (— with the reason as tooltip); fallback results get a dotted underline and the note.
function cellHtml(c, row) {
  const v = c.computed ? c.computed(row) : row[c.key];
  if (v === null) {
    const reason = row.non_compliant?.blocked_reason ?? tl('noNcData');
    return `<td class="blocked${c.num ? ' n' : ''}" title="${reason}">—</td>`;
  }
  const cls = (c.cls ? c.cls(v) : '') + (c.num ? ' n' : '');
  const display = c.fmt ? c.fmt(v) : v;
  if (c.key.startsWith('nc_') && row.non_compliant?.fallback_used) {
    const mark = c.key === 'nc_percentage' ? '<span class="fallback-mark"></span>' : '';
    return `<td class="${cls} fallback" title="${row.non_compliant.note}">${mark}<span class="fallback-value">${display}</span></td>`;
  }
  return `<td class="${cls}">${display}</td>`;
}

function ncFooterHtml(rows) {
  const computed = rows.filter(r => r.non_compliant && !r.non_compliant.blocked);
  const totalHaram = computed.reduce((s, r) => s + r.non_compliant.haram, 0);
  const totalClean = computed.reduce((s, r) => s + r.non_compliant.clean_total, 0);
  return `
    <span>${tl('ncShort')} <strong>${fmtAmount(totalHaram)}</strong></span>
    <span>${tl('netTotal')} <strong class="${signCls(totalClean)}">${fmtCurrency(totalClean)}</strong> ${tl('computedOf', { c: computed.length, n: rows.length })}</span>`;
}

function ncNoteHtml(report, noun) {
  return report.some(n => n.fallback_used)
    ? tl('ncNoteFallback', { noun: tl('noun.' + noun), mark: '<span class="fallback-mark"></span>' })
    : tl('ncNote', { noun: tl('noun.' + noun) });
}

const PS_COLUMNS = [
  { key: 'reuters_code', label: 'col.stock', fmt: ticker },
  { key: 'close_date', label: 'col.exitDate', fmt: fmtDate },
  { key: 'close_price', label: 'col.exitPrice', fmt: v => v.toFixed(2), num: true },
  { key: 'volume', label: 'col.volume', fmt: v => v.toLocaleString(locale()), num: true },
  { key: 'net_pnl', label: 'col.pl', fmt: v => fmtCurrency(v), cls: signCls, num: true },
  { key: 'net_pnl_percentage', label: 'col.plPct', fmt: fmtPct, cls: signCls, num: true },
  ...(NON_COMPLIANT.length > 0 ? NC_COLUMNS : []),
];

function getPsFiltered() {
  let sells = [...ALL_PARTIAL_SELLS];
  const stock = document.getElementById('psFilterStock').value;
  const outcome = document.getElementById('psFilterOutcome').value;
  const from = document.getElementById('psFilterFrom').value;
  const to = document.getElementById('psFilterTo').value;
  const minPnl = document.getElementById('psFilterMinPnl').value;
  const maxPnl = document.getElementById('psFilterMaxPnl').value;

  if (stock) sells = sells.filter(t => t.reuters_code === stock);
  if (outcome === 'win') sells = sells.filter(t => t.net_pnl >= 0);
  if (outcome === 'loss') sells = sells.filter(t => t.net_pnl < 0);
  if (from) sells = sells.filter(t => t.close_date >= from);
  if (to) sells = sells.filter(t => t.close_date <= to + 'T23:59:59Z');
  if (minPnl !== '') sells = sells.filter(t => t.net_pnl >= +minPnl);
  if (maxPnl !== '') sells = sells.filter(t => t.net_pnl <= +maxPnl);

  return sortRows(sells, PS_COLUMNS, psSortCol, psSortDir);
}

function renderPsTable(sells) {
  document.getElementById('psTableHead').innerHTML = headerHtml(PS_COLUMNS, psSortCol, psSortDir, 'setPsSort');

  document.getElementById('psTableBody').innerHTML = sells.length === 0
    ? `<tr class="empty-row"><td colspan="${PS_COLUMNS.length}">${tl('noPs')}</td></tr>`
    : sells.map(t => '<tr>' + PS_COLUMNS.map(c => cellHtml(c, t)).join('') + '</tr>').join('');

  const totalPnl = sells.reduce((s, t) => s + t.net_pnl, 0);
  const ncFooter = NON_COMPLIANT.length === 0 ? '' : ncFooterHtml(sells);
  document.getElementById('psTableFooter').innerHTML = `
    <span>${tl('showingPs', { n: sells.length, total: ALL_PARTIAL_SELLS.length })}</span>
    <span>${tl('filteredPnl')} <strong class="${signCls(totalPnl)}">${fmtCurrency(totalPnl)}</strong></span>${ncFooter}`;
}

function setPsSort(col) {
  if (psSortCol === col) psSortDir *= -1;
  else { psSortCol = col; psSortDir = col === 'reuters_code' ? 1 : -1; }
  refreshPs();
}

function resetPsFilters() {
  document.getElementById('psFilterStock').value = '';
  document.getElementById('psFilterOutcome').value = '';
  document.getElementById('psFilterFrom').value = '';
  document.getElementById('psFilterTo').value = '';
  document.getElementById('psFilterMinPnl').value = '';
  document.getElementById('psFilterMaxPnl').value = '';
  refreshPs();
}

function refreshPs() {
  renderPsTable(getPsFiltered());
}

const DIV_COLUMNS = [
  { key: 'symbol_code', label: 'col.stock', fmt: ticker },
  { key: 'date', label: 'col.date', fmt: fmtDate },
  { key: 'amount', label: 'col.amount', fmt: v => fmtCurrency(v), cls: signCls, num: true },
  ...(NON_COMPLIANT_DIVIDENDS.length > 0 ? NC_COLUMNS : []),
];

function getDivFiltered() {
  let dividends = [...ALL_DIVIDENDS];
  const stock = document.getElementById('divFilterStock').value;
  const from = document.getElementById('divFilterFrom').value;
  const to = document.getElementById('divFilterTo').value;
  const min = document.getElementById('divFilterMin').value;
  const max = document.getElementById('divFilterMax').value;

  if (stock) dividends = dividends.filter(d => d.symbol_code === stock);
  if (from) dividends = dividends.filter(d => d.date >= from);
  if (to) dividends = dividends.filter(d => d.date <= to + 'T23:59:59Z');
  if (min !== '') dividends = dividends.filter(d => d.amount >= +min);
  if (max !== '') dividends = dividends.filter(d => d.amount <= +max);

  return sortRows(dividends, DIV_COLUMNS, divSortCol, divSortDir);
}

function renderDivTable(dividends) {
  document.getElementById('divTableHead').innerHTML = headerHtml(DIV_COLUMNS, divSortCol, divSortDir, 'setDivSort');

  document.getElementById('divTableBody').innerHTML = dividends.length === 0
    ? `<tr class="empty-row"><td colspan="${DIV_COLUMNS.length}">${tl('noDiv')}</td></tr>`
    : dividends.map(d => '<tr>' + DIV_COLUMNS.map(c => cellHtml(c, d)).join('') + '</tr>').join('');

  const total = dividends.reduce((s, d) => s + d.amount, 0);
  const stockCount = new Set(dividends.map(d => d.symbol_code)).size;
  document.getElementById('divTableFooter').innerHTML = `
    <span>${tl('showingDiv', { n: dividends.length, total: ALL_DIVIDENDS.length })}</span>
    <span>${tl('stocks')} <strong>${stockCount}</strong></span>
    <span>${tl('filteredTotal')} <strong class="${signCls(total)}">${fmtCurrency(total)}</strong></span>${NON_COMPLIANT_DIVIDENDS.length === 0 ? '' : ncFooterHtml(dividends)}`;
}

function setDivSort(col) {
  if (divSortCol === col) divSortDir *= -1;
  else { divSortCol = col; divSortDir = col === 'symbol_code' ? 1 : -1; }
  refreshDiv();
}

function resetDivFilters() {
  document.getElementById('divFilterStock').value = '';
  document.getElementById('divFilterFrom').value = '';
  document.getElementById('divFilterTo').value = '';
  document.getElementById('divFilterMin').value = '';
  document.getElementById('divFilterMax').value = '';
  refreshDiv();
}

function refreshDiv() {
  renderDivTable(getDivFiltered());
}

// ── Page chrome ─────────────────────────────────────────
window.addEventListener('scroll', () => {
  document.getElementById('scrollTop').classList.toggle('visible', window.scrollY > 400);
});

document.getElementById('themeToggle').addEventListener('click', () => {
  const next = document.documentElement.dataset.theme === 'light' ? 'dark' : 'light';
  document.documentElement.dataset.theme = next;
  try { localStorage.setItem('br-theme', next); } catch (e) {}
  if (HAS_TRADES) renderCharts();
  renderSparkline();
});

document.getElementById('langToggle').addEventListener('click', () => {
  LANG = LANG === 'ar' ? 'en' : 'ar';
  document.documentElement.lang = LANG;
  document.documentElement.dir = isRtl() ? 'rtl' : 'ltr';
  try { localStorage.setItem('br-lang', LANG); } catch (e) {}
  renderAll();
  // Re-rendered cards are new elements the reveal observer never saw: show them straight away.
  document.querySelectorAll('.reveal').forEach(el => el.classList.add('in'));
});

// Static text carries data-i18n (text), data-i18n-title, data-i18n-aria and data-i18n-placeholder keys.
function applyStaticText() {
  document.title = `${I18N.en.appName} · ${I18N.ar.appName}`;
  document.querySelectorAll('[data-i18n]').forEach(el => { el.textContent = tl(el.dataset.i18n); });
  document.querySelectorAll('[data-i18n-title]').forEach(el => { el.title = tl(el.dataset.i18nTitle); el.setAttribute('aria-label', el.title); });
  document.querySelectorAll('[data-i18n-aria]').forEach(el => { el.setAttribute('aria-label', tl(el.dataset.i18nAria)); });
  document.querySelectorAll('[data-i18n-placeholder]').forEach(el => { el.placeholder = tl(el.dataset.i18nPlaceholder); });
  document.querySelectorAll('.section-index').forEach((el, i) => { el.textContent = SECTION_INDEX[LANG][i] + '.'; });
  document.getElementById('generatedAt').textContent = tl('updated', { date: new Date().toLocaleDateString(locale(), { day: 'numeric', month: 'long', year: 'numeric' }) });
}

function initObservers() {
  if ('IntersectionObserver' in window) {
    const revealer = new IntersectionObserver(entries => entries.forEach(e => {
      if (e.isIntersecting) { e.target.classList.add('in'); revealer.unobserve(e.target); }
    }), { rootMargin: '0px 0px -40px 0px' });
    document.querySelectorAll('.reveal').forEach((el, i) => { el.style.transitionDelay = (i % 6) * 50 + 'ms'; revealer.observe(el); });

    const links = [...document.querySelectorAll('#nav a')];
    const spy = new IntersectionObserver(entries => entries.forEach(e => {
      if (!e.isIntersecting) return;
      links.forEach(a => a.classList.toggle('active', a.getAttribute('href') === '#' + e.target.id));
    }), { rootMargin: '-30% 0px -60% 0px' });
    links.forEach(a => { const s = document.querySelector(a.getAttribute('href')); if (s) spy.observe(s); });
  }
  // Failsafe: never leave content hidden if the observer doesn't fire (print, some embeds, headless capture).
  setTimeout(() => document.querySelectorAll('.reveal').forEach(el => el.classList.add('in')), 'IntersectionObserver' in window ? 2500 : 0);
}

// Init
if (!HAS_TRADES) document.querySelectorAll('[data-trades]').forEach(el => { el.style.display = 'none'; });

const stocks = [...new Set(ALL_TRADES.map(t => t.reuters_code))].sort();
const sel = document.getElementById('filterStock');
stocks.forEach(s => { const o = document.createElement('option'); o.value = s; o.textContent = s; sel.appendChild(o); });

document.querySelectorAll('#controls select, #controls input').forEach(el => el.addEventListener('change', refresh));

if (ALL_PARTIAL_SELLS.length > 0) {
  document.getElementById('partial-sells-table').style.display = '';
  document.getElementById('navPartialSellsTable').style.display = '';
  const psStocks = [...new Set(ALL_PARTIAL_SELLS.map(t => t.reuters_code))].sort();
  const psSel = document.getElementById('psFilterStock');
  psStocks.forEach(s => { const o = document.createElement('option'); o.value = s; o.textContent = s; psSel.appendChild(o); });
  document.querySelectorAll('#psControls select, #psControls input').forEach(el => el.addEventListener('change', refreshPs));
}

if (ALL_DIVIDENDS.length > 0) {
  document.getElementById('dividends').style.display = '';
  document.getElementById('navDividends').style.display = '';
  const divStocks = [...new Set(ALL_DIVIDENDS.map(d => d.symbol_code))].sort();
  const divSel = document.getElementById('divFilterStock');
  divStocks.forEach(s => { const o = document.createElement('option'); o.value = s; o.textContent = s; divSel.appendChild(o); });
  document.querySelectorAll('#divControls select, #divControls input').forEach(el => el.addEventListener('change', refreshDiv));
}

if (typeof Chart !== 'undefined') {
  Chart.defaults.font.family = getComputedStyle(document.body).fontFamily;
  Chart.defaults.font.size = 11;
}

// Everything that shows text; runs at load and again whenever the language changes.
function renderAll() {
  applyStaticText();
  renderOverview();
  if (HAS_TRADES) {
    renderKpis();
    renderPeriods();
    renderCalendar('tradesCalendar', METRICS.tradesCalendar, 'tradesCalendarSection');
    if (typeof Chart !== 'undefined') renderCharts();
    refresh();
  }
  renderPartialSells();
  renderCalendar('partialSellsCalendar', METRICS.partialSellsCalendar, 'partialSellsCalendarSection');
  if (typeof Chart !== 'undefined') renderSparkline();
  if (ALL_PARTIAL_SELLS.length > 0) {
    if (NON_COMPLIANT.length > 0) document.getElementById('psNote').innerHTML = ncNoteHtml(NON_COMPLIANT, 'sell');
    refreshPs();
  }
  if (ALL_DIVIDENDS.length > 0) {
    if (NON_COMPLIANT_DIVIDENDS.length > 0) document.getElementById('divNote').innerHTML = ncNoteHtml(NON_COMPLIANT_DIVIDENDS, 'dividend');
    refreshDiv();
  }
}

renderAll();
initObservers();
</script>
</body>
</html>
""";
    }
}