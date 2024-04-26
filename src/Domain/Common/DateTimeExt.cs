namespace Domain.Common;

public static class DateTimeExt
{
    public static long ToUnixMs(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }

    public static DateTime ToUtcDateTime(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
    }

    public static DateTime ToDateTime(this DateOnly dateOnly)
    {
        return new DateTime(dateOnly, default, DateTimeKind.Utc);
    }

    public static string DaysAgoGe(this DateTime date)
    {
        var dateNow = DateTime.UtcNow;
        var diff = dateNow - date;

        return diff.Days switch
        {
            -2 => "მაზეგ",
            -1 => "ზეგ",
            0 => "დღეს",
            1 => "გუშინ",
            2 => "გუშინწინ",
            var days => $"{days} დღის წინ",
        };
    }
}