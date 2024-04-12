using System.Globalization;

namespace Client.Common;

public static class WebUtil
{
    public static int GetGradientDegrees(string input)
    {
        return int.Parse(input[2..8], NumberStyles.HexNumber) * 2047 % 360;
    }

    public static int GetGradientDegrees(long index)
    {
        return (int)(index * 2047 % 360);
    }
}