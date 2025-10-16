using Xunit.Abstractions;
using Xunit.Sdk;

// ReSharper disable MemberCanBePrivate.Global

namespace CoyoteApp
{
    public class Program(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

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
                            // Add a method 
                            // await 
                            Console.WriteLine("Done.");
                            return;
                        case "C":
                            Console.WriteLine("Running concurrent test without Coyote ...");
                            // Add a method 
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
    }
}