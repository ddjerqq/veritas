using Domain.Common;

namespace Test.Domain.Common;

internal class ByteArrayExtTest
{
    [Test]
    [Parallelizable]
    public void TestSha256Hash()
    {
        byte[] bytes = "aaa"u8.ToArray();
        var hash = bytes.Sha256().ToHexString();
        Console.WriteLine(hash);
        Assert.That(hash, Is.EqualTo("9834876dcfb05cb167a5c24953eba58c4ac89b1adf57f28f2f9d09af107ee8f0"));
    }

    [Test]
    [Parallelizable]
    public void TestToBase64String()
    {
        byte[] bytes = "aaa"u8.ToArray();
        var hash = bytes.Sha256().ToBase64String();
        Console.WriteLine(hash);
        Assert.That(hash, Is.EqualTo("mDSHbc+wXLFnpcJJU+uljErImxrfV/KPL50JrxB+6PA="));
    }

    [Test]
    [Parallelizable]
    public void TestArrayEquals()
    {
        byte[] bytes1 = "aaa"u8.ToArray();
        byte[] bytes2 = "aaa"u8.ToArray();
        Assert.That(bytes1.ArrayEquals(bytes2), Is.True);

        bytes2 = "bbb"u8.ToArray();
        Assert.That(bytes1.ArrayEquals(bytes2), Is.False);
        Assert.That(bytes1.ArrayEquals(null), Is.False);
    }
}