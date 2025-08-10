namespace ExpenseAI.Application.Queries.Expenses.DTOs;

public class CategoryStatsDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}
