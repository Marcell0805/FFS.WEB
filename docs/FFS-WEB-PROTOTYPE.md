# FFS Web Prototype (V0)

**Financial Forecasting System** — experimental in-memory Blazor web prototype.

Location: `D:\repos\FFS Web`

## Purpose

Answer:

> How well can the existing FFS Mobile data model and AppGen architecture be leveraged for a useful desktop/web FFS experience?

This is intentionally a throwaway / evolutionary V0. Optimize for learning, not production.

## How to run

```powershell
cd "D:\repos\FFS Web"
dotnet run --project src/FFS.Web
```

Open the printed URL (default `http://localhost:5049`). Demo data loads in memory on startup. No database required.

## What was reused from AppGen

| Reused | Detail |
|--------|--------|
| Naming convention | Sibling folder `{App} Web` under `D:\repos\` |
| Layered mental model | Domain → Application → Web |
| Blazor host idea | Inspired by unwired AppGen Blazor stubs (sidebar shell) |

### AppGen limitations encountered

- AppGen product UI generation emits **MVC + EF + real SQL**, not Blazor.
- Blazor templates exist under AppGen but are **not wired** into `SolutionGenerator`.
- `BlazorWeb` in AppGen config aliases to **MvcWeb**.
- FFS hub has `Targets.Web.Enabled: false`.
- No charting packages in AppGen web templates.
- Classic `dotnet new blazorserver` is unavailable on .NET 10 SDK; V0 uses `dotnet new blazor -int Server` instead.

**Decision:** hand-build a slim Blazor Web App. Do not run AppGen generate-web for this experiment (would force DB/CRUD).

## What was reused from FFS Mobile

| Concept | Notes |
|---------|--------|
| Account, Category, Transaction, Goal, GoalTransaction, NotificationSource | Ported semantics to C# |
| MoneyDirection | MoneyIn, MoneyOut, Transfer |
| TransactionStatus | Captured, Confirmed, Edited, Ignored |
| CategoryType | Income, Expense, Transfer |
| Reporting rules | Ignored excluded; Transfer excluded from cash flow; spending = MoneyOut only |
| Date range presets | This Month → Last 12 Months |
| Brand colours | Teal/slate from mobile theme |
| Tagline | “Know your money. Forecast your future.” |

Budget / BudgetBucket / BudgetItem were **vision-only** in mobile — implemented lightly for V0 from `docs/FFS-VISION-AND-ROADMAP.md`.

## Architecture

```text
UI (Blazor pages)
  ↓
Application services
  ↓
Reporting / budget / goal / projection rules
  ↓
IFinancialDataProvider
  ↓
InMemoryFinancialDataProvider
  ↓
FfsDemoSeed (startup)
```

Projects:

- `src/FFS.Domain` — entities & enums
- `src/FFS.Application` — provider, seed, services
- `src/FFS.Web` — Blazor interactive server UI

## Data model

- **Account** — Everyday Banking, Savings, Credit Card (ZAR)
- **Category** — includes CategoryType + separate `BudgetBucket` assignment
- **Transaction** — positive Amount + Direction + Status
- **Goal** / **GoalTransaction** — progress from stored `CurrentAmount`; contributions are a history log
- **Budget** / **BudgetItem** — monthly income target + planned amounts per category/bucket
- **BudgetBucketKind** — Needs, Wants, Savings, Unassigned

## In-memory provider

`IFinancialDataProvider` with `InMemoryFinancialDataProvider` singleton.

Supports read collections + `UpdateTransaction` for session-only edits.

Future (not implemented): `SqliteFinancialDataProvider`, `ApiFinancialDataProvider`.

## Financial calculation rules

1. Status **Ignored** → excluded from cash flow and spending aggregates.
2. Direction **Transfer** → does not affect cash flow; not treated as spending.
3. **MoneyIn** → income; **MoneyOut** → spending/outflow.
4. Net = MoneyIn − MoneyOut.
5. Spending-by-category / top merchants = MoneyOut only, not Ignored.
6. CategoryType ≠ BudgetBucket.

## Screens

1. **Dashboard** — KPIs, cash-flow chart, spending, budget snapshot, goals, recent txns  
2. **Transactions** — search, filters, sort, detail drawer, in-memory edit  
3. **Reports** — Income vs Spending, period cash flow, monthly trend, category donut, top merchants  
4. **Planning** — income target vs actual; Needs/Wants/Savings planned/actual/remaining; category table  
5. **Goals** — progress bars, targets, contribution history  
6. **Forecast** — labelled **Simple Projection** (3/6/12 months)

## Forecasting approach

Naive average of recent ~3 months MoneyIn/MoneyOut applied forward.

Clearly labelled **Simple Projection** — not AI and not the final FFS forecast engine.

## Assumptions

- Single demo household; no multi-user.
- Account balances estimated from openings + cash-flow txns (transfers excluded from that adjustment).
- Goal `CurrentAmount` is authoritative; contribution rows do not auto-roll up (same as mobile).
- Current-month budget is seeded for “today”.

## Limitations / intentionally excluded

Auth, accounts, sync, multi-tenancy, bank APIs, statement import, AI, EF/DB, production deploy, notification capture/OCR, perfecting mobile web layout.

## Evolution toward production FFS Web

1. Extract shared financial rules into a portable library used by Flutter + Web.
2. Swap `IFinancialDataProvider` for SQLite / API without rewriting UI.
3. Revisit AppGen when Blazor product generation (or a shared web shell) is ready.
4. Add persistence, auth, and sync only when moving toward FFS Plus.

## Stack

- .NET 8 Blazor Web App (Interactive Server)
- Blazor-ApexCharts (free)
- Bootstrap Icons + custom CSS (FFS teal/slate)
- No DevExpress / paid UI
