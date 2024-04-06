using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Common;

public abstract class AppComponentBase : QueryParameterBinderComponentBase, ICancellableComponent, IToastComponent
{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    [Inject]
    public IToastService Toast { get; set; } = default!;
}