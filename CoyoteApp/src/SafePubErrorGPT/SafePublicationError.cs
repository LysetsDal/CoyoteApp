using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoyoteApp;
public class SafePublicationError
{
    private static Worker _worker; // published without synchronization

    public static async Task Main()
    {
        var t1 = Task.Run(() =>
        {
            // Initialize Worker and assign reference
            _worker = new Worker(42);
        });

        var t2 = Task.Run(() =>
        {
            // Possible data race: reading _worker without synchronization
            if (_worker != null)
            {
                // May observe a partially constructed Worker (safe publication error)
                Console.WriteLine("Value: " + _worker.Value);
            }
        });

        await Task.WhenAll(t1, t2);
    }

    public class Worker
    {
        public int Value;

        public Worker(int value)
        {
            // Simulate construction delay
            Thread.Sleep(10);
            Value = value;
        }
    }
}