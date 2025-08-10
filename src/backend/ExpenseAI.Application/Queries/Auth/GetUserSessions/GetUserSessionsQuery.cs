using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Auth.DTOs;

namespace ExpenseAI.Application.Queries.Auth.GetUserSessions;

public record GetUserSessionsQuery(
    Guid UserId
) : IQuery<List<UserSessionDto>>;
