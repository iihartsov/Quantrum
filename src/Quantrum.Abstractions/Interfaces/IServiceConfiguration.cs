using Microsoft.Extensions.DependencyInjection;

namespace Quantrum.Abstractions.Interfaces;

public interface IServiceConfiguration
{
    void AddServices(IServiceCollection services);
    
    void AddPipeline(IPipelineBuilder pipeline);
}
