namespace CoyoteApp.DataRaces;

public class Turnstile
{
    public int count = 0;

    /* Note on delayMilliSeconds
     *     1 ms -> Unit tests fails every time
     *    10 ms -> Unit test fails < 30 sec
     *   100 ms -> Unit test fails < 10 min (maybe)
     *  1000 ms -> Unit test fails ???
     */ 
    public void PassThrough(int delayMilliSeconds)
    {
        int temp = count;
        Task.Delay(delayMilliSeconds);
        count = temp + 1;
    }
}