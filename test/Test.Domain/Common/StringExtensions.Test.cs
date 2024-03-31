using Domain.Common;

namespace Test.Domain.Common;

internal class StringExtTest
{
    [Test]
    [Parallelizable]
    public void TestToHexBytesEven()
    {
        var hex = "ffff";
        var bytes = hex.ToBytesFromHex();
        Assert.That(bytes, Is.EqualTo(new byte[] { 255, 255 }));
    }

    [Test]
    [Parallelizable]
    public void TestToHexBytesOdd()
    {
        var hex = "fffff";
        var bytes = hex.ToBytesFromHex();
        Assert.That(bytes, Is.EqualTo(new byte[] { 255, 255, 15 }));
    }

    [Test]
    [Parallelizable]
    public void TestToSnakeCase()
    {
        var snake = "HelloWorld".ToSnakeCase();
        Assert.That(snake, Is.EqualTo("hello_world"));
    }
}