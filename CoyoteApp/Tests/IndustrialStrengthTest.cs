using Microsoft.Coyote.SystematicTesting;
using Xunit;
using Xunit.Abstractions;

namespace CoyoteApp.IndustrialStrength;

public class IndustrialStrengthTest(ITestOutputHelper logger)
{
    /// <summary>
    /// Synchronous Unit test.
    /// </summary>
    [Fact(DisplayName = "Test_Concatenator")]
    public async Task Test_Concatenator()
    {
        var list = new List<string>();
        Concatenator concatenator = new Concatenator(list);
        Task t1 = concatenator.SendMessageHelper("a");
        Task t2 = concatenator.SendMessageHelper("b");
        await Task.WhenAll(t1, t2);
        
        
        Assert.Equal(100, concatenator._list.Count); // holds 
    }
    
    /// <summary>
    /// Concurrent Coyote test run with default Configuration see <see cref="ConfigurationFactory"/>. 
    /// </summary>
    [Test]
    [Fact(DisplayName = "CoyoteTest_UnsafePublication")]
    public async Task CoyoteTest_UnsafePublication()
    {
        var conf = ConfigurationFactory.GetDefaultConfiguration();
        var engine = EngineUtils.GetDefaultTestEngine(conf, Test_Concatenator, logger);
        EngineUtils.EmitTestRunReport(engine, logger);
        
        var reportText = engine.TestReport.GetText(conf, "[CONCAT] ");
            
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
    
}
