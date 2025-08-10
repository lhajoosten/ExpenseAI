using FluentValidation;

namespace ExpenseAI.Application.Queries.Auth.GetUserSessions;

public class GetUserSessionsValidator : AbstractValidator<GetUserSessionsQuery>
{
    public GetUserSessionsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
