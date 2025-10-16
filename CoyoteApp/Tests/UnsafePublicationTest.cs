using System.Runtime.Versioning;
using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.SystematicTesting.Frameworks.XUnit;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoyoteApp.Tests;


public class UnsafePublicationTest
{
    private readonly ITestOutputHelper _output = new TestOutputHelper();

    /// <summary>
    /// Synchronous Unit test for <see cref="UnsafeInheritance"/>
    /// </summary>
    [Fact (DisplayName = "Test_UnsafePub_Inheritance")]
    public async Task Test_UnsafePub_Inheritance()
    {
        var logger = new TestOutputLogger(_output);
        
        var t1 = Task.Run(() => new UnsafeInheritanceChild());
        await Task.WhenAll(t1);

        var result = t1.Result;
        logger.WriteLine("Res: " + result.ObservedMessage); 
        
        Assert.NotNull(result.ObservedMessage);
    }
    
    /// <summary>
    /// Synchronous repetaed Unit test for <see cref="UnsafeInheritance"/>
    /// </summary>
    [Fact (DisplayName = "Test_UnsafePub_Inheritance_X")]
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
    [Fact]
    public async Task CoyoteTest_UnsafePub_Inheritance()
    {
        var conf = Utils.GetDefaultConfiguration_10000();
        var forrest = TestingEngine.Create(conf, Test_UnsafePub_Inheritance);
        forrest.Run(); // Run Forrest, Run!

        var reportText = forrest.TestReport.GetText(conf, "[UNSAFE_PUB] ");
        
        Assert.True(forrest.TestReport.NumOfFoundBugs == 0, reportText);
    }
        
        
    [Test]
    [Fact]
    public static async Task Test_UnsafePublication_V1()
    {

        GlobalRegistry._childReference = null;

        var unsafeObserved = false;
        var cts = new CancellationTokenSource();

        var observer = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                var child = GlobalRegistry._childReference;
                if (child != null)
                {
                    var name = child.getName();
                    if (name == null)
                    {
                        unsafeObserved = true;
                        return;
                    }
                }
                
                await Task.Delay(5, cts.Token);
            }
        }, cts.Token);

        var parent = new UnsafePublicationParent();

        await Task.Delay(500);
        
        cts.Cancel();
        try
        {
            await observer;
        }
        catch(TaskCanceledException) {}
        
        Assert.True(unsafeObserved, "Expected unsafe publication, but none was observed");

    }
    
    [Test]
    [Fact]
    public static async Task Test_UnsafePublication_Unit()
    {
        // Construct the object
        UnsafePublicationParent unsafePublicationParent = null;

        // Call the objects method 
        var task = Task.Run(() =>
        {
            unsafePublicationParent = new UnsafePublicationParent();
            return unsafePublicationParent.getChildObjectAsync();
        });

        await Task.Delay(100);
        // Assert ...
        var child = await task;
        Assert.False(string.IsNullOrEmpty(child.getName()));
    }   
    
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task GetChildObject_ShouldReturnValidChild_MultipleTimes(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine($"Running iteration {i + 1}/{iterations}");
            await Test_UnsafePublication_Unit();
        }
    }
}