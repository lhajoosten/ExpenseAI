using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;

namespace ExpenseAI.Application.Commands.Auth.RefreshToken;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokenService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!result.IsValid)
            return Result.Error("Invalid or expired refresh token");

        var refreshResult = new RefreshTokenResult(
            result.AccessToken,
            result.RefreshToken,
            result.ExpiresAt
        );

        return Result.Success(refreshResult);
    }
}
