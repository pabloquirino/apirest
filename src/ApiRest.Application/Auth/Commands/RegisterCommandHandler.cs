using ApiRest.Application.Interfaces;
using ApiRest.Domain.Entities;
using ApiRest.Domain.Exceptions;
using ApiRest.Domain.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public class RegisterCommandHandler(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IAuthService authService,
    IUnitOfWork uow) : IRequestHandler<RegisterCommand, TokenResult>
{
    public async Task<TokenResult> Handle(
        RegisterCommand request, CancellationToken ct)
    {
        if (await userRepo.EmailExistsAsync(request.Email, ct))
            throw new DomainException("Email already registered.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, hash);

        await userRepo.AddAsync(user, ct);

        var rawRefresh = authService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(
            user.Id, rawRefresh, 7); 

        await refreshTokenRepo.AddAsync(refreshToken, ct);
        await uow.CommitAsync(ct);

        var access = authService.GenerateAccessToken(user);
        return new TokenResult(access, rawRefresh, DateTime.UtcNow.AddMinutes(15));
    }
}