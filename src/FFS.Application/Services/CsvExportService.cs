using System.Text;
using FFS.Application.Data;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

public sealed class CsvExportService
{
    private readonly IFinancialDataProvider _data;
    private readonly BudgetService _budget;

    public CsvExportService(IFinancialDataProvider data, BudgetService budget)
    {
        _data = data;
        _budget = budget;
    }

    public string ExportWorkbook()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== transactions ===");
        sb.AppendLine("date,merchant,description,account,direction,status,amount");
        var accounts = _data.Accounts.ToDictionary(a => a.Id);
        foreach (var t in _data.Transactions.OrderByDescending(x => x.TransactionDate))
        {
            accounts.TryGetValue(t.AccountId, out var a);
            sb.AppendLine(string.Join(',',
                Csv(t.TransactionDate.ToString("yyyy-MM-dd")),
                Csv(t.Merchant),
                Csv(t.Description),
                Csv(a?.Name ?? ""),
                Csv(t.Direction.ToString()),
                Csv(t.Status.ToString()),
                t.Amount.ToString("0.00")));
        }

        sb.AppendLine();
        sb.AppendLine("=== budget ===");
        var overview = _budget.GetCurrentMonthOverview();
        sb.AppendLine($"year,month,income_target,actual_income");
        sb.AppendLine($"{overview.Year},{overview.Month},{overview.IncomeTarget:0.00},{overview.ActualIncome:0.00}");

        sb.AppendLine();
        sb.AppendLine("=== budget_bucket ===");
        sb.AppendLine("bucket,planned,actual,remaining");
        foreach (var b in overview.Buckets)
            sb.AppendLine($"{b.Bucket},{b.Planned:0.00},{b.Actual:0.00},{b.Remaining:0.00}");

        sb.AppendLine();
        sb.AppendLine("=== budget_item ===");
        sb.AppendLine("category,bucket,planned,actual,remaining");
        foreach (var r in overview.Categories)
            sb.AppendLine($"{Csv(r.CategoryName)},{r.Bucket},{r.Planned:0.00},{r.Actual:0.00},{r.Remaining:0.00}");

        return sb.ToString();
    }

    private static string Csv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
