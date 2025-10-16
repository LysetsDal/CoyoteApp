using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

namespace CoyoteApp.DataRaces;

public class DataRacesTest
{
    /// <summary>
    /// Synchronous Unit test.
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
    /// Concurrent Coyote test run with default Configuration. 
    /// </summary>
    [Fact]
    public async Task CoyoteTest_DataRace_Turnstile()
    {
        var conf = Utils.GetDefaultConfiguration();
        var engine = TestingEngine.Create(conf, Test_DataRace_Turnstile);
        engine.Run();
        
        var reportText = engine.TestReport.GetText(conf, "[SAFE_PUB] "); ;
            
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
}