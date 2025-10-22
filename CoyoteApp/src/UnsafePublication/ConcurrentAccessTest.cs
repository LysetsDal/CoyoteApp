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

namespace CoyoteApp.src.UnsafePublication
{
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
                obj.x = 42;
            });
            Task reader = Task.Run(() =>
            {
                while (obj == null);
                int value = obj.x;
                // The value read here may be 0 due to unsafe publication
                _output.WriteLine($"Read value: {value}");
                Assert.Equal(42, value);

            });
            await Task.WhenAll(creator, writer, reader);

            Assert.Equal(42, obj.x);
        }

        [Fact(DisplayName = "Test_UnsafePub_Inheritance_X")]
        public async Task Test_UnsafePub_Inheritance_X()
        {
            const int ITERATIONS = 10;
            for (var i = 0; i < ITERATIONS; i++)
            {
                await TestUnsafePublication();
                _output.WriteLine("Iteration: " + i);
            }
        }

    }

    public class UnsafePublication
    {
        public UnsafePublication()
        {
                
        }
        public int x;
    }
}
