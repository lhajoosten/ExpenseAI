using FluentValidation;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoicesByUser;

public class GetInvoicesByUserValidator : AbstractValidator<GetInvoicesByUserQuery>
{
    public GetInvoicesByUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");

        When(x => x.FromDate.HasValue && x.ToDate.HasValue, () =>
        {
            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .WithMessage("ToDate must be greater than or equal to FromDate");
        });
    }
}
