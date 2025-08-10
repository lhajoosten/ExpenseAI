namespace ExpenseAI.Application.Queries.Expenses.DTOs;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public DateTimeOffset ExpenseDate { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? MerchantName { get; set; }
    public string? PaymentMethod { get; set; }
    public bool IsReimbursable { get; set; }
    public bool IsReimbursed { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAiCategorized { get; set; }
    public double? AiConfidenceScore { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class ExpenseStatsDto
{
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public decimal AverageAmount { get; set; }
    public List<CategoryStatsDto> CategoryBreakdown { get; set; } = new();
    public List<MonthlyStatsDto> MonthlyBreakdown { get; set; } = new();
    public decimal PendingReimbursements { get; set; }
}

public class CategoryStatsDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyStatsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
