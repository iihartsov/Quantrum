using Quantrum.Abstractions.Entities;

namespace Quantrum.Abstractions.Interfaces;

public interface ITestEndpointResolver
{
    QTestEndpoint Resolve(QTestCase testCase);
}
