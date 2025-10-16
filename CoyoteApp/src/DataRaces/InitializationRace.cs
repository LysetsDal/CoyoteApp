namespace CoyoteApp;

public class InitializationRace
{
    private static InitializationRace? instance;
    private static readonly object rl = new();
    
    public int hashcode { get; set;  }
    
    private InitializationRace()
    {
        Thread.Sleep(50);
        hashcode = GetHashCode();
    }

    public static InitializationRace getInstance()
    {
        // Not an Atomic operation
        return instance ??= new InitializationRace();
    }
}