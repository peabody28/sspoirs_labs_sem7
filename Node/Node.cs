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
        private List<MulticastGroup> _groups;
        private CancellationTokenSource _cancellationTokenSource;

        public Node(string name)
        {
            _port = 5555;
            _name = name;
            _groups = new List<MulticastGroup>();

            _channel = new UdpClient();
            _channel.EnableBroadcast = true;
            _channel.Client.Bind(new IPEndPoint(IPAddress.Any, _port));

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Listen();

            while (true)
            {
                var action = GetAction();

                IPEndPoint remotePoint = null;
                Packet packet = null;
                string multicastGroupName = null;

                if (action == SelectAction.EnterMulticastGroup || action == SelectAction.MulticastMessage || action == SelectAction.LeaveMulticastGroup)
                {
                    Console.Write("Input multicast group name: ");
                    multicastGroupName = Console.ReadLine();
                }

                if(action == SelectAction.BroadcastMessage)
                {
                    remotePoint = new IPEndPoint(IPAddress.Broadcast, _port);
                }
                else if(action == SelectAction.MulticastMessage)
                {
                    var group = _groups.FirstOrDefault(g => g.Name.Equals(multicastGroupName));
                    if (group == null)
                        Console.WriteLine("Group not found");
                    else
                        remotePoint = group.Endpoint;
                }
                else if(action == SelectAction.EnterMulticastGroup)
                {
                    Console.Write("Input multicast group address: ");
                    var ipAddress = IPAddress.Parse(Console.ReadLine());
                    var endpoint = new IPEndPoint(ipAddress, _port);

                    _groups.Add(new MulticastGroup(multicastGroupName, endpoint, _channel));
                }
                else if(action == SelectAction.LeaveMulticastGroup)
                {
                    var group = _groups.FirstOrDefault(g => g.Name.Equals(multicastGroupName));
                    if (group == null)
                        Console.WriteLine("Group not found");
                    else
                    {
                        group.Leave(_channel);
                        _groups.Remove(group);
                    }
                }

                if (action == SelectAction.BroadcastMessage || action == SelectAction.MulticastMessage)
                {
                    Console.Write("Input message: ");
                    var message = Console.ReadLine();
                    packet = new Packet { From = _name, Message = message };

                    var data = Encoding.UTF8.GetBytes(packet.ToString());
                    _channel.Send(data, remotePoint);
                }

                if (action == SelectAction.Exit)
                    break;
            }

            _cancellationTokenSource.Cancel();
        }

        private SelectAction GetAction()
        {
            Console.WriteLine();
            Console.WriteLine("1 - broadcast message");
            Console.WriteLine("2 - enter multicast group");
            Console.WriteLine("3 - multicast message");
            Console.WriteLine("4 - leave multicast group");
            Console.WriteLine("5 - exit");
            Console.Write("Input action: ");

            var action = Convert.ToInt32(Console.ReadLine());

            return (SelectAction)action;
        }

        private void Listen()
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
            }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}
