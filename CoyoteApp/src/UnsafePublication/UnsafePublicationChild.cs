namespace CoyoteApp;
// ReSharper disable once ConvertToPrimaryConstructor

public class UnsafePublicationChild : IAsyncType
{
    public Task Initialization { get; }
    public int Value { get; set; }
    private string name;
    
    public UnsafePublicationChild(string name)
    {
        Initialization = InitializeAsync();
    }
    private async Task InitializeAsync()
    {
        await Task.Delay(500);
        this.name = "Initialized Async";
    }

    public async Task<String> GetChildObjectAsync()
    {
        return this.name;
    }

    public string getName()
    {
        return this.name;
    }
    
}