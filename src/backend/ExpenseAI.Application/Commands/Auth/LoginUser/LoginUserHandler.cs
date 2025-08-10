using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Commands.Auth.LoginUser;

public class LoginUserHandler : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;
    private readonly SignInManager<ExpenseAIIdentityUser> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(
        UserManager<ExpenseAIIdentityUser> userManager,
        SignInManager<ExpenseAIIdentityUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginUserResult>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Error("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Error("Account is locked due to multiple failed login attempts");

        if (result.IsNotAllowed)
            return Result.Error("Login not allowed. Please confirm your email address");

        if (!result.Succeeded)
            return Result.Error("Invalid email or password");

        // Generate JWT tokens
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var loginResult = new LoginUserResult(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            accessToken.Token,
            refreshToken.Token,
            accessToken.ExpiresAt,
            user.EmailConfirmed
        );

        return Result.Success(loginResult);
    }
}
