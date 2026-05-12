using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Interfaces;
using Quantrum.Engine.Execution.Invocation;
using Quantrum.Engine.Execution.Lifetime;
using Quantrum.Engine.Execution.Parallelization;
using Quantrum.Engine.Execution.Pipeline;
using Quantrum.Engine.Execution.Runner;

namespace Quantrum.Engine.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddEngine(this IServiceCollection services)
    {
        services.AddSingleton<ITestInvoker, TestInvoker>();
        services.AddSingleton<IScopeFactory, ScopeFactory>();
        services.AddSingleton<IPipelineBuilder, PipelineBuilder>();
        services.AddSingleton<ITestEndpointResolver, TestEndpointResolver>();
        
        services.AddSingleton<Func<QTestContext, Task>>(sp =>
        {
            var builder = sp.GetRequiredService<IPipelineBuilder>();
            return builder.Build();
        });
        
        services.AddSingleton<QEngine>();
        
        services
            .AddSingleton<IExecutionStrategy, AssemblyExecution>()
            .AddSingleton<IExecutionStrategy, ClassExecution>()
            .AddSingleton<IExecutionStrategy, SequentialExecution>();

        services.AddSingleton<ParallelScopeDispatcher>(sp =>
        {
            var strategies = sp
                .GetServices<IExecutionStrategy>()
                .ToDictionary(
                    strategy => strategy.Scope,
                    strategy => strategy);

            return new ParallelScopeDispatcher(strategies);
        });
        
        services.AddSingleton<QTestRunner>();

        return services;
    }
}
