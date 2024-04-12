using System.Security.Cryptography;
using System.Text;

namespace Domain.Common;

public static class StringExt
{
    private static readonly char[] Chars = "abcdef0123456789".ToArray();

    public static string RandomHexString(int length)
    {
        return new string(Random.Shared.GetItems(Chars, length));
    }

    private static int GetHexVal(char hex)
    {
        return hex - (hex < 58 ? 48 : hex < 97 ? 55 : 87);
    }

    public static byte[] ToBytesFromHex(this string hex)
    {
        // add a 0 before the last character
        if (hex.Length % 2 == 1)
            hex = hex.Insert(hex.Length - 1, "0");

        var arr = new byte[hex.Length >> 1];

        for (var i = 0; i < hex.Length >> 1; ++i)
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));

        return arr;
    }

    public static string ToSnakeCase(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length < 2)
            return text;

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (var i = 1; i < text.Length; ++i)
        {
            var c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static string Sha256(this string input)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(input)).ToHexString();
    }
}