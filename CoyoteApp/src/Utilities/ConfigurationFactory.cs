using System.Runtime.InteropServices.JavaScript;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

namespace CoyoteApp;

public static class ConfigurationFactory
{
    /// <summary>
    /// Default test configuration. It has a max cap of 100_000 scheduling steps. Exploration of
    /// memory-access race conditions enabled. And uses a probabilistic priority-based exploration strategy.
    /// </summary>
    /// <returns> Our default coyote test configuration for the engine to run.</returns>
    public static Configuration GetDefaultConfiguration()
    {
        return Configuration.Create()
                .WithConsoleLoggingEnabled()            // Show Iteration prints
                // .WithVerbosityEnabled()              // Debug All Interleaving traces   
                .WithMaxSchedulingSteps(10_000)         // Max scheduling steps (i.e. decisions) to be explored during testing.
                .WithMemoryAccessRaceCheckingEnabled()  // Enable exploration of race conditions at memory-access locations.
                .WithPrioritizationStrategy()           // A (fair) probabilistic priority-based exploration strategy. 
                .WithActorTraceVisualizationEnabled();
    }
    
    /// <summary>
    /// Default test configuration with 100 iterations.
    /// </summary>
    /// <returns> The default test configuration, run 100 times </returns>
    public static Configuration GetDefaultConfiguration_100()
    {
        return GetDefaultConfiguration()
            .WithTestingIterations(100);
    }
    
    /// <summary>
    /// Default test configuration with 1000 iterations.
    /// </summary>
    /// <returns> The default test configuration, run 1000 times </returns>
    public static Configuration GetDefaultConfiguration_1000()
    {
        return GetDefaultConfiguration()
            .WithTestingIterations(1_000);
    }
    
    /// <summary>
    /// Default test configuration with 10000 iterations.
    /// </summary>
    /// <returns> The default test configuration, run 10000 times </returns>
    public static Configuration GetDefaultConfiguration_10000()
    {
        return GetDefaultConfiguration()
            .WithTestingIterations(10_000);
    }
    
    public static Configuration GetDefaultConfiguration_NoDeadlocks()
    {
        return GetDefaultConfiguration_10000()
            .WithPotentialDeadlocksReportedAsBugs(false)
            .WithDeadlockTimeout(10_000);
    }
}
