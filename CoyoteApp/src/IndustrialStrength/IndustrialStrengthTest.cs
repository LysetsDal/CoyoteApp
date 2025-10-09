using System.Collections.Concurrent;
using Xunit;

namespace CoyoteApp.IndustrialStrength;

public class IndustrialStrengthTest
{
    private List<string> list = new ();
    
    public async Task RunTest()
    {
        Task t1 = SendMessageHelper("a");
        Task t2 = SendMessageHelper("b");
        await Task.WhenAll(t1, t2);
        Assert.Equal(100, list.Count); // holds 
    }

    private Task SendMessageHelper(string prefix)
    {
        return Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                list.Add(string.Concat(prefix, i));
                await Task.Yield();
            }
        });
    }
}