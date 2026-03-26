using ApiRest.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest req,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new RegisterCommand(req.Name, req.Email, req.Password), ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest req,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new LoginCommand(req.Email, req.Password), ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest req,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new RefreshTokenCommand(req.RefreshToken), ct);
        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(
        [FromBody] RefreshRequest req,
        CancellationToken ct)
    {
        await sender.Send(new RevokeTokenCommand(req.RefreshToken), ct);
        return NoContent();
    }
}

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);