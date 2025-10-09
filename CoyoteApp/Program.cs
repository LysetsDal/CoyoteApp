using System;
using System.Threading.Tasks;
using CoyoteApp.IndustrialStrength;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using Xunit;

// ReSharper disable MemberCanBePrivate.Global

namespace CoyoteApp
{

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
        public static async Task Test_UnsafePublication()
        {
            // Construct the object
            UnsafePublicationParent unsafePublicationParent = null;

            // Call the objects method 
            var task = Task.Run(() =>
            {
                unsafePublicationParent = new UnsafePublicationParent();
                return unsafePublicationParent.getChildObjectAsync();
            });

            // Assert ...
            Assert.False(string.IsNullOrEmpty(task.Result.getName()));
        }





        

    }
}