using Xunit.Abstractions;

namespace CoyoteApp.IndustrialStrength;

public class Concatenator
{
    public List<string> _list;

    public Concatenator(List<string> list)
    {
        _list = list;
    }
    
    
    public Task SendMessageHelper(string prefix)
    {
        return Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                _list.Add(string.Concat(prefix, i));
                await Task.Yield();
            }
        });
    }
    
}