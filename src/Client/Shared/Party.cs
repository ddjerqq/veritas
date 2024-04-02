namespace Client.Shared;

public record Party(int Id, string Name, string ShortName, string LeaderName, string ColorClass, string ShadowClass)
{
    public string LogoPath => $"/assets/{ShortName}-logo.webp";

    public string LeaderPortraitPath => $"/assets/{ShortName}-leader.webp";

    public string LeaderGoldenPortraitPath => $"/assets/{ShortName}-leader-gold.webp";
}