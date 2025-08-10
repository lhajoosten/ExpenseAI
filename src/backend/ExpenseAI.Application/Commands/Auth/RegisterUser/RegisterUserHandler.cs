using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Commands.Auth.RegisterUser;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public RegisterUserHandler(
        UserManager<ExpenseAIIdentityUser> userManager,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<Result<RegisterUserResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result.Error("User with this email already exists");

        // Create new user
        var user = new ExpenseAIIdentityUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = false // Require email confirmation
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Error($"Failed to create user: {errors}");
        }

        // Generate email confirmation token
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Send confirmation email (fire and forget)
        _ = Task.Run(async () =>
        {
            await _emailService.SendEmailConfirmationAsync(user.Email, emailConfirmationToken, cancellationToken);
        }, cancellationToken);

        // Generate JWT tokens
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        var registerResult = new RegisterUserResult(
            user.Id,
            user.Email!,
            accessToken.Token,
            refreshToken.Token,
            accessToken.ExpiresAt
        );

        return Result.Success(registerResult);
    }
}
