using System.Diagnostics;
using System.Security.Cryptography;
using Domain.Common;

namespace Test.Domain.Common;

internal class MinerTest
{
    [Test]
    [TestCase(2)]
    [TestCase(4)]
    [NonParallelizable]
    public void TestMine(int difficulty)
    {
        Span<byte> payload = new byte[128];
        RandomNumberGenerator.Fill(payload);

        var watch = Stopwatch.StartNew();
        var nonce = Miner.Mine(payload.ToArray(), difficulty);
        watch.Stop();

        BitConverter
            .GetBytes(nonce)
            .CopyTo(payload.Slice(payload.Length - sizeof(long), sizeof(long)));

        var hash = SHA256.HashData(payload);

        Console.WriteLine(nonce.ToString("N0"));
        Console.WriteLine(watch.Elapsed.ToString("c"));
        Console.WriteLine(hash.ToHexString());

        Assert.That(hash.ToHexString()[..difficulty], Is.EqualTo(new string('0', difficulty)));
    }
}