using System.Diagnostics.CodeAnalysis;

namespace Client.Common;

public static class StringExt
{
    /// <summary>
    /// Left joins all classes with the `other` class list,
    /// and favors ones from self
    /// </summary>
    public static string Join(this string classes, [StringSyntax("html")] string other)
    {
        var selfClasses = classes.Split(' ');
        var otherClasses = other.Split(' ');

        // use hash sets
        var set = new HashSet<string>(selfClasses);
        foreach (var c in otherClasses)
        {
            set.Add(c);
        }

        return string.Join(' ', set);
    }
}