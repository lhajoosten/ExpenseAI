using FluentValidation;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseStats;

public class GetExpenseStatsValidator : AbstractValidator<GetExpenseStatsQuery>
{
    public GetExpenseStatsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x)
            .Must(x => x.StartDate == null || x.EndDate == null || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be less than or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Start date cannot be in the future")
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(1))
            .WithMessage("End date cannot be more than 1 day in the future")
            .When(x => x.EndDate.HasValue);
    }
}
