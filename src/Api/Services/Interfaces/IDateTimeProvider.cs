namespace Api.Services.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
    public long UtcNowUnixTimeMilliseconds { get; }
}