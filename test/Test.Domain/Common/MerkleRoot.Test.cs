using System.Security.Cryptography;
using Domain.Common;

namespace Test.Domain.Common;

internal class TestMerkleRoot
{
    [Test]
    [Parallelizable]
    public void TestMerkleRootOnPredeterminedHashes()
    {
        List<byte[]> txHashes = [
            SHA256.HashData("aaa"u8.ToArray()).ToArray(),
            SHA256.HashData("bbb"u8.ToArray()).ToArray(),
            SHA256.HashData("ccc"u8.ToArray()).ToArray(),
            SHA256.HashData("ddd"u8.ToArray()).ToArray(),
        ];

        var merkleRootDigest = MerkleRoot
            .BuildMerkleRoot(txHashes)
            .ToHexString();

        Console.WriteLine(merkleRootDigest);

        var expected = "20d91ce8e5b46488788bee6b7b2dec6216168c5bf2e1dc484be420bad8462aa9";
        Assert.That(merkleRootDigest, Is.EqualTo(expected));
    }

    [Test]
    [Parallelizable]
    public void TestMerkleRootOnRandomTransactions()
    {
        var txHashes = new List<byte[]>();

        for (int i = 0; i < 100; i++)
        {
            var txHash = new byte[32];
            RandomNumberGenerator.Fill(txHash);
            txHashes.Add(txHash);
        }

        var merkleRootDigest = MerkleRoot
            .BuildMerkleRoot(txHashes)
            .ToHexString();

        Console.WriteLine($"Computed Merkel Root: {merkleRootDigest}");

        Assert.That(merkleRootDigest, Is.Not.Empty);
    }
}