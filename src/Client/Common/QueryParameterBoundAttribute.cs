namespace Client.Common;

[AttributeUsage(AttributeTargets.Property)]
public sealed class QueryParameterBoundAttribute(string name = default!) : Attribute
{
    public string? Name { get; set; } = name;
}