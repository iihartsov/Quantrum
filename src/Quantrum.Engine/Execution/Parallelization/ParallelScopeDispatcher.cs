using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Parallelization;

public sealed class ParallelScopeDispatcher
{
    private readonly IReadOnlyDictionary<ParallelScope, IExecutionStrategy> _strategies;

    public ParallelScopeDispatcher(IReadOnlyDictionary<ParallelScope, IExecutionStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Task Dispatch(
        ParallelScope scope,
        ParallelOptions options,
        IReadOnlyCollection<QTestCase> tests,
        Func<QTestCase, Task> execute)
    {
        return _strategies.TryGetValue(scope, out var strategy)
            ? strategy.Execute(tests, execute, options)
            : throw new InvalidOperationException($"Parallel Scope - {scope} is not supported");
    }
}
