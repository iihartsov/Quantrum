using Quantrum.Abstractions.Enums;

namespace Quantrum.Engine.Execution.Queue;

public sealed record ExecutionQueue<T>(string Source, ParallelScope Scope, IReadOnlyCollection<T> TestCases);
