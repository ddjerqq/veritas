namespace Client.Services;

public class IpService(ApiService api, CookieUtil cookie)
{
    public async Task<string> GetIpAddress(CancellationToken ct = default)
    {
        var ip = await cookie.GetCookie("ip_addr");
        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = await api.GetIp(ct);
            await cookie.SetCookie("ip_addr", ip, 14);
        }

        return ip;
    }
}