namespace Client.Common;

public interface ICancellableComponent : IDisposable
{
    protected CancellationTokenSource? CancellationTokenSource { get; set; }

    protected CancellationToken CancellationToken => (CancellationTokenSource ??= new CancellationTokenSource()).Token;

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