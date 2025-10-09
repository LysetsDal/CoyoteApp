using Microsoft.Coyote.SystematicTesting;
using Xunit;

namespace CoyoteApp.DataRaces;

public class DataRacesTest
{
    [Test]
    [Fact]
    public static async Task Test_DataRace_Turnstile()
    {
        const int DELAY = 1000;
        var turnstile = new Turnstile();

        var task1 = Task.Run(() => turnstile.PassThrough(DELAY));
        var task2 = Task.Run(() => turnstile.PassThrough(DELAY));

        await Task.WhenAll(task1, task2);
        
        Assert.Equal(2, turnstile.count);
    }
}