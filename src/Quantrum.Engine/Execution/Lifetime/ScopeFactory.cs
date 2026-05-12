using Microsoft.Extensions.DependencyInjection;
using Quantrum.Abstractions.Interfaces;

namespace Quantrum.Engine.Execution.Lifetime;

public class ScopeFactory : IScopeFactory
{
    private readonly IServiceProvider _provider;

    public ScopeFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    public AsyncServiceScope Create()
    {
        return _provider.CreateAsyncScope();
    }
}
