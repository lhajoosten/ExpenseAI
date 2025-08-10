using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Commands.Auth.ConfirmEmail;

public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand, bool>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;

    public ConfirmEmailHandler(UserManager<ExpenseAIIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Error("Invalid confirmation token");

        if (user.EmailConfirmed)
            return Result.Success(true); // Already confirmed

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Error($"Failed to confirm email: {errors}");
        }

        return Result.Success(true);
    }
}
