using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Shared;

public abstract class AppComponentBase : ComponentBase, IDisposable
{
    [Inject]
    public IToastService Toast { get; set; } = default!;

    private CancellationTokenSource? _cancellationTokenSource;

    protected CancellationToken CancellationToken => (_cancellationTokenSource ??= new CancellationTokenSource()).Token;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_cancellationTokenSource is null)
            return;

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }
}