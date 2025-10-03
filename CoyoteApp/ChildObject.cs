namespace CoyoteApp;

public class ChildObject
{
    private readonly string name;

    public ChildObject(string name)
    {
        this.name = name;
    }

    public string getName()
    {
        return this.name;
    }

}