using System.Net;
using System.Net.Sockets;

namespace Node
{
    public class MulticastGroup
    {
        public IPEndPoint Endpoint { get; set; }
        public string Name { get; set; }

        public MulticastGroup(string name, IPEndPoint endpoint, UdpClient udpClient)
        {
            Name = name;
            Endpoint = endpoint;

            udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(endpoint.Address));
            udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
        }

        public void Leave(UdpClient udpClient)
        {
            udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(Endpoint.Address));
        }
    }
}
