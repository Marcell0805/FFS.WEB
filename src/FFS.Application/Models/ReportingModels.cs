namespace FFS.Application.Models;

public record CashFlowSummary(decimal MoneyIn, decimal MoneyOut, decimal Net, int TransactionCount);

public record CategoryTotal(long? CategoryId, string CategoryName, decimal Total);

public record MerchantTotal(string Merchant, decimal Total, int TransactionCount, string? TopCategoryName);

public record MonthlyTrendPoint(string Month, DateTime MonthStart, decimal MoneyIn, decimal MoneyOut);

public record DailyCashFlowPoint(DateTime Day, string Label, decimal MoneyIn, decimal MoneyOut);

public record ChartPoint(string Label, decimal Value);

public record NamedValue(string Name, decimal Value);
