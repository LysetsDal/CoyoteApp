using System.Diagnostics.CodeAnalysis;

namespace CoyoteApp;

public class UnsafePublicationParent 
{
    private int x { get; set; }
    private UnsafePublicationChild _unsafePublicationChild; // UnsafePublicationChild's method might be called before the constructor is done?
    
    public UnsafePublicationParent()
    {
        x = 42;
        _unsafePublicationChild = new UnsafePublicationChild("Unsafe publication async");
        GlobalRegistry._childReference = _unsafePublicationChild;
    }
    
    public async Task<UnsafePublicationChild> getChildObjectAsync()
    {
        return _unsafePublicationChild;
    }

}