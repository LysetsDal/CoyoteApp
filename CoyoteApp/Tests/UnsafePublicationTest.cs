using CoyoteApp;
using Microsoft.Coyote.SystematicTesting;
using Xunit;
using Xunit.Abstractions;

namespace Tests;


public class UnsafePublicationTest(ITestOutputHelper output)
{
    /// <summary>
    /// Synchronous Unit test for <see cref="UnsafeInheritance"/>
    /// </summary>
    [Fact(DisplayName = "Test_UnsafePub_Inheritance")]
    public async Task Test_UnsafePub_Inheritance()
    {
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
        var conf = ConfigurationFactory.GetDefaultConfiguration_10000();
        var engine = TestingEngine.Create(conf, Test_UnsafePub_Inheritance);
        engine.Run();

        var reportText = engine.TestReport.GetText(conf, "[UNSAFE_PUB] ");
        
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
    
    /* ============================================================== */

    public class ThreadData
    {
        public int x;
        // public int x = 42;

        public ThreadData()
        {
            x = 42;
        }
        
    }
    
    [Fact]
    public async Task Test_UnsafePub_ThreadData_Reproducible()
    {
        ThreadData? data = null;
        int observed = -1;

        var threadCreate = Task.Run(() =>
        {
            data = new ThreadData(); // publishes without synchronization
        });

        var threadRead = Task.Run(() =>
        {
            while (data is null) {}
            observed = data.x; // could see 0 if constructor not visible
            output.WriteLine("TR: " + observed);
            Assert.NotEqual(0, observed);
        });

        await Task.WhenAll(threadCreate, threadRead);
        Assert.Equal(42, observed); // May fail (though rarely)
    }
    
    /// <summary>
    /// Synchronous Unit test for <see cref="ThreadData"/>
    /// </summary>
    [Fact(DisplayName = "Test_UnsafePub_ThreadData")]
    public async Task Test_UnsafePub_ThreadData()
    {

        ThreadData? data = null;

        var threadCreate = Task.Run(() =>
        {
            data = new ThreadData();
        });
        
        var threadSet = Task.Run(() =>
        {
            while (data is null) {}
            data.x = 42;

        });
        
        var threadRead1 = Task.Run(() =>
        {
            while (data is null) {}
            var read = data.x;
            output.WriteLine("TR1: " + read);
            Assert.Equal(42, read);
            return read;
        });        
        
        var threadRead2 = Task.Run(() =>
        {
            while (data is null) {}

            var read = data.x;
            output.WriteLine("TR2: " + read);
            Assert.Equal(42, read);
            return read;
        });

        await Task.WhenAll(threadCreate, threadSet, threadRead1, threadRead2);

        Assert.NotNull(data);
        Assert.Equal(42, threadRead1.Result);
        Assert.Equal(42, threadRead2.Result);
    }
    
    /// <summary>
    /// Synchronous repeated Unit test for <see cref="ThreadData"/>
    /// </summary>
    [Fact(DisplayName = "Test_UnsafePub_ThreadData_X")]
    public async Task Test_UnsafePub_ThreadData_X()
    {
        const int ITERATIONS = 10_000;
        for (var i = 0; i < ITERATIONS; i++)
        {
            await Test_UnsafePub_ThreadData();
            output.WriteLine("Iteration: " + i);
        }
    }

    /// <summary>
    /// Concurrent Coyote test for <see cref="ThreadData"/>, run with default Configuration.
    /// </summary>
    [Test]
    [Fact(DisplayName = "CoyoteTest_UnsafePub_ThreadData")]
    public async Task CoyoteTest_UnsafePub_ThreadData()
    {
        var conf = ConfigurationFactory.GetDefaultConfiguration_NoDeadlocks();
        var engine = EngineFactory.GetDefaultTestEngine(conf, Test_UnsafePub_ThreadData_Reproducible, output);

        var reportText = engine.TestReport.GetText(conf, "[UNSAFE_PUB] ");
        var report = engine.ReadableTrace;
        
        output.WriteLine(report);
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
}
