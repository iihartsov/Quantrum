using System.Reflection;

namespace Quantrum.Abstractions.Entities;

public sealed record QTestEndpoint(Type Class, MethodInfo Method);
