using System.Collections.Concurrent;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoyoteApp.IndustrialStrength;

public class IndustrialStrengthTest
{
    /// <summary>
    /// Synchronous Unit test.
    /// </summary>
    [Fact]
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
    /// Concurrent Coyote test run with default Configuration see <see cref="Utils"/>. 
    /// </summary>
    [Fact]
    public async Task CoyoteTest_UnsafePublication()
    {
        var conf = Utils.GetDefaultConfiguration();
        var engine = TestingEngine.Create(conf, Test_Concatenator);
        engine.Run();
        var reportText = engine.TestReport.GetText(conf, "[CONCAT] "); ;
            
        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }
    
}