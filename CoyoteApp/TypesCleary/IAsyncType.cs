namespace CoyoteApp;

public interface IAsyncType
{
    /// <summary>
    /// The result of the asynchronous initialization of this instance.
    /// </summary>
    Task Initialization { get; }
    
    public int Value { get; set; }
}