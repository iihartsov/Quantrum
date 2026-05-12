using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Quantrum.Engine.Execution.Queue;
using Quantrum.TestAdapter.VSTest;

namespace Quantrum.TestAdapter.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddVSTestAdapter(this IServiceCollection services)
    {
        return services
            .AddSingleton<VSTestAdapter>()
            .AddSingleton<TestReporter>()
            .AddSingleton<QueueBuilder<TestCase>>();
    }
}
