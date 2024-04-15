using System.Globalization;
using Domain.ValueObjects;

namespace Client.Common;

public static class PartyExt
{
    public static string GetLogoPath(this Party party)
    {
        return $"/assets/{party.ShortName}-logo.webp";
    }

    public static string GetLeaderPortraitPath(this Party party)
    {
        return $"/assets/{party.ShortName}-leader.webp";
    }

    public static string GetLeaderGoldenPortraitPath(this Party party)
    {
        return $"/assets/{party.ShortName}-leader-gold.webp";
    }

    public static string GetColor(this Party party)
    {
        return party.Id switch
        {
            5 => "#ce2121",
            9 => "#d4a700",
            36 => "#317e38",
            42 => "#0b6abe",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

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
            5 or 9 or 36 or 42 => $"text-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetBorderClass(this Party party)
    {
        // for tailwind class discovery
        _ = "border-unm border-lelo border-girchi border-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"border-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetHoverBgClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:bg-unm hover:bg-lelo hover:bg-girchi hover:bg-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"hover:bg-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetShadowClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:shadow-unm hover:shadow-lelo hover:shadow-girchi hover:shadow-gd";
        return party.Id switch
        {
            5 or 9 or 36 or 42 => $"hover:shadow-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }
}