using ApiRest.Application.Interfaces;
using MediatR;

namespace ApiRest.Application.Auth.Commands;

public record RegisterCommand(
    string Name,
    string Email,
    string Password) : IRequest<TokenResult>;