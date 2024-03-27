using System.Security.Cryptography;

namespace Domain.Common;

public static class Miner
{
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
            nonceBuffer = BitConverter.GetBytes(nonce).AsSpan();
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