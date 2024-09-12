namespace Ping
{
    public class PingerContext
    {
        public PingerState PingerState { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
