using System.Runtime.Versioning;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

namespace CoyoteApp.Tests;


public class UnsafePublicationTest
{

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