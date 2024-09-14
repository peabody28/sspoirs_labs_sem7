using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Node
{
    public class Node : IDisposable
    {
        private int _port;
        private string _name;
        private UdpClient _channel;

        public Node(string name)
        {
            _port = 5555;
            _name = name;
            _channel = new UdpClient();
            _channel.EnableBroadcast = true;

            _channel.Client.Bind(new IPEndPoint(IPAddress.Any, _port));
        }

        public void Start()
        {
            var cancelationTokenSource = new CancellationTokenSource();
            var cancelationToken = cancelationTokenSource.Token;

            Listen(cancelationToken);

            while (true)
            {
                Console.Write("Input message: ");
                var message = Console.ReadLine();

                if (message.Equals("exit"))
                    break;

                var packet = new Packet { From = _name, Message = message };
                var remotePoint = new IPEndPoint(IPAddress.Broadcast, _port);

                SendString(packet.ToString(), remotePoint);
            }

            cancelationTokenSource.Cancel();
        }

        private void SendString(string message, IPEndPoint target)
        {
            var data = Encoding.UTF8.GetBytes(message);
            _channel.Send(data, target);
        }

        private void Listen(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                var remoteHost = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var data = _channel.Receive(ref remoteHost);

                    var message = Encoding.UTF8.GetString(data);

                    var packet = Packet.Parse(message);

                    if (!packet.From.Equals(_name))
                        Console.WriteLine(message);
                }
            }, cancellationToken);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}
