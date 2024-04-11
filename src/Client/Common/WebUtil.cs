using System.Globalization;

namespace Client.Common;

public static class WebUtil
{
    public static int GetGradientDegrees(string address) => int.Parse(address[2..8], NumberStyles.HexNumber) * 2047 % 360;
    public static int GetGradientDegrees(long index) => (int)(index * 2047 % 360);
}