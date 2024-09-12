using System.Net;
using System.Net.NetworkInformation;

namespace Ping
{
    public class Tracerouter
    {
        public void Start(IPAddress address)
        {
            var ping = new System.Net.NetworkInformation.Ping();

            for (int i = 1; i <= 32; i++)
            {
                var pingOptions = new PingOptions { Ttl = i };

                var reply = ping.Send(address, timeout: 1000, new byte[2], pingOptions);

                Console.WriteLine($"{i}. {reply.RoundtripTime}ms {reply.Address}");
                if (reply.Address.Equals(address))
                    break;

                Thread.Sleep(1000);
            }
        }
    }
}
