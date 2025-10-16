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
        private readonly ITestOutputHelper _output;

        public Program(ITestOutputHelper output)
        {
            _output = output;
        }

        public static async Task Main(string[] args)
        {
            TestOutputHelper output = new TestOutputHelper();
            Program p = new Program(output);
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
            // Call the objects method 
            var t1 = Task.Run(EscapingInstance.getInstance);
            var t2 = Task.Run(EscapingInstance.getInstance);
            
            await Task.WhenAll(t1, t2);

            var instance1 = t1.Result;
            var instance2 = t2.Result;
            
            // Assert
            
            _output.WriteLine("T1: " + instance1.hashcode);
            _output.WriteLine("T2: " + instance2.hashcode);
            
            Assert.NotSame(instance1, instance2);
        }
        
        [Fact]
        public async Task CoyoteTest_UnsafePublication()
        {
            var conf = Utils.GetDefaultConfiguration();
            var engine = TestingEngine.Create(conf, Test_UnsafePublication);
            engine.Run();
            
            var reportText = engine.TestReport.GetText(conf, "@SAFE-PUBLICATION: "); ;
            
            Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
        }
        
    }
}