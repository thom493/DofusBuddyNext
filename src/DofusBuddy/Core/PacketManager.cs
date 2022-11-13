using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using DofusBuddy.Core.GameEvents;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace DofusBuddy.Core
{
    public class PacketManager
    {
        private const string _galgarionIpAddress = "172.65.204.203";
        private readonly Regex _fightTurnRegex = new("^GTM.*?GTS(\\d*)\\|");
        private readonly Regex _chatMessageRegex = new("^cMK\\|(\\d*)\\|(.*?)\\|(.*?)\\|");

        public event EventHandler<FightTurnEventArgs>? FightTurnPacketReceived;
        public event EventHandler<ChatMessageEventArgs>? ChatMessagePacketReceived;

        public PacketManager()
        {
        }

        public void Initialize()
        {
            string hostName = Dns.GetHostName();
            IPAddress localNetworkAddress = Dns.GetHostEntry(hostName).AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);

            LibPcapLiveDevice device = CaptureDeviceList.Instance
                .OfType<LibPcapLiveDevice>()
                .First(x => x.Addresses.Any(y => localNetworkAddress.Equals(y.Addr.ipAddress)));

            device.Open();
            device.OnPacketArrival += Device_OnPacketArrival;
            device.StartCapture();
        }

        private void Device_OnPacketArrival(object sender, PacketCapture packetCapture)
        {
            RawCapture packet = packetCapture.GetPacket();
            if (IsPacketFromDofus(packet, out byte[] data))
            {
                OnDofusPacketArrival(data);
            }
        }

        private static bool IsPacketFromDofus(RawCapture rawCapture, out byte[] data)
        {
            data = Array.Empty<byte>();

            var parsedPacket = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            if (parsedPacket is not EthernetPacket)
            {
                return false;
            }

            IPPacket ip = parsedPacket.Extract<IPPacket>();
            if (ip is null)
            {
                return false;
            }

            if (ip.SourceAddress.ToString() != _galgarionIpAddress)
            {
                return false;
            }

            TcpPacket tcp = parsedPacket.Extract<TcpPacket>();
            if (tcp?.PayloadData == null || tcp.PayloadData.Length < 0)
            {
                return false;
            }

            data = tcp.PayloadData;

            return true;
        }

        private void OnDofusPacketArrival(byte[] bytes)
        {
            string data = Encoding.ASCII.GetString(bytes);

            Debug.WriteLine($"packet: {data.Replace("\0", "\\0")}");

            if (IsGameTurnPacket(data, out FightTurnEventArgs? fightTurnEventArgs))
            {
                FightTurnPacketReceived?.Invoke(this, fightTurnEventArgs!);
            }
            else if (IsChatMessagePacket(data, out ChatMessageEventArgs? chatMessageEventArgs))
            {
                ChatMessagePacketReceived?.Invoke(this, chatMessageEventArgs!);
            }
        }

        private bool IsGameTurnPacket(string data, out FightTurnEventArgs? fightTurnEventArgs)
        {
            fightTurnEventArgs = null;
            Match match = _fightTurnRegex.Match(data);

            if (match.Success)
            {
                fightTurnEventArgs = new FightTurnEventArgs(match.Groups[1].Value);
            }

            return match.Success;
        }

        private bool IsChatMessagePacket(string data, out ChatMessageEventArgs? chatMessageEventArgs)
        {
            chatMessageEventArgs = null;
            Match match = _chatMessageRegex.Match(data);

            if (match.Success)
            {
                chatMessageEventArgs = new ChatMessageEventArgs(match.Groups[1].Value, match.Groups[2].Value);
            }

            return match.Success;
        }
    }
}
