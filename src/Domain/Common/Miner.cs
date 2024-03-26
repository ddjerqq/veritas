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

        long nonce = 0;

        while (true)
        {
            ReadOnlySpan<byte> nonceBytes = BitConverter.GetBytes(nonce);
            nonceBytes.CopyTo(payload.Slice(destOffset, sizeof(long)));
            ReadOnlySpan<byte> hash = SHA256.HashData(payload);

            if (hash.Slice(0, halfDifficulty).SequenceEqual(predicate))
            {
                return nonce;
            }

            nonce++;
        }
    }
}