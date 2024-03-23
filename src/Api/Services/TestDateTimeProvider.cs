using Api.Services.Interfaces;

namespace Api.Services;

public class TestDateTimeProvider(DateTime time) : IDateTimeProvider
{
    public DateTime UtcNow => time;
    public long UtcNowUnixTimeMilliseconds => new DateTimeOffset(time).ToUnixTimeMilliseconds();
}