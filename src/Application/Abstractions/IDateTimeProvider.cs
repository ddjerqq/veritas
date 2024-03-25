namespace Application.Abstractions;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }

    public long UtcNowUnixTimeMilliseconds { get; }
}