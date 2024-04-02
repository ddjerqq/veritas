using Microsoft.JSInterop;

namespace Client.Services;

public class UtilJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new (() => jsRuntime.InvokeAsync<IJSObjectReference>(
        "import", "./util.js").AsTask());

    public async ValueTask<string> SaveAsFile(string fileName, byte[] data)
    {
        var module = await _moduleTask.Value;
        var bytesBase64 = Convert.ToBase64String(data);
        return await module.InvokeAsync<string>("saveAsFile", fileName, bytesBase64);
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
