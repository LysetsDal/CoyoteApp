namespace CoyoteApp;

public class EscapingInstance
{
    private static EscapingInstance instance;

    public int x { get; set;  }
    
    private EscapingInstance()
    {
        // Initialize me daddy...
        var rand = new Random(42069);
        x = rand.Next();
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