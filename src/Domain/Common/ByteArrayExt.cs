namespace Domain.Common;

public static class ByteArrayExt
{
    public static string ToHexString(this byte[] bytes)
    {
        return BitConverter
            .ToString(bytes)
            .Replace("-", "")
            .ToLower();
    }

    public static string ToBase64String(this byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

    public static bool ArrayEquals(this byte[]? a, byte[]? b)
    {
        if (a is null) return b is null;
        if (b is null) return false;

        if (a.Length != b.Length) return false;

        ReadOnlySpan<byte> aSpan = a;
        ReadOnlySpan<byte> bSpan = b;

        return aSpan.SequenceEqual(bSpan);
    }
}