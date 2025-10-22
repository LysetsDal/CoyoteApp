using CoyoteApp;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.SystematicTesting.Frameworks.XUnit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Tests;


public class UnsafePublicationTest
{
    private readonly ITestOutputHelper _output = new TestOutputHelper();

    /// <summary>
    /// Synchronous Unit test for <see cref="UnsafeInheritance"/>
    /// </summary>
    [Fact(DisplayName = "Test_UnsafePub_Inheritance")]
    public async Task Test_UnsafePub_Inheritance()
    {
        var output = new TestOutputLogger(_output);

        var t1 = Task.Run(() =>
        {
            var usi = new UnsafeInheritanceDerived();
            output.WriteLine("Final ObservedMessage: " + usi.ObservedMessage);
            return usi;
        });

        await Task.WhenAll(t1);

        var result = t1.Result;
        
        Assert.NotNull(result.ObservedMessage.First());
    }
    
    /// <summary>
    /// Synchronous repeated Unit test for <see cref="UnsafeInheritance"/>
    /// </summary>
    [Fact(DisplayName = "Test_UnsafePub_Inheritance_X")]
    public async Task Test_UnsafePub_Inheritance_X()
    {
        const int ITERATIONS = 10_000;
        var output = new TestOutputLogger(_output);
        for (var i = 0; i < ITERATIONS; i++)
        {
            await Test_UnsafePub_Inheritance();
            output.WriteLine("Iteration: " + i);
        }
    }

    /// <summary>
    /// Concurrent Coyote test for <see cref="UnsafeInheritance"/>, run with default Configuration.
    /// </summary>
    [Test]
    [Fact(DisplayName = "CoyoteTest_UnsafePub_Inheritance")]
    public async Task CoyoteTest_UnsafePub_Inheritance()
    {
        var conf = Utils.GetDefaultConfiguration_10000();
        var engine = TestingEngine.Create(conf, Test_UnsafePub_Inheritance);
        engine.Run();

        var reportText = engine.TestReport.GetText(conf, "[UNSAFE_PUB] ");
        
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
    
}
