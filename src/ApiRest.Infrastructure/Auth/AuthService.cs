using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ApiRest.Application.Interfaces;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiRest.Infrastructure.Auth;

public class AuthService(
    IOptions<JwtSettings> jwtOptions,
    IRefreshTokenRepository refreshTokenRepo,
    IUnitOfWork uow) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public string GenerateAccessToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name,  user.Name),
            new Claim(ClaimTypes.Role,              user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public async Task<TokenResult> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var stored = await refreshTokenRepo.GetByTokenAsync(refreshToken, ct)
            ?? throw new DomainException("Invalid refresh token.");

        if (!stored.IsValid)
            throw new DomainException("Refresh token is expired or revoked.");

        // Rotacionar: revoga o antigo, gera um novo
        stored.Revoke();

        var newRefreshToken = GenerateRefreshToken();
        var newStored = RefreshToken.Create(
            stored.UserId, newRefreshToken, _jwt.RefreshExpiryDays);

        await refreshTokenRepo.AddAsync(newStored, ct);
        await uow.CommitAsync(ct);

        var accessToken = GenerateAccessToken(stored.User);

        return new TokenResult(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes));
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct)
    {
        var stored = await refreshTokenRepo.GetByTokenAsync(refreshToken, ct)
            ?? throw new DomainException("Invalid refresh token.");

        stored.Revoke();
        await uow.CommitAsync(ct);
    }
}