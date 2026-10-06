using FFS.Application.Data;
using FFS.Application.Services;
using FFS.Domain.Entities;
using FFS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FFS.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class FinanceController : ControllerBase
{
    private readonly IFinancialDataProvider _data;
    private readonly TransactionQueryService _transactions;
    private readonly ReportingService _reporting;
    private readonly BudgetService _budgets;

    public FinanceController(
        IFinancialDataProvider data,
        TransactionQueryService transactions,
        ReportingService reporting,
        BudgetService budgets)
    {
        _data = data;
        _transactions = transactions;
        _reporting = reporting;
        _budgets = budgets;
    }

    [HttpGet("accounts")]
    [Tags("Accounts")]
    public ActionResult<IReadOnlyList<Account>> Accounts() => Ok(_data.Accounts);

    [HttpGet("accounts/{id:long}")]
    [Tags("Accounts")]
    public ActionResult<Account> Account(long id)
    {
        var account = _data.Accounts.FirstOrDefault(a => a.Id == id);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpGet("categories")]
    [Tags("Categories")]
    public ActionResult<IReadOnlyList<Category>> Categories() => Ok(_data.Categories);

    [HttpGet("goals")]
    [Tags("Goals")]
    public ActionResult<IReadOnlyList<Goal>> Goals() => Ok(_data.Goals);

    [HttpGet("budget")]
    [Tags("Budget")]
    public IActionResult Budget() => Ok(_budgets.GetCurrentMonthOverview());

    [HttpGet("transactions")]
    [Tags("Transactions")]
    public IActionResult Transactions(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] long? accountId,
        [FromQuery] long? categoryId,
        [FromQuery] string? direction,
        [FromQuery] string? merchant,
        [FromQuery] string? search)
    {
        var filter = new TransactionFilter
        {
            Start = from,
            End = to,
            AccountId = accountId,
            CategoryId = categoryId,
            Search = string.IsNullOrWhiteSpace(search) ? merchant : search,
            Direction = Enum.TryParse<MoneyDirection>(direction, true, out var parsed) ? parsed : null
        };
        return Ok(_transactions.Query(filter));
    }

    [HttpGet("transactions/{id:long}")]
    [Tags("Transactions")]
    public ActionResult<Transaction> Transaction(long id)
    {
        var txn = _data.GetTransaction(id);
        return txn is null ? NotFound() : Ok(txn);
    }

    [HttpPost("transactions")]
    [Tags("Transactions")]
    public ActionResult<Transaction> Create([FromBody] CreateTransactionRequest request)
    {
        if (!Enum.TryParse<MoneyDirection>(request.Direction, true, out var direction) ||
            direction is not MoneyDirection.MoneyIn and not MoneyDirection.MoneyOut)
        {
            return BadRequest("Direction must be MoneyIn or MoneyOut.");
        }

        var txn = _data.AddMoney(direction);
        if (request.Amount is decimal amount && amount > 0)
            txn.Amount = amount;
        if (!string.IsNullOrWhiteSpace(request.Merchant))
            txn.Merchant = request.Merchant.Trim();
        _data.UpdateTransaction(txn);
        return Ok(txn);
    }

    [HttpPost("transactions/{id:long}/goal")]
    [Tags("Transactions")]
    public IActionResult LinkGoal(long id, [FromBody] GoalLinkRequest request)
    {
        if (_data.GetTransaction(id) is null) return NotFound();
        try
        {
            _data.SetTransactionGoal(id, request.GoalId);
            return Ok(new
            {
                transactionId = id,
                goalId = _data.GoalIdForTransaction(id)
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reports/cash-flow")]
    [Tags("Reports")]
    public IActionResult CashFlow([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(_reporting.CashFlow(from, to));

    [HttpGet("reports/categories")]
    [Tags("Reports")]
    public IActionResult CategoriesReport([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(_reporting.SpendingByCategory(from, to));

    [HttpGet("reports/banks")]
    [Tags("Reports")]
    public IActionResult Banks([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(_reporting.SpendingByInstitution(from, to));

    [HttpGet("reports/merchants")]
    [Tags("Reports")]
    public IActionResult Merchants([FromQuery] DateTime from, [FromQuery] DateTime to) =>
        Ok(_reporting.TopMerchants(from, to));
}

public record CreateTransactionRequest(string Direction, decimal? Amount, string? Merchant);

public record GoalLinkRequest(long? GoalId);
