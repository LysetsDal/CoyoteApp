using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.SystematicTesting.Frameworks.XUnit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoyoteApp.DataRaces;

public class DataRacesTest
{
    private ITestOutputHelper _output = new TestOutputHelper();
    
    /// <summary>
    /// Synchronous Unit test for Turnstile.
    /// </summary>
    [Fact]
    public async Task Test_DataRace_Turnstile()
    {
        const int DELAY = 100;
        var turnstile = new Turnstile();

        var task1 = Task.Run(() => turnstile.PassThrough(DELAY));
        var task2 = Task.Run(() => turnstile.PassThrough(DELAY));

        await Task.WhenAll(task1, task2);
        
        Assert.Equal(2, turnstile.count);
    }
    
    /// <summary>
    /// Concurrent Coyote test for Turnstile, run with default Configuration. 
    /// </summary>
    [Fact]
    public async Task CoyoteTest_DataRace_Turnstile()
    {
        var conf = Utils.GetDefaultConfiguration();
        var engine = TestingEngine.Create(conf, Test_DataRace_Turnstile);
        engine.Run();
        
        var reportText = engine.TestReport.GetText(conf, "[DATA_RACE] "); ;
            
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
    
    /// <summary>
    /// Synchronous Unit test for InitializationRace.
    /// </summary>
    [Fact]
    public async Task Test_DataRace_InitializationRace()
    {
        TestOutputLogger output = new TestOutputLogger(_output);
        
        var t1 = Task.Run(InitializationRace.getInstance);
        var t2 = Task.Run(InitializationRace.getInstance);

        Task.WhenAll(t1, t2);

        var instance1 = t1.Result.hashcode;
        var instance2 = t2.Result.hashcode;
        
        output.WriteLine("T1 :" + instance1);
        output.WriteLine("T2 :" + instance2);
        
        Assert.NotEqual(instance1, instance2);
    }
    
    /// <summary>
    /// Concurrent Coyote test for InitializationRace, run with default Configuration. 
    /// </summary>
    [Fact]
    public async Task CoyoteTest_DataRace_InitializationRace()
    {
        var conf = Utils.GetDefaultConfiguration();
        var engine = TestingEngine.Create(conf, Test_DataRace_InitializationRace);
        engine.Run();
        
        var reportText = engine.TestReport.GetText(conf, "[DATA_RACE] "); ;
            
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
}