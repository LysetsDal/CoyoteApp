using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoyoteApp.Tests;

public class ReorderingTest
{
    [Fact]
    public void VolatilePreventsReordering()
    {
        var outputs = new ConcurrentBag<string>();
        // Replace Console.WriteLine with a delegate for testability
        Action<string> output = s => outputs.Add(s);

        // Run Init and Print concurrently many times
        Parallel.For(0, 10000, _ =>
        {
            var obj = new DataInit(output);
            Parallel.Invoke(
                () => obj.Init(),
                () => obj.Print()
            );
        });

        // Assert "0" is never printed when initialized
        foreach (var line in outputs)
        {
            Assert.NotEqual("0", line);
        }
    }
}



/*
 * The DataInit class in Figure 7 has two methods, Init and Print; both may be called from multiple threads. 
 * If no memory operations are reordered, Print can only print “Not initialized” or “42,” but there are two possible cases when Print could print a “0”:

    Write 1 and Write 2 were reordered.
    Read 1 and Read 2 were reordered.
 *If _initialized were not marked as volatile, both reorderings would be permitted. 
 *However, when _initialized is marked as volatile, neither reordering is allowed! 
 *In the case of writes, you have an ordinary write followed by a volatile write, and a volatile write can’t be reordered with a prior memory operation. 
 *In the case of the reads, you have a volatile read followed by an ordinary read, and a volatile read can’t be reordered with a subsequent memory operation.
 *
 * Release-Acquire semantics thus prevent both kinds of reordering in this example.
 * 
 * Note that if the _data field is volatile but _initialized is not, both reorderings would be permitted. As a result, remembering this example is a good way to remember the volatile semantics.
 *
 */

public class DataInit
{
    private int _data = 0;
    private volatile bool _initialized = false;
    private readonly Action<string> _output;
    public DataInit(Action<string> output)
    {
        _output = output;
    }
    public void Init()
    {
        _data = 42;            // Write 1
        _initialized = true;   // Write 2
    }
    public void Print()
    {
        if (_initialized)// Read 1
        {
            _output(_data.ToString());  // Read 2
        }
        else
        {
            _output("Not initialized");
        }
    }
}
