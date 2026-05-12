namespace Quantrum.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestCaseAttribute(string? displayName = null, string? skipReason = null) : Attribute
{
    public string? DisplayName { get; } = displayName;
    public string? SkipReason { get; } = skipReason;
}
