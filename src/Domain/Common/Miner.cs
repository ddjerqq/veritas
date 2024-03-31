using System.Security.Cryptography;

namespace Domain.Common;

public static class Miner
{
    private static void CopyWithoutAllocating(long value, Span<byte> dest)
    {
        dest[0] = (byte)((value >> 8 * 0) & 0xFF);
        dest[1] = (byte)((value >> 8 * 1) & 0xFF);
        dest[2] = (byte)((value >> 8 * 2) & 0xFF);
        dest[3] = (byte)((value >> 8 * 3) & 0xFF);
        dest[4] = (byte)((value >> 8 * 4) & 0xFF);
        dest[5] = (byte)((value >> 8 * 5) & 0xFF);
        dest[6] = (byte)((value >> 8 * 6) & 0xFF);
        dest[7] = (byte)((value >> 8 * 7) & 0xFF);
    }

    public static long Mine(byte[] data, int difficulty)
    {
        int destOffset = data.Length - sizeof(long);
        int halfDifficulty = difficulty / 2;

        ReadOnlySpan<byte> predicate = stackalloc byte[halfDifficulty];
        Span<byte> payload = data;

        Span<byte> nonceBuffer = stackalloc byte[sizeof(long)];
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