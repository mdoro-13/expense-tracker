using System.Security.Claims;

namespace Api.Utils.Helpers;

public static class JwtUtils
{
    public static string? GetUserId(ClaimsPrincipal user)
    {
        var claim = user.Claims.FirstOrDefault(x => x.Type == "user_id");

        if (claim is null)
        {
            throw new BadHttpRequestException("No user_id claim found");
        }

        return claim!.Value;
    }
}
