using FFS.Application;
using FFS.Application.Services;
using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Web.Services;

public static class AssistantAnswers
{
    public static string Answer(string query, IReadOnlyList<FaqEntry> faq, DashboardViewModel dash)
    {
        var live = TryLive(query, dash);
        if (live is not null) return live;

        var hits = FaqSearch.Rank(faq, query);
        if (hits.Count == 0 || hits[0].Score <= 0)
            return "I don't have an answer for that yet. Try Help, or ask about your money in, money out, budget, or goals.";

        var text = hits[0].Entry.Answer;
        if (hits.Count > 1 && hits[1].Score * 2 >= hits[0].Score)
            text += "\n\nRelated: " + hits[1].Entry.Question;
        return text;
    }

    private static string? TryLive(string query, DashboardViewModel dash)
    {
        var tokens = FaqSearch.Tokens(query).ToHashSet(StringComparer.Ordinal);
        bool Has(params string[] words) => words.Any(tokens.Contains);

        if (Has("goal", "goals", "track", "behind") || (Has("saved", "saving") && !Has("budget", "planned", "needs", "wants")))
            return Goals(dash);

        if (Has("budget", "needs", "wants", "planned") || Has("savings"))
            return Budget(dash);

        if (Has("income", "earned") || (Has("money") && Has("in")))
            return $"Your money in this month is {MoneyFormat.Zar(dash.ThisMonthCashFlow.MoneyIn)}.";

        if (Has("spent", "spending", "expenses", "expense") || (Has("money") && Has("out")))
            return $"Your money out this month is {MoneyFormat.Zar(dash.ThisMonthCashFlow.MoneyOut)}.";

        if (Has("net") || (Has("cash") && Has("flow")))
        {
            var net = dash.ThisMonthCashFlow.Net;
            var tone = net >= 0 ? "You're cash positive this month." : "You're cash negative this month.";
            return $"Your net cash flow this month is {MoneyFormat.Zar(net)}. {tone}";
        }

        return null;
    }

    private static string Goals(DashboardViewModel dash)
    {
        var goals = dash.Goals;
        if (goals.Count == 0)
            return "You don't have an active goal yet. Add one under Goals when you're ready.";

        var saved = goals.Sum(g => g.Goal.CurrentAmount);
        var lines = new List<string> { $"You have saved {MoneyFormat.Zar(saved)} across your goals." };
        foreach (var g in goals)
        {
            var pct = ProgressScale.Percent(g.Goal.CurrentAmount, g.Goal.TargetAmount);
            lines.Add($"{g.Goal.Name}: you have saved {MoneyFormat.Zar(g.Goal.CurrentAmount)} of {MoneyFormat.Zar(g.Goal.TargetAmount)} ({ProgressScale.Label(pct)}%), so this goal is {Status(g.Goal)}.");
        }

        lines.Add("On track means you have saved at least half of your target. Behind means you have saved less than half. Your target date is not part of that.");
        return string.Join("\n", lines);
    }

    private static string Budget(DashboardViewModel dash)
    {
        var buckets = dash.Budget.Buckets;
        if (buckets.Count == 0 || buckets.Sum(b => b.Planned) <= 0)
            return "You don't have a Needs, Wants, and Savings plan for this month yet.";

        var lines = new List<string> { "Here is your plan for this month." };
        foreach (var b in buckets)
        {
            var middle = b.Bucket == BudgetBucketKind.Savings ? "saved" : "spent";
            var last = b.Bucket == BudgetBucketKind.Savings ? "vs your target" : "remaining";
            lines.Add($"{b.Bucket}: planned {MoneyFormat.Zar(b.Planned)}, {middle} {MoneyFormat.Zar(b.Actual)}, {last} {MoneyFormat.Zar(b.Remaining)}.");
        }

        return string.Join("\n", lines);
    }

    private static string Status(Goal goal)
    {
        if (goal.Progress >= 1) return "completed";
        if (goal.Progress >= 0.5m) return "on track";
        return "behind";
    }
}
