using Blazored.Toast;
using Blazored.Toast.Services;
using Client.Shared;
using Microsoft.AspNetCore.Components;

namespace Client.Common;

public abstract class AppComponentBase : QueryParameterBinderComponentBase, IDisposable
{
    private CancellationTokenSource? CancellationTokenSource { get; set; }

    protected CancellationToken CancellationToken => (CancellationTokenSource ??= new CancellationTokenSource()).Token;

    [Inject]
    public IToastService Toast { get; set; } = default!;

    public static void ShowToast(IToastService toast, ToastLevel level, string content)
    {
        RenderFragment childContent = builder =>
        {
            builder.AddContent(0, content);
        };

        var toastParameters = new ToastParameters();
        toastParameters.Add(nameof(Shared.Toast.Level), level);
        toastParameters.Add(nameof(Shared.Toast.ToastService), toast);
        toastParameters.Add(nameof(Shared.Toast.ChildContent), childContent);

        toast.ShowToast<Toast>(toastParameters, settings =>
        {
            settings.Timeout = 3;
        });
    }

    protected void ShowInfo(string content) => ShowToast(Toast, ToastLevel.Info, content);

    protected void ShowSuccess(string content) => ShowToast(Toast, ToastLevel.Success, content);

    protected void ShowWarning(string content) => ShowToast(Toast, ToastLevel.Warning, content);

    protected void ShowError(string content) => ShowToast(Toast, ToastLevel.Error, content);

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