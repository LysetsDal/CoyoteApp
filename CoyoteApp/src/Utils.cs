using System.Runtime.InteropServices.JavaScript;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

namespace CoyoteApp;

public class Utils
{
    public static Configuration GetDefaultConfiguration()
    {
        return Configuration.Create()
            .WithTestingIterations(100)
            .WithMaxSchedulingSteps(100000)
            .WithMemoryAccessRaceCheckingEnabled()
            .WithPrioritizationStrategy()
            .WithActorTraceVisualizationEnabled();
    }
    
    
}