using ApiRest.Domain.Entities;

namespace ApiRest.Application.Interfaces;

public interface IAuthService
{
    string          GenerateAccessToken(User user);
    string          GenerateRefreshToken();
    Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task            RevokeAsync(string refreshToken, CancellationToken ct = default);
}

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);