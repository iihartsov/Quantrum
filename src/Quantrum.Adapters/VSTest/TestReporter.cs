using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Quantrum.Abstractions.Entities;

namespace Quantrum.TestAdapter.VSTest;

public sealed class TestReporter
{
    public Task Report(QTestCaseResult caseResult, IFrameworkHandle frameworkHandle)
    {
        var testCase = new TestCase(caseResult.DisplayName, new Uri(VSTestAdapter.UriString), caseResult.Source);

        frameworkHandle.RecordStart(testCase);

        var outcome = caseResult.IsPassed
            ? TestOutcome.Passed
            : TestOutcome.Failed;

        frameworkHandle.RecordResult(new TestResult(testCase)
        {
            Outcome = outcome,
            Duration = caseResult.Duration,
            ErrorMessage = caseResult.Exception?.Message,
            ErrorStackTrace = caseResult.Exception?.StackTrace
        });

        frameworkHandle.RecordEnd(testCase, outcome);

        return Task.CompletedTask;
    }
}
