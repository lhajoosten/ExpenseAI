using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Commands.Auth.ResetPassword;

public class ResetPasswordHandler : ICommandHandler<ResetPasswordCommand, bool>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;

    public ResetPasswordHandler(UserManager<ExpenseAIIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Error("Invalid reset token");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Error($"Failed to reset password: {errors}");
        }

        // Update security stamp to invalidate existing tokens
        await _userManager.UpdateSecurityStampAsync(user);

        return Result.Success(true);
    }
}
