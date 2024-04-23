using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Common;

public abstract class AppComponentBase : QueryParameterBinderComponentBase, IToastComponent, IDisposable
{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    public CancellationToken CancellationToken => (CancellationTokenSource ??= new CancellationTokenSource()).Token;

    [Inject]
    public IToastService Toast { get; set; } = default!;

    void IDisposable.Dispose()
    {
        if (CancellationTokenSource is not null)
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
            CancellationTokenSource = null;
        }
    }
}