namespace CoyoteApp;

public class UnsafePublication : IAsyncType
{
    private int x { get; set; }
    public ChildObject ChildObject; // ChildObject's method might be called before the constructor is done?

    public Task Initialization { get; }
    public int Value { get; set; }
    
    
    
    
    public UnsafePublication()
    {
        Initialization = InitializeAsync();
        // x = 42;
        // ChildObject = new ChildObject("Unsafe publication async");
    }

    private async Task InitializeAsync()
    {
        await Task.Delay(100);
        x = 42;
        ChildObject = new ChildObject("Unsafe publication async");
    }

    public async Task<ChildObject> GetChildObject()
    {
        return ChildObject;
    }


}