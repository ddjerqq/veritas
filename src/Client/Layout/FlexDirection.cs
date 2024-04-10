namespace Client.Layout;

public enum FlexDirection
{
    Row,
    Col,
}

public static class FlexDirectionExt
{
    public static string GetClass(this FlexDirection flex) => flex switch
    {
        FlexDirection.Row => "flex-row",
        FlexDirection.Col => "flex-col",
        _ => throw new ArgumentOutOfRangeException(nameof(flex), flex, null),
    };
}