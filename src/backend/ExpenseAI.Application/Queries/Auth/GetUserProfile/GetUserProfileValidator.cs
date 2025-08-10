using FluentValidation;

namespace ExpenseAI.Application.Queries.Auth.GetUserProfile;

public class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
