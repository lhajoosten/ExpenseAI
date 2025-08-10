using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Auth.DTOs;

namespace ExpenseAI.Application.Queries.Auth.GetUserProfile;

public record GetUserProfileQuery(
    Guid UserId
) : IQuery<UserProfileDto?>;
