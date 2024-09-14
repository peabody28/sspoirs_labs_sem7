using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Node
{
    public class Node
    {
        private int _port;
        private string _name;
        private UdpClient _udpClient;

        public Node(string name)
        {
            _port = 5555;
            _name = name;
            _udpClient = new UdpClient(_port);
        }

        public void Start()
        {
            Listen();

            while(true)
            {
                Console.Write("Input message: ");
                var message = Console.ReadLine();

                var packet = new Packet { From = _name, Message = message };
                var remotePoint = new IPEndPoint(IPAddress.Broadcast, _port);

                SendString(packet.ToString(), remotePoint);
            }
        }

        private void SendString(string message, IPEndPoint target)
        {
            var data = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(data, target);
        }

        private void Listen()
        {
            Task.Run(() =>
            {
                var remoteHost = new IPEndPoint(IPAddress.Any, 0);
                var result = _udpClient.Receive(ref remoteHost);
                var message = Encoding.UTF8.GetString(result);

                var packet = Packet.Parse(message);

                //if(!packet.From.Equals(_name))
                Console.WriteLine(message);
            });
        }

    }
}
