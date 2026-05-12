using Microsoft.Extensions.DependencyInjection;

namespace Quantrum.Abstractions.Interfaces;

public interface IScopeFactory
{
    AsyncServiceScope Create();
}
