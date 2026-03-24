using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiRest.API.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value =
            user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
            user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (value == null)
            throw new UnauthorizedAccessException("User id claim not found.");

        return Guid.Parse(value);
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}