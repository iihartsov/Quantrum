using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Parallelization;

public sealed class ClassExecution : IExecutionStrategy
{
    public ParallelScope Scope => ParallelScope.Class;

    public async Task Execute(
        IReadOnlyCollection<QTestCase> testsCases,
        Func<QTestCase, Task> execute,
        ParallelOptions options)
    {
        var groups = testsCases.GroupBy(testCase => testCase.ClassName);
        
        await Parallel.ForEachAsync(groups, options, async (group, cancellationToken) =>
        {
            foreach (var test in group)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await execute(test);
            }
        });
    }
}
