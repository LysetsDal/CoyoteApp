namespace CoyoteApp;

public class UnsafeInheritance
{
    public UnsafeInheritance()
    {
        // SharedInstance = this;
        Initialize();
    }

    
    public virtual void Initialize()
    {
        // overridden by child class 
        Console.WriteLine("Base.Initialize");
    }
}

public class UnsafeInheritanceDerived : UnsafeInheritance
{
    private readonly string message = "If you see this: SafePublication";
    public List<string> ObservedMessage;

    public UnsafeInheritanceDerived() : base()
    {
        Console.WriteLine("Derived constructor running, message = " + message);
        ObservedMessage = new List<string>();
    }
    
    public override void Initialize()
    {
        Console.WriteLine("Derived.Initialize called, message = " + message);
        ObservedMessage.Add(message);
    }
}