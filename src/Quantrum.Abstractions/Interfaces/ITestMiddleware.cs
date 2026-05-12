using Quantrum.Abstractions.Entities;

namespace Quantrum.Abstractions.Interfaces;

public interface ITestMiddleware
{
    Task Invoke(QTestContext context, Func<Task> next);
}
