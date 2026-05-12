using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Quantrum.Engine.Extensions;
using Quantrum.TestAdapter.Extensions;

namespace Quantrum.TestAdapter.VSTest;

[ExtensionUri(VSTestAdapter.UriString)]
public sealed class TestExecutor : ITestExecutor
{
    private readonly VSTestAdapter _adapter;
    private CancellationTokenSource? _cancellationTokenSource;
    
    public TestExecutor()
    {
        var provider = new ServiceCollection()
            .AddEngine()
            .AddVSTestAdapter()
            .BuildServiceProvider();

        _adapter = provider.GetRequiredService<VSTestAdapter>();
    }

    public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        if (tests is null || frameworkHandle is null)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();

        _adapter.RunTests(tests, frameworkHandle, _cancellationTokenSource.Token).GetAwaiter().GetResult();
    }

    public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
    {
        if (sources is null || frameworkHandle is null)
        {
            return;
        }

        var tests = sources
            .SelectMany(TestDiscoverer.DiscoverInternal)
            .ToList();

        RunTests(tests, runContext, frameworkHandle);
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }
}
