using ApiRest.Application.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<TokenResult>;