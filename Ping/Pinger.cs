using System.Net;

namespace Ping
{
    public class Pinger
    {
        private const int _maxDataSize = 65499;

        private Dictionary<string, PingerContext> _pingers;

        public Pinger()
        {
            _pingers = new();
        }

        private static void Start(IPAddress address, PingerState state, CustomOptions customOptions)
        {
            var ping = new System.Net.NetworkInformation.Ping();
            var data = new byte[customOptions.DataSize];
            
            while (true)
            {
                var reply = ping.Send(address, timeout: 1000, data);

                if(state.OutputAllowed)
                {
                    Console.WriteLine($"{state.PingerName}: Reply from {reply.Address} time={reply.RoundtripTime}ms ttl={reply.Options?.Ttl}");
                }

                Thread.Sleep(customOptions.Delay);
            }
        }

        public void Create(string name, IPAddress address)
        {
            var customOptions = new CustomOptions { DataSize = _maxDataSize, Delay = 7 };
            var state = new PingerState { PingerName = name, OutputAllowed = false };

            var cancelTokenSource = new CancellationTokenSource();
            var cancellationToken = cancelTokenSource.Token;

            _pingers.Add(name, new PingerContext { CancellationTokenSource = cancelTokenSource, PingerState = state });

            Task.Run(() =>
            {
                Start(address, state, customOptions);
            }, cancellationToken);
        }

        public void SwitchOutput(string name)
        {
            if (!_pingers.TryGetValue(name, out var context))
            {
                Console.WriteLine("No such pinger");
            }
            else
            {
                var state = context.PingerState;
                state.OutputAllowed = !state.OutputAllowed;
            }
        }

        public void Stop(string name)
        {
            if (!_pingers.TryGetValue(name, out var context))
            {
                Console.WriteLine("No such pinger");
            }
            else
            {
                var cancellationTokenSource = context.CancellationTokenSource;
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        public IEnumerable<string> GetActivePingers()
        {
            return _pingers.Keys;
        }
    }
}
