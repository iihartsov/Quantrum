namespace Quantrum.Abstractions.Entities;

public sealed class QTestContext
{
    public object? TestInstance { get; set; }
    public required QTestCase TestCase { get; init; }
    public required QTestEndpoint Endpoint { get; init; }
    public IServiceProvider? ServiceProvider { get; set; }
    public Func<Task>? ExecuteTestAsync { get; set; }
    public Exception? Exception { get; set; }
}
