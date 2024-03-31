using System.Security.Cryptography;

namespace Domain.Common;

public static class Miner
{
    private static void CopyWithoutAllocating(long value, Span<byte> dest)
    {
        for (var i = 0; i < sizeof(long); i++)
        {
            dest[i] = (byte)((value >> 8 * i) & 0xFF);
        }
    }

    private static void CopyWithAllocating(long value, Span<byte> dest)
    {
        BitConverter.GetBytes(value).AsSpan().CopyTo(dest);
    }

    public static long Mine(byte[] data, int difficulty)
    {
        int destOffset = data.Length - sizeof(long);
        int halfDifficulty = difficulty / 2;

        ReadOnlySpan<byte> predicate = stackalloc byte[halfDifficulty];
        Span<byte> payload = data;

        Span<byte> nonceBuffer = stackalloc byte[8];
        Span<byte> nonceBufferA = stackalloc byte[8];
        Span<byte> hashBuffer = stackalloc byte[32];

        long nonce = 0;

        while (true)
        {
            CopyWithoutAllocating(nonce, nonceBuffer);

            nonceBuffer.CopyTo(payload.Slice(destOffset, sizeof(long)));
            SHA256.HashData(payload, hashBuffer);

            if (hashBuffer[..halfDifficulty].SequenceEqual(predicate))
            {
                return nonce;
            }

            nonce++;
        }
    }
}