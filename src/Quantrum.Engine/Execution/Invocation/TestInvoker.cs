using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Invocation;

public class TestInvoker : ITestInvoker
{
    public async Task Invoke(MethodInfo method, object instance, IServiceProvider provider)
    {
        var args = method
            .GetParameters()
            .Select(p => provider.GetRequiredService(p.ParameterType))
            .ToArray();
        
        var result = method.Invoke(instance, args);

        if (result is Task task)
        {
            await task;
        }
    }
}
