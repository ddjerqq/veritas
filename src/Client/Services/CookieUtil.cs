using System.Globalization;
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

    public async Task<FullVoterDto?> GetVoterAsync()
    {
        var address = await GetCookie(nameof(FullVoterDto.Address));
        var publicKey = await GetCookie(nameof(FullVoterDto.PublicKey));
        var privateKey = await GetCookie(nameof(FullVoterDto.PrivateKey));
        var signature = await GetCookie(nameof(FullVoterDto.Signature));
        var time = await GetCookie(nameof(FullVoterDto.LastVoteTime));

        if (address is null || publicKey is null || privateKey is null || signature is null)
            return null;

        Console.WriteLine($"time is {time}");

        var lastVote = DateTime.TryParse(time, out var date) ? date : DateTime.MinValue;
        Console.WriteLine($"last vote time is {lastVote}");

        return new FullVoterDto(address, publicKey, privateKey, signature, lastVote);
    }

    public async Task SetVoterAsync(FullVoterDto voter)
    {
        await SetCookie(nameof(FullVoterDto.Address), voter.Address, 365);
        await SetCookie(nameof(FullVoterDto.PublicKey), voter.PublicKey, 365);
        await SetCookie(nameof(FullVoterDto.PrivateKey), voter.PrivateKey, 365);
        await SetCookie(nameof(FullVoterDto.Signature), voter.Signature, 365);
        await SetCookie(nameof(FullVoterDto.LastVoteTime), voter.LastVoteTime.ToString("u")!, 365);
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