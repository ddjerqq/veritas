using System.Security.Cryptography;
using System.Text;
using Domain.Common;

namespace Client.Pages.About;

public record FaqEntry(string Question, string Answer)
{
    public string Id => SHA256.HashData(Encoding.UTF8.GetBytes(Question)).ToHexString()[..8];

    public bool Shown { get; set; }

    public static IEnumerable<FaqEntry> All()
    {
        yield return new FaqEntry("rogor mushaobs mieci.ge?", "kargad!");
        yield return new FaqEntry("rogor mushaobs mieci.ge?", "kargad!");
        yield return new FaqEntry("rogor mushaobs mieci.ge?", "kargad!");
    }
}
