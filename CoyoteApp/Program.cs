using System;
using System.Threading.Tasks;
using CoyoteApp.IndustrialStrength;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

// ReSharper disable MemberCanBePrivate.Global

namespace CoyoteApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
        }

        foreach (var arg in args)
        {
            if (arg[0] == '-')
            {
                switch (arg.ToUpperInvariant().Trim('-'))
                {
                    case "S":
                        Console.WriteLine("Running sequential test without Coyote ...");
                        await Test_UnsafePublication();
                        Console.WriteLine("Done.");
                        return;
                    case "C":
                        Console.WriteLine("Running concurrent test without Coyote ...");
                        // await Test_Concurrent_UnsafePublication();
                        Console.WriteLine("Done.");
                        return;
                    case "?":
                    case "H":
                    case "HELP":
                        PrintUsage();
                        return;
                    default:
                        Console.WriteLine("### Unknown arg: " + arg);
                        PrintUsage();
                        return;
                }
            }
        }
    }
    
    private static void PrintUsage()
    {
        Console.WriteLine("Usage: AccountManager [option]");
        Console.WriteLine("Options:");
        Console.WriteLine("  -s    Run sequential test without Coyote");
        Console.WriteLine("  -c    Run concurrent test without Coyote");
    }

    [Test]
    public static async Task Test_AsyncStringConcat()
    {
        IndustrialStrengthTest industryTest = new();
        await industryTest.RunTest();
    }

    [Test]
    public static async Task Test_AsyncTypeInit()
    {
        var asyncType = new AsyncType();
        
        // Call the objects method 
        var task1 = Task.Run(async () =>
        {
            return asyncType.Value;
        });
        
        await Task.WhenAll(task1);
        
        Assert.True(task1.Equals(42));
    }    
    
    [Test]
    public static async Task Test_UnsafePublication()
    {
        // Construct the object
        UnsafePublication unsafePublication = null;

        // Call the objects method 
        var task = Task.Run(() =>
        {
            unsafePublication = new UnsafePublication();
            return unsafePublication.GetChildObject();
        });

        await Task.Delay(1000);
        // Assert ...
        Assert.False(string.IsNullOrEmpty(task.Result.getName()));
    }     
    
    
    
    
    
    
    /* NORMAL UNIT TEST */
    [Fact]
    public static async Task Test_UnsafePublication_Unit()
    {
        // Construct the object
        UnsafePublication unsafePublication = null;

        // Call the objects method 
        var task = Task.Run(() =>
        {
            unsafePublication = new UnsafePublication();
            return unsafePublication.GetChildObject();
        });

        await Task.Delay(100);
        // Assert ...
        Assert.False(string.IsNullOrEmpty(task.Result.getName()));
    }    
    
    [Fact]
    public static async Task Test_UnsafePublication_Unit_Multiple()
    {
        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine($"Running Test_UnsafePublication_Unit iteration {i + 1}");
            await Test_UnsafePublication_Unit();
        }
    }
    
    
    [Fact]
    public static async Task Test_AsyncTypeInit_Unit()
    {
        var asyncType = new AsyncType();
        
        // Call the objects method 
        var task1 = Task.Run(async () =>
        {
            await asyncType.Initialization;
            return asyncType.Value;
        });
        
        await Task.WhenAll(task1);
        
        Assert.Equal(42, task1.Result);
    }   
    
    [Fact]
    public static async Task Test_AsyncTypeInit_Unit_Multiple()
    {
        for (int i = 0; i < 500; i++)
        {
            Console.WriteLine($"Running Test_AsyncTypeInit_Unit iteration {i + 1}");
            await Test_AsyncTypeInit_Unit();
        }
    }
    
}