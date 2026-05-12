using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Enums;

namespace Quantrum.Abstractions.Interfaces;

public interface IExecutionStrategy
{
    ParallelScope Scope { get; }
    
    Task Execute(IReadOnlyCollection<QTestCase> testsCases, Func<QTestCase, Task> execute, ParallelOptions options);
}
