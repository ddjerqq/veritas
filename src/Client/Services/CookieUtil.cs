using Application.Dto;
using Microsoft.JSInterop;

namespace Client.Services;

public class CookieUtil(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
        "import", "./js/cookie.js").AsTask());

    public async ValueTask<string?> GetCookie(string name)
    {
        var module = await _moduleTask.Value;
        var cookie = await module.InvokeAsync<string?>("get", name);
        if (string.IsNullOrWhiteSpace(cookie)) return null;
        if (cookie is "null" or "undefined") return null;
        return cookie;
    }

    public async ValueTask SetCookie(string name, string value, int expireDays)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("set", name, value, expireDays);
    }

    public async ValueTask<FullVoterDto?> GetVoter()
    {
        var address = await GetCookie(nameof(FullVoterDto.Address));
        var publicKey = await GetCookie(nameof(FullVoterDto.PublicKey));
        var privateKey = await GetCookie(nameof(FullVoterDto.PrivateKey));
        var signature = await GetCookie(nameof(FullVoterDto.Signature));

        if (string.IsNullOrWhiteSpace(address)
            || string.IsNullOrWhiteSpace(publicKey)
            || string.IsNullOrWhiteSpace(privateKey)
            || string.IsNullOrWhiteSpace(signature))
            return null;

        return new FullVoterDto(address, publicKey, privateKey, signature);
    }

    public async ValueTask SetVoter(FullVoterDto voter)
    {
        await SetCookie(nameof(FullVoterDto.Address), voter.Address, 365);
        await SetCookie(nameof(FullVoterDto.PublicKey), voter.PublicKey, 365);
        await SetCookie(nameof(FullVoterDto.PrivateKey), voter.PrivateKey, 365);
        await SetCookie(nameof(FullVoterDto.Signature), voter.Signature, 365);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}