namespace Quantrum.Abstractions.Entities;

public sealed class QTestCaseResult
{
    public string Source { get; }
    public string ClassName { get; }
    public string MethodName { get; }
    public bool IsPassed { get; }
    public Exception? Exception { get; }
    public TimeSpan Duration { get; }

    public string DisplayName => $"{ClassName}.{MethodName}";

    private QTestCaseResult(
        string source,
        string className,
        string methodName,
        bool isPassed,
        TimeSpan duration,
        Exception? exception)
    {
        Source = source;
        ClassName = className;
        MethodName = methodName;
        IsPassed = isPassed;
        Duration = duration;
        Exception = exception;
    }

    public static QTestCaseResult Passed(string source, string className, string methodName, TimeSpan duration) => 
        new(source, className, methodName, true, duration, null);

    public static QTestCaseResult Failed(string source, string className, string methodName, TimeSpan duration, Exception exception) => 
        new(source, className, methodName, false, duration, exception);
}
