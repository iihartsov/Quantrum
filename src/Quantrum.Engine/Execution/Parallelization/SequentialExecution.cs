using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Parallelization;

public sealed class SequentialExecution : IExecutionStrategy
{
    public ParallelScope Scope => ParallelScope.None;

    public async Task Execute(
        IReadOnlyCollection<QTestCase> testsCases,
        Func<QTestCase, Task> execute,
        ParallelOptions options)
    {
        foreach (var test in testsCases)
        {
            options.CancellationToken.ThrowIfCancellationRequested();
            
            await execute(test);
        }
    }
}
