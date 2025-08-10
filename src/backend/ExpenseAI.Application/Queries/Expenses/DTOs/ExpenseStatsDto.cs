namespace ExpenseAI.Application.Queries.Expenses.DTOs;

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
