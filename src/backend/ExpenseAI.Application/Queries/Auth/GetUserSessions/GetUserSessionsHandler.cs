using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Auth.DTOs;

namespace ExpenseAI.Application.Queries.Auth.GetUserSessions;

public class GetUserSessionsHandler : IQueryHandler<GetUserSessionsQuery, List<UserSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSessionsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<UserSessionDto>>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _unitOfWork.UserSessions.GetActiveSessionsByUserAsync(request.UserId, cancellationToken);
        var currentSessionId = _currentUserService.SessionId;

        var dtos = sessions.Select(session => new UserSessionDto
        {
            Id = session.Id,
            DeviceInfo = session.DeviceInfo,
            IpAddress = session.IpAddress,
            CreatedAt = session.CreatedAt,
            LastActiveAt = session.LastActiveAt,
            IsActive = session.IsActive,
            IsCurrent = session.Id == currentSessionId
        }).ToList();

        return Result.Success(dtos);
    }
}
