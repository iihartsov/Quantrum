using Quantrum.Abstractions.Entities;
using Quantrum.Engine.Execution.Parallelization;
using Quantrum.Engine.Execution.Queue;

namespace Quantrum.Engine.Execution.Runner;

public class QTestRunner
{
    private readonly QEngine _engine;
    private readonly ParallelScopeDispatcher _dispatcher;

    public QTestRunner(QEngine engine, ParallelScopeDispatcher dispatcher)
    {
        _engine = engine;
        _dispatcher = dispatcher;
    }

    public async Task Execute(
        IReadOnlyCollection<ExecutionQueue<QTestCase>> queues,
        ParallelOptions options,
        Func<QTestCaseResult, Task> completed)
    {
        foreach (var queue in queues)
        {
            await _dispatcher.Dispatch(
                queue.Scope,
                options,
                queue.TestCases,
                async test =>
                {
                    var result = await _engine.Execute(test);
                    
                    await completed(result);
                });
        }
    }
}
