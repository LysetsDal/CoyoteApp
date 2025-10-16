namespace CoyoteApp;

public class EscapingInstance
{
    private static volatile EscapingInstance instance;
    private static readonly object rl = new();
    
    public int hashcode { get; set;  }
    
    private EscapingInstance()
    {
        Thread.Sleep(500);
        // Initialize me daddy...
        
        hashcode = GetHashCode();
    }

    public static EscapingInstance getInstance()
    {
        if (instance == null)
        {
            instance = new EscapingInstance();
        }

        return instance;
    }
}