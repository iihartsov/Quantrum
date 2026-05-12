using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Parallelization;

public sealed class AssemblyExecution : IExecutionStrategy
{
    public ParallelScope Scope => ParallelScope.Assembly;

    public async Task Execute(
        IReadOnlyCollection<QTestCase> testsCases,
        Func<QTestCase, Task> execute,
        ParallelOptions options)
    {
        await Parallel.ForEachAsync(testsCases, options, async (test, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await execute(test);
        });
    }
}
