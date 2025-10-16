using Xunit.Abstractions;

namespace CoyoteApp.IndustrialStrength;

public class Concatenator
{
    public List<string> list = new ();
    
    
    public Task<int> SendMessageHelper(string prefix)
    {
        return Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                list.Add(string.Concat(prefix, i));
                await Task.Yield();
            }

            return list.Count;
        });
    }
    
}