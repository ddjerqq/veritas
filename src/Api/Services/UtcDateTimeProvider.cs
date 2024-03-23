using Api.Services.Interfaces;

namespace Api.Services;

public class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public long UtcNowUnixTimeMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}