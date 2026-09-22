using FFS.Application.Data;
using FFS.Application.Models;
using FFS.Domain.Entities;
using FFS.Domain.Enums;

namespace FFS.Application.Services;

public sealed class TransactionQueryService
{
    private readonly IFinancialDataProvider _data;

    public TransactionQueryService(IFinancialDataProvider data) => _data = data;

    public IReadOnlyList<TransactionRow> Query(TransactionFilter filter)
    {
        IEnumerable<Transaction> query = _data.Transactions;

        if (filter.Start.HasValue)
            query = query.Where(t => t.TransactionDate >= filter.Start.Value.Date);
        if (filter.End.HasValue)
            query = query.Where(t => t.TransactionDate <= filter.End.Value.Date.AddDays(1).AddTicks(-1));
        if (filter.AccountId.HasValue)
            query = query.Where(t => t.AccountId == filter.AccountId.Value);
        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        if (filter.Direction.HasValue)
            query = query.Where(t => t.Direction == filter.Direction.Value);
        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim();
            query = query.Where(t =>
                t.Merchant.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                (t.Reference?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (_data.Accounts.FirstOrDefault(a => a.Id == t.AccountId)?.InstitutionName
                    .Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var accounts = _data.Accounts.ToDictionary(a => a.Id);
        var categories = _data.Categories.ToDictionary(c => c.Id);

        query = filter.SortBy switch
        {
            TransactionSortBy.Merchant => filter.SortDesc
                ? query.OrderByDescending(t => t.Merchant)
                : query.OrderBy(t => t.Merchant),
            TransactionSortBy.Amount => filter.SortDesc
                ? query.OrderByDescending(t => t.Amount)
                : query.OrderBy(t => t.Amount),
            TransactionSortBy.Status => filter.SortDesc
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            TransactionSortBy.Direction => filter.SortDesc
                ? query.OrderByDescending(t => t.Direction)
                : query.OrderBy(t => t.Direction),
            TransactionSortBy.Bank => filter.SortDesc
                ? query.OrderByDescending(t => accounts.TryGetValue(t.AccountId, out var ab) ? ab.InstitutionName : "")
                : query.OrderBy(t => accounts.TryGetValue(t.AccountId, out var ab) ? ab.InstitutionName : ""),
            TransactionSortBy.Category => filter.SortDesc
                ? query.OrderByDescending(t => t.CategoryId is long cid && categories.TryGetValue(cid, out var cb) ? cb.Name : "")
                : query.OrderBy(t => t.CategoryId is long cid && categories.TryGetValue(cid, out var cb) ? cb.Name : ""),
            _ => filter.SortDesc
                ? query.OrderByDescending(t => t.TransactionDate).ThenByDescending(t => t.Id)
                : query.OrderBy(t => t.TransactionDate).ThenBy(t => t.Id)
        };

        return query.Select(t =>
        {
            accounts.TryGetValue(t.AccountId, out var a);
            var accountName = a?.Name ?? "Unknown";
            var bankName = a?.InstitutionName ?? "—";
            var categoryName = t.CategoryId is long id && categories.TryGetValue(id, out var c)
                ? c.Name
                : "Uncategorized";
            return new TransactionRow(t, accountName, bankName, categoryName);
        }).ToList();
    }

    public Transaction? Get(long id) => _data.GetTransaction(id);

    public void Update(Transaction transaction) => _data.UpdateTransaction(transaction);
}

public enum TransactionSortBy
{
    Date,
    Merchant,
    Amount,
    Status,
    Bank,
    Category,
    Direction
}

public sealed class TransactionFilter
{
    public string? Search { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public long? AccountId { get; set; }
    public long? CategoryId { get; set; }
    public MoneyDirection? Direction { get; set; }
    public TransactionStatus? Status { get; set; }
    public TransactionSortBy SortBy { get; set; } = TransactionSortBy.Date;
    public bool SortDesc { get; set; } = true;
}

public record TransactionRow(
    Transaction Transaction,
    string AccountName,
    string BankName,
    string CategoryName);

public enum TransactionGroupBy
{
    None,
    Category,
    Merchant,
    Bank
}
