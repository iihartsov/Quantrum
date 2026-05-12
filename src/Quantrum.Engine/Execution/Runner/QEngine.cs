using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Runner;

public sealed class QEngine
{
    private readonly IScopeFactory _scope;
    private readonly ITestInvoker _invoker;
    private readonly ITestEndpointResolver _resolver;
    private readonly Func<QTestContext, Task> _pipeline;

    public QEngine(
        IScopeFactory scope,
        ITestInvoker invoker,
        ITestEndpointResolver resolver,
        Func<QTestContext, Task> pipeline)
    {
        _scope = scope;
        _invoker = invoker;
        _resolver = resolver;
        _pipeline = pipeline;
    }

    public async Task<QTestCaseResult> Execute(QTestCase testCase)
    {
        Exception? capturedException = null;
        var endpoint = _resolver.Resolve(testCase);
        var context = new QTestContext
        {
            TestCase = testCase,
            Endpoint = endpoint,
        };

        context.ExecuteTestAsync = async() =>
        {
            await using var scope = _scope.Create();
            var provider = scope.ServiceProvider;
            var instance = ActivatorUtilities.CreateInstance(provider, endpoint.Class);
            
            context.ServiceProvider = provider;
            context.TestInstance = instance;

            try
            {
                await _invoker.Invoke(endpoint.Method, instance, provider);
            }
            catch (Exception ex)
            {
                capturedException = ex;
                throw;
            }
        };

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _pipeline(context);
            
            stopwatch.Stop();
            
            if (capturedException is not null)
            {
                return QTestCaseResult.Failed(
                    testCase.Source, testCase.ClassName, testCase.MethodName, stopwatch.Elapsed, capturedException);
            }
            
            return QTestCaseResult.Passed(
                context.TestCase.Source, context.TestCase.ClassName, context.TestCase.MethodName, stopwatch.Elapsed);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            
            return QTestCaseResult.Failed(
                context.TestCase.Source, context.TestCase.ClassName, context.TestCase.MethodName, stopwatch.Elapsed, exception);
        }
    }
}
