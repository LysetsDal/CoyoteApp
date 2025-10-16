namespace CoyoteApp;

public class UnsafeInheritance
{
    // public static UnsafeInheritance? SharedInstance;
    
    public UnsafeInheritance()
    {
        // SharedInstance = this;
        Initialize();
    }

    
    protected virtual void Initialize()
    {
        // overridden by child class 
    }
}

public class UnsafeInheritanceDerived : UnsafeInheritance
{
    private readonly string message = "If you see this: SafePublication";
    public string? ObservedMessage { get; private set; }

    protected override void Initialize()
    {
        ObservedMessage = message;
    }
}