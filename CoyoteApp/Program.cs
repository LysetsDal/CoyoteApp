using System;
using System.Threading.Tasks;
using CoyoteApp.IndustrialStrength;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

// ReSharper disable MemberCanBePrivate.Global

namespace CoyoteApp
{
    public class Program
    {
        public readonly TestOutputHelper output;
        
        public static async Task Main(string[] args)
        {
            Program p = new Program();
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
                            await p.Test_UnsafePublication();
                            Console.WriteLine("Done.");
                            return;
                        case "C":
                            Console.WriteLine("Running concurrent test without Coyote ...");
                            // await p.Test_Concurrent_UnsafePublication();
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
        
        [Fact]
        public async Task Test_UnsafePublication()
        {
            // Construct the object
            EscapingInstance escapingInstance;

            // Call the objects method 
            var t1 = Task.Run(() =>
            {
                return EscapingInstance.getInstance();
            });
            
            var t2 = Task.Run(() =>
            {
                return EscapingInstance.getInstance();
            });

            Task.WhenAll(t1, t2);

            // Assert
            output.WriteLine("T1: " + t1.Result.x);
            output.WriteLine("T2: " + t2.Result.x);
            Assert.True(t1.Result.x != t2.Result.x);
        }

        [Test]
        public async Task CoyoteTest_UnsafePublication()
        {
            var conf = Configuration.Create().WithTestingIterations(10);
            var engine = TestingEngine.Create(conf, Test_UnsafePublication);
            engine.Run();
        }
        


        

    }
}