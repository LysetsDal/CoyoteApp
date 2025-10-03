namespace CoyoteApp;

public sealed class AsyncType : IAsyncType
{
    public Task Initialization { get; }

    public int Value { get; set; }

    public AsyncType()
    {
        Initialization = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        // Simulate delay of construction
        await Task.Delay(100);
        Value = 42;
    }
}