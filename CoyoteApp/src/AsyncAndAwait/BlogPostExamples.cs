namespace CoyoteApp.AsyncAndAwait;

public static class BlogPostExamples
{
    
    public static void CopyStreamToStream(Stream source, Stream destination)
    {
        var buffer = new byte[0x1000];
        int numRead;
        while ((numRead = source.Read(buffer, 0, buffer.Length)) != 0)
        {
            destination.Write(buffer, 0, numRead);
        }
    }
    
    public static async Task CopyStreamToStreamAsync(Stream source, Stream destination)
    {
        var buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            await destination.WriteAsync(buffer, 0, numRead);
        }
    }
}