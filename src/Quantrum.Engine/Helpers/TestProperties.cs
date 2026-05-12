using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Quantrum.Engine.Helpers;

public static class TestProperties
{
    public static TestProperty SkipReason { get; } = TestProperty
        .Register("TestCase.SkipReason", "Skip", typeof(string), typeof(TestCase));
    
    public static TestProperty ClassName { get; } = TestProperty
        .Register("TestCase.ClassName", "Class", typeof(string), typeof(TestCase));
    
    public static TestProperty MethodName { get; } = TestProperty
        .Register("TestCase.MethodName", "Method", typeof(string), typeof(TestCase));

    public static TestProperty ParallelScope { get; } = TestProperty
        .Register("TestCase.ParallelScope", "ParallelScope", typeof(int), typeof(TestCase));
}
