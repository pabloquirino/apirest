using ApiRest.Application.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public record RevokeTokenCommand(string RefreshToken) : IRequest;

public class RevokeTokenCommandHandler(IAuthService authService)
    : IRequestHandler<RevokeTokenCommand>
{
    public Task Handle(RevokeTokenCommand request, CancellationToken ct)
        => authService.RevokeAsync(request.RefreshToken, ct);
}