using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Pipeline;

public sealed class PipelineBuilder : IPipelineBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Func<Func<QTestContext, Task>, Func<QTestContext, Task>>> _pipeline;
    
    public PipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _pipeline = [];
    }

    public IPipelineBuilder Use(Func<QTestContext, Func<Task>, Task> pipeline)
    {
        _pipeline.Add(next => context => pipeline(context, () => next(context)));
        return this;
    }

    public IPipelineBuilder Use<T>() where T : ITestMiddleware => Use(async (context, next) =>
    {
        var pipeline = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        await pipeline.Invoke(context, next);
    });
    
    public Func<QTestContext, Task> Build()
    {
        Func<QTestContext, Task> terminal = context => context.ExecuteTestAsync?.Invoke() ?? Task.CompletedTask;
        
        for (var i = _pipeline.Count - 1; i >= 0; i--)
        {
            terminal = _pipeline[i](terminal);
        }
        
        return terminal;
    }
}
