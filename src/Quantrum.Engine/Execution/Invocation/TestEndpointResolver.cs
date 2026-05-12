using System.Reflection;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Invocation;

public class TestEndpointResolver : ITestEndpointResolver
{
    public QTestEndpoint Resolve(QTestCase testCase)
    {
        var assembly = Assembly.LoadFrom(testCase.Source);
        var type = assembly.GetType(testCase.ClassName);

        if (type is null)
        {
            throw new InvalidOperationException($"Type '{testCase.ClassName}' was not found.");
        }

        var method = type.GetMethod(testCase.MethodName);

        if (method is null)
        {
            throw new InvalidOperationException($"Method '{testCase.MethodName}' was not found.");
        }

        return new QTestEndpoint(type, method);
    }
}
