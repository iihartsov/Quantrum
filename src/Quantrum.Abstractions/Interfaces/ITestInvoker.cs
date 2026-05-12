using System.Reflection;

namespace Quantrum.Abstractions.Interfaces;

public interface ITestInvoker
{
    Task Invoke(MethodInfo method, object instance, IServiceProvider provider);
}
