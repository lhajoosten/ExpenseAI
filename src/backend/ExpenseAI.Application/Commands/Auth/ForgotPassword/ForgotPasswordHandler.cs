using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Commands.Auth.ForgotPassword;

public class ForgotPasswordHandler : ICommandHandler<ForgotPasswordCommand, bool>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler(UserManager<ExpenseAIIdentityUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Always return success to prevent email enumeration attacks
        if (user == null || !user.EmailConfirmed)
            return Result.Success(true);

        // Generate password reset token
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Send password reset email (fire and forget)
        _ = Task.Run(async () =>
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email!, resetToken, cancellationToken);
        }, cancellationToken);

        return Result.Success(true);
    }
}
