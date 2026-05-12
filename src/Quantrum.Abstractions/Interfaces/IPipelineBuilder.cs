using Quantrum.Abstractions.Entities;

namespace Quantrum.Abstractions.Interfaces;

public interface IPipelineBuilder
{
    IPipelineBuilder Use(Func<QTestContext, Func<Task>, Task> pipeline);
    IPipelineBuilder Use<T>() where T : ITestMiddleware;
    Func<QTestContext, Task> Build();
}
