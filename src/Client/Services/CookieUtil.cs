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