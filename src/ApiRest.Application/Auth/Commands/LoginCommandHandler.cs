using ApiRest.Application.Interfaces;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public class LoginCommandHandler(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IAuthService authService,
    IUnitOfWork uow) : IRequestHandler<LoginCommand, TokenResult>
{
    public async Task<TokenResult> Handle(
        LoginCommand request, CancellationToken ct)
    {
        var user = await userRepo.GetByEmailAsync(request.Email, ct)
            ?? throw new DomainException("Invalid credentials.");

        if (!user.IsActive)
            throw new DomainException("User is inactive.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid credentials.");

        // Revoga todos os refresh tokens anteriores do usuário
        await refreshTokenRepo.RevokeAllByUserAsync(user.Id, ct);

        var rawRefresh   = authService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, rawRefresh, 7);

        await refreshTokenRepo.AddAsync(refreshToken, ct);
        await uow.CommitAsync(ct);

        var access = authService.GenerateAccessToken(user);
        return new TokenResult(access, rawRefresh, DateTime.UtcNow.AddMinutes(15));
    }
}