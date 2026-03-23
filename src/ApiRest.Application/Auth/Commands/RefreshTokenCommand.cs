using ApiRest.Application.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResult>;

public class RefreshTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, TokenResult>
{
    public Task<TokenResult> Handle(
        RefreshTokenCommand request, CancellationToken ct)
        => authService.RefreshAsync(request.RefreshToken, ct);
}