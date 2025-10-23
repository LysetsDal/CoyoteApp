using Microsoft.Coyote.SystematicTesting;
using Microsoft.Coyote.SystematicTesting.Frameworks.XUnit;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoyoteApp.Tests;

public class ConcurrentAccessTest
{
    private readonly ITestOutputHelper _output;

    public ConcurrentAccessTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task TestUnsafePublication()
    {
        UnsafePublication obj = null;
        Task creator = Task.Run(() =>
        {
            obj = new UnsafePublication();
        });
        Task writer = Task.Run(() =>
        {
            while (obj == null);
            obj.X = 42;
        });
        Task reader = Task.Run(() =>
        {
            while (obj == null);
            int value = obj.X;
            _output.WriteLine($"Read value: {value}");
            Assert.Equal(42, value);

        });
        await Task.WhenAll(creator, writer, reader);

        Assert.Equal(42, obj.X);
    }
    [Fact]
    public async Task TestUnsafeIncrement()
    {
        UnsafePublication obj = new UnsafePublication();
        Object someObj = new Object();
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    obj.Increment();
                }
            }));
        }
        await Task.WhenAll(tasks);
        _output.WriteLine($"Final value: {obj.X}");
        Assert.Equal(100000, obj.X);
    }

    /// <summary>
    /// Concurrent Coyote test for <see cref="UnsafeInheritance"/>, run with default Configuration.
    /// </summary>
    [Test]
    [Fact(DisplayName = "CoyoteTest_UnsafePub_Inheritance")]
    public async Task CoyoteTest_UnsafePub_()
    {
        var conf = Utils.GetDefaultConfiguration_100();
        var engine = TestingEngine.Create(conf, TestUnsafePublication);
        // Ensure Results folder exists
        var dicInfo = Directory.CreateDirectory("./TestResults/");
        engine.Run();
        engine.TryEmitReports(dicInfo.FullName, "test", out IEnumerable<string> reportPaths );
        _output.WriteLine("Report paths: " + string.Join(", ", reportPaths));

        var reportText = engine.TestReport.GetText(conf, "[UNSAFE_PUB] ");

        Assert.True(engine.TestReport.NumOfFoundBugs == 0, reportText);
    }

}

public class UnsafePublication
{
    private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
    private int _x;
    public int X
    {
        get
        {
            cacheLock.EnterReadLock();
            try
            {
                return _x;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
        set
        {
            cacheLock.EnterWriteLock();
            try
            {
                _x = value;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
    }

    public void Increment()
    {
        cacheLock.EnterWriteLock();
        try
        {
            _x++;
        }
        finally
        {
            cacheLock.ExitWriteLock();
        }
    }
}
