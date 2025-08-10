using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Auth.DTOs;
using ExpenseAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Application.Queries.Auth.GetUserProfile;

public class GetUserProfileHandler : IQueryHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly UserManager<ExpenseAIIdentityUser> _userManager;

    public GetUserProfileHandler(UserManager<ExpenseAIIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserProfileDto?>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result.Success<UserProfileDto?>(null);

        var dto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

        return Result.Success<UserProfileDto?>(dto);
    }
}
