using System.Reflection;
using Moq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Attributes;
using Quantrum.Abstractions.Entities;
using Quantrum.Abstractions.Interfaces;
using Quantrum.Engine.Execution.Runner;

namespace Quantrum.Tests.Unit.Execution;

public class Engine
{
    private readonly Mock<ITestInvoker> _invoker = new();
    private readonly Mock<IScopeFactory> _scopeFactory = new();
    private readonly Mock<ITestEndpointResolver> _resolver = new();

    [TestCase]
    public async Task ExecuteTestCase_Passed()
    {
        var testCase = CreateTestCase();
        var endpoint = CreateEndpoint();

        _resolver.Setup(x => x.Resolve(testCase)).Returns(endpoint);
        
        _invoker
            .Setup(x => x.Invoke(
                It.IsAny<MethodInfo>(),
                It.IsAny<object>(),
                It.IsAny<IServiceProvider>()))
            .Returns(Task.CompletedTask);

        _scopeFactory
            .Setup(x => x.Create())
            .Returns(new ServiceCollection().BuildServiceProvider().CreateAsyncScope());

        var engine = Create(context => context.ExecuteTestAsync!());

        var result = await engine.Execute(testCase);

        using var scope = new AssertionScope();
        result.IsPassed.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.ClassName.Should().Be("TestClass");
        result.MethodName.Should().Be("TestMethod");
    }

    [TestCase]
    public async Task ExecuteTestCase_FailedOnException()
    {
        var testCase = CreateTestCase();
        var endpoint = CreateEndpoint();
        var exception = new InvalidOperationException("fail");

        _resolver.Setup(x => x.Resolve(testCase)).Returns(endpoint);

        _invoker
            .Setup(x => x.Invoke(
                It.IsAny<MethodInfo>(),
                It.IsAny<object>(),
                It.IsAny<IServiceProvider>()))
            .ThrowsAsync(exception);

        _scopeFactory
            .Setup(x => x.Create())
            .Returns(new ServiceCollection().BuildServiceProvider().CreateAsyncScope());

        var engine = Create(async context =>
        {
            try
            {
                await context.ExecuteTestAsync!();
            }
            catch (Exception ex)
            {
                context.Exception = ex;
            }
        });

        var result = await engine.Execute(testCase);
        
        using var scope = new AssertionScope();
        result.IsPassed.Should().BeFalse();
        result.Exception.Should().Be(exception);
    }
    
    private QEngine Create(Func<QTestContext, Task> pipeline) => 
        new(_scopeFactory.Object, _invoker.Object, _resolver.Object, pipeline);

    private static QTestCase CreateTestCase() =>
        new("test.dll", "TestClass", "TestMethod");

    private static QTestEndpoint CreateEndpoint() => 
        new(typeof(Engine), typeof(Engine).GetMethod(nameof(CreateEndpoint))!);
}
