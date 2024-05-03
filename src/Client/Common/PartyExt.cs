using System.Globalization;
using Domain.ValueObjects;

namespace Client.Common;

public static class PartyExt
{
    public static string GetLogoPath(this Party party) => $"/assets/{party.ShortName}-logo.webp";

    public static string GetLeaderPortraitPath(this Party party) => $"/assets/{party.ShortName}-leader.webp";

    public static string GetLeaderGoldenPortraitPath(this Party party) => $"/assets/{party.ShortName}-leader-gold.webp";

    public static (byte r, byte g, byte b) GetColorRgb(this Party party)
    {
        var color = party.Color;
        var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
        var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
        var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);
        return (r, g, b);
    }

    public static string GetTextClass(this Party party)
    {
        // for tailwind class discovery
        _ = "text-strategy-agmashenebeli text-euro-georgia text-unm text-euro-democrats text-citizens " +
            "text-law-and-order text-lelo text-girchi-iago text-gd text-girchi-zurab";
        return party.Id switch
        {
            0 => "no_party",
            var id when Party.Allowed.Contains(id) => $"text-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetBorderClass(this Party party)
    {
        // for tailwind class discovery
        _ = "border-strategy-agmashenebeli border-euro-georgia border-unm border-euro-democrats border-citizens " +
            "border-law-and-order border-lelo border-girchi-iago border-gd border-girchi-zurab";
        return party.Id switch
        {
            0 => "no_party",
            var id when Party.Allowed.Contains(id) => $"border-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetHoverBgClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:bg-strategy-agmashenebeli hover:bg-euro-georgia hover:bg-unm hover:bg-euro-democrats " +
            "hover:bg-citizens hover:bg-law-and-order hover:bg-lelo hover:bg-girchi-iago hover:bg-gd hover:bg-girchi-zurab";
        return party.Id switch
        {
            0 => "no_party",
            var id when Party.Allowed.Contains(id) => $"hover:bg-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }

    public static string GetHoverShadowClass(this Party party)
    {
        // for tailwind class discovery
        _ = "hover:shadow-strategy-agmashenebeli hover:shadow-euro-georgia hover:shadow-unm " +
            "hover:shadow-euro-democrats hover:shadow-citizens hover:shadow-law-and-order hover:shadow-lelo " +
            "hover:shadow-girchi-iago hover:shadow-gd hover:shadow-girchi-zurab";
        return party.Id switch
        {
            0 => "no_party",
            var id when Party.Allowed.Contains(id) => $"hover:shadow-{party.ShortName}",
            _ => throw new ArgumentOutOfRangeException(nameof(party)),
        };
    }
}