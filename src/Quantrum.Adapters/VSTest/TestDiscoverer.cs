using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Quantrum.Abstractions.Attributes;
using Quantrum.Abstractions.Enums;
using Quantrum.Engine.Helpers;

namespace Quantrum.TestAdapter.VSTest;

[FileExtension(".dll")]
[FileExtension(".exe")]
[DefaultExecutorUri(VSTestAdapter.UriString)]
public sealed class TestDiscoverer : ITestDiscoverer
{
    public void DiscoverTests(
        IEnumerable<string> sources,
        IDiscoveryContext discoveryContext,
        IMessageLogger logger,
        ITestCaseDiscoverySink discoverySink)
    {
        foreach (var source in sources)
        {
            try
            {
                foreach (var testCase in DiscoverInternal(source))
                {
                    discoverySink.SendTestCase(testCase);
                }
            }
            catch (Exception ex)
            {
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }
    }

    internal static IEnumerable<TestCase> DiscoverInternal(string source)
    {
        Assembly assembly;

        try
        {
            assembly = Assembly.LoadFrom(source);
        }
        catch
        {
            yield break;
        }

        foreach (var type in GetTypes(assembly))
        {
            foreach (var method in GetTypeMethods(type))
            {
                var attribute = GetAttribute(method);

                if (attribute is null)
                {
                    continue;
                }

                var resolvedScope =
                    method.GetCustomAttribute<ParallelizeAttribute>()?.Scope ??
                    type.GetCustomAttribute<ParallelizeAttribute>()?.Scope ??
                    assembly.GetCustomAttribute<ParallelizeAttribute>()?.Scope ??
                    ParallelScope.Assembly;

                yield return CreateTestCase(type, method, source, attribute, resolvedScope);
            }
        }
    }

    private static TestCase CreateTestCase(
        Type type,
        MethodInfo method,
        string source,
        TestCaseAttribute attribute,
        ParallelScope scope)
    {
        var testCase = new TestCase(BuildTestName(type, method), new Uri(VSTestAdapter.UriString), source);

        testCase.SetPropertyValue(TestProperties.ParallelScope, (int)scope);
        testCase.SetPropertyValue(TestProperties.ClassName, type.FullName);
        testCase.SetPropertyValue(TestProperties.MethodName, method.Name);

        if (attribute.SkipReason is not null)
        {
            testCase.SetPropertyValue(TestProperties.SkipReason, attribute.SkipReason);
        }

        return testCase;
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly) => assembly
        .GetTypes()
        .Where(type =>
             type.IsClass &&
            !type.IsInterface &&
            !type.IsAbstract);

    private static MethodInfo[] GetTypeMethods(Type type) => type
        .GetMethods(BindingFlags.Public | BindingFlags.Instance);

    private static TestCaseAttribute? GetAttribute(MethodInfo method) => method
        .GetCustomAttribute<TestCaseAttribute>();

    private static string BuildTestName(Type type, MethodInfo method) => $"{type.FullName}.{method.Name}";
}
