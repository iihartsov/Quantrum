using Quantrum.Abstractions.Enums;

namespace Quantrum.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public class ParallelizeAttribute(ParallelScope scope) : Attribute
{
    public ParallelScope Scope { get; } = scope;
}
