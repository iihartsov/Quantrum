using Quantrum.Abstractions.Enums;

namespace Quantrum.Engine.Execution.Queue;

public sealed class QueueBuilder<T>
{
    public IReadOnlyCollection<ExecutionQueue<T>> Build(
        IEnumerable<T> testCases,
        Func<T, string> sourceSelector,
        Func<T, ParallelScope> scopeSelector)
    {
        return testCases
            .GroupBy(testCase => (Source: sourceSelector(testCase), Scope: scopeSelector(testCase)))
            .Select(group => new ExecutionQueue<T>(
                group.Key.Source,
                group.Key.Scope,
                group
                    .ToList()))
            .ToList()
            .AsReadOnly();
    }
}
