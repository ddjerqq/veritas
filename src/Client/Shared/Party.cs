using System.Globalization;

namespace Client.Shared;

public record Party(int Id)
{
    public string LogoPath => $"/assets/{this.GetShortName()}-logo.webp";

    public string LeaderPortraitPath => $"/assets/{this.GetShortName()}-leader.webp";

    public string LeaderGoldenPortraitPath => $"/assets/{this.GetShortName()}-leader-gold.webp";

    public override int GetHashCode() => Id.GetHashCode();
}

public static class PartyExt
{
    public static string GetShortName(this Party party) => party.Id switch
    {
        5 => "unm",
        9 => "lelo",
        36 => "girchi",
        42 => "gd",
        _ => throw new ArgumentOutOfRangeException(nameof(party)),
    };

    public static string GetName(this Party party) => party.Id switch
    {
        5 => "ერთიანი ნაციონალური მოძრაობა",
        9 => "ლელო საქართველოსთვის",
        36 => "გირჩი",
        42 => "ქართული ოცნება",
        _ => throw new ArgumentOutOfRangeException(nameof(party)),
    };

    public static string GetLeaderName(this Party party) => party.Id switch
    {
        5 => "მიხეილ სააკაშვილი",
        9 => "მამუკა ხარაძე",
        36 => "ზურაბ გირჩი ჯაფარიძე",
        42 => "ბიძინა ივანიშვილი",
        _ => throw new ArgumentOutOfRangeException(nameof(party)),
    };

    public static string GetColor(this Party party) => party.Id switch
    {
        5 => "#ce2121",
        9 => "#d4a700",
        36 => "#317e38",
        42 => "#0b6abe",
        _ => throw new ArgumentOutOfRangeException(nameof(party)),
    };

    public static (byte r, byte g, byte b) GetColorRgb(this Party party)
    {
        var color = party.GetColor();
        var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
        var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
        var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);
        return (r, g, b);
    }

    public static string GetTextClass(this Party party)
    {
        // for tailwind class discovery
        _ = "text-unm text-lelo text-girchi text-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"text-{party.GetShortName()}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetBgClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:bg-unm hover:bg-lelo hover:bg-girchi hover:bg-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"hover:bg-{party.GetShortName()}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetShadowClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:shadow-unm hover:shadow-lelo hover:shadow-girchi hover:shadow-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"hover:shadow-{party.GetShortName()}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }
}