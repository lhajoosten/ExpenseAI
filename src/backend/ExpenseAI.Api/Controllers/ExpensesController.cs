using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ExpenseAI.Application.Commands.Expenses.CreateExpense;
using ExpenseAI.Application.Commands.Expenses.UpdateExpense;
using ExpenseAI.Application.Commands.Expenses.DeleteExpense;
using ExpenseAI.Application.Commands.Expenses.UploadReceipt;
using ExpenseAI.Application.Queries.Expenses.GetExpenseById;
using ExpenseAI.Application.Queries.Expenses.GetExpensesByUser;
using ExpenseAI.Application.Queries.Expenses.SearchExpenses;
using ExpenseAI.Application.Queries.Expenses.GetExpenseStats;
using ExpenseAI.Application.Queries.Expenses.DTOs;
using ExpenseAI.Application.Common;

namespace ExpenseAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all expenses for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseDto>>> GetExpenses(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userId = GetCurrentUserId();
        var query = new GetExpensesByUserQuery(userId, skip, take);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Get a specific expense by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpense(Guid id)
    {
        var userId = GetCurrentUserId();
        var query = new GetExpenseByIdQuery(id, userId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            if (result.Value == null)
                return NotFound();

            return Ok(result.Value);
        }

        if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
            return Forbid();

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Search expenses with filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ExpenseDto>>> SearchExpenses(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] DateTimeOffset? startDate = null,
        [FromQuery] DateTimeOffset? endDate = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var userId = GetCurrentUserId();
        var query = new SearchExpensesQuery(
            userId, searchTerm, category, minAmount, maxAmount,
            startDate, endDate, skip, take);

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Get expense statistics and analytics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ExpenseStatsDto>> GetExpenseStats(
        [FromQuery] DateTimeOffset? startDate = null,
        [FromQuery] DateTimeOffset? endDate = null)
    {
        var userId = GetCurrentUserId();
        var query = new GetExpenseStatsQuery(userId, startDate, endDate);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        var userId = GetCurrentUserId();
        var command = new CreateExpenseCommand(
            userId,
            request.Description,
            request.Amount,
            request.Currency,
            request.CategoryName,
            request.ExpenseDate,
            request.Notes,
            request.MerchantName,
            request.PaymentMethod,
            request.IsReimbursable);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetExpense), new { id = result.Value }, result.Value);

        if (result.Status == Ardalis.Result.ResultStatus.Invalid)
            return BadRequest(result.ValidationErrors);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Update an existing expense
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateExpenseCommand(
            id,
            userId,
            request.Description,
            request.Amount,
            request.Currency,
            request.CategoryName,
            request.ExpenseDate,
            request.Notes,
            request.MerchantName,
            request.PaymentMethod,
            request.IsReimbursable);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound();

        if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
            return Forbid();

        if (result.Status == Ardalis.Result.ResultStatus.Invalid)
            return BadRequest(result.ValidationErrors);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Delete an expense
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteExpense(Guid id)
    {
        var userId = GetCurrentUserId();
        var command = new DeleteExpenseCommand(id, userId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return NoContent();

        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound();

        if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
            return Forbid();

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Upload a receipt for an expense
    /// </summary>
    [HttpPost("{id}/receipt")]
    public async Task<ActionResult<string>> UploadReceipt(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var userId = GetCurrentUserId();
        var command = new UploadReceiptCommand(
            id, userId, file.OpenReadStream(), file.FileName, file.ContentType);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { ReceiptUrl = result.Value });

        if (result.Status == Ardalis.Result.ResultStatus.NotFound)
            return NotFound();

        if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
            return Forbid();

        if (result.Status == Ardalis.Result.ResultStatus.Invalid)
            return BadRequest(result.ValidationErrors);

        return BadRequest(result.Errors);
    }

    private Guid GetCurrentUserId()
    {
        // In a real implementation, this would extract the user ID from the JWT token
        // For now, return a placeholder
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        // Fallback for development - in production this should throw an exception
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}

// Request DTOs
public record CreateExpenseRequest(
    string Description,
    decimal Amount,
    string Currency,
    string CategoryName,
    DateTimeOffset ExpenseDate,
    string? Notes = null,
    string? MerchantName = null,
    string? PaymentMethod = null,
    bool IsReimbursable = false);

public record UpdateExpenseRequest(
    string Description,
    decimal Amount,
    string Currency,
    string CategoryName,
    DateTimeOffset ExpenseDate,
    string? Notes = null,
    string? MerchantName = null,
    string? PaymentMethod = null,
    bool? IsReimbursable = null);
