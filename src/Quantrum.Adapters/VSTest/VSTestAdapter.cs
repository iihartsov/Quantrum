using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;
using Quantrum.Engine.Execution.Queue;
using Quantrum.Engine.Execution.Runner;
using Quantrum.Engine.Helpers;

namespace Quantrum.TestAdapter.VSTest;

public sealed class VSTestAdapter
{
    public const string UriString = "executor://quantrum/v1";

    private readonly QueueBuilder<TestCase> _queueBuilder;
    private readonly QTestRunner _testRunner;
    private readonly TestReporter _reporter;

    public VSTestAdapter(QueueBuilder<TestCase> queueBuilder, QTestRunner testRunner, TestReporter reporter)
    {
        _queueBuilder = queueBuilder;
        _testRunner = testRunner;
        _reporter = reporter;
    }

    public async Task RunTests(
        IEnumerable<TestCase> testCases,
        IFrameworkHandle frameworkHandle,
        CancellationToken cancellationToken)
    {
        var options = new ParallelOptions
        {
            CancellationToken = cancellationToken
        };

        var queue = _queueBuilder
            .Build(
                testCases,
                testCase => testCase.Source,
                testCase => (ParallelScope)testCase.GetPropertyValue(
                    TestProperties.ParallelScope,
                    (int)ParallelScope.None))
            .Select(queue => new ExecutionQueue<QTestCase>(
                queue.Source,
                queue.Scope,
                queue.TestCases
                    .Select(testCase => new QTestCase(
                        testCase.Source,
                        testCase.GetPropertyValue(TestProperties.ClassName, string.Empty),
                        testCase.GetPropertyValue(TestProperties.MethodName, string.Empty)))
                    .ToList()))
            .ToList()
            .AsReadOnly();

        await _testRunner.Execute(queue, options, result => _reporter.Report(result, frameworkHandle));
    }
}
