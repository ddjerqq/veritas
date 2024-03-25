using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auth;

public static class Cookie
{
    public static readonly CookieOptions Options = new()
    {
        Domain = Environment.GetEnvironmentVariable("WEB_APP__DOMAIN"),
        MaxAge = TimeSpan.FromDays(1),
        Secure = true,
        HttpOnly = true,
        IsEssential = true,
        SameSite = SameSiteMode.None,
        Path = "/",
    };
}