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

namespace DofusBuddy.Core.Managers
{
    public class PacketManager
    {
        private readonly Regex _fightTurnRegex = new("GTS(\\d*)\\|");
        private readonly Regex _chatMessageRegex = new("^cMK\\|(\\d*)\\|(.*?)\\|(.*?)\\|");
        private readonly Regex _groupInvitationRegex = new("^PIK(.*?)\\|(.*)\0");

        private DateTimeOffset _lastGroupInvitationInvoked = DateTimeOffset.Now;

        public event EventHandler<FightTurnEventArgs>? FightTurnPacketReceived;
        public event EventHandler<ChatMessageEventArgs>? ChatMessagePacketReceived;
        public event EventHandler<GroupInvitationEventArgs>? GroupInvitationReceived;

        public PacketManager()
        {
            string hostName = Dns.GetHostName();
            IPAddress localNetworkAddress = Dns.GetHostEntry(hostName).AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);

            LibPcapLiveDevice device = CaptureDeviceList.Instance
                .OfType<LibPcapLiveDevice>()
                .First(x => x.Addresses.Any(y => localNetworkAddress.Equals(y.Addr.ipAddress)));

            device.Open();

            // TODO: Add other dofus retro servers
            device.Filter = "ip and tcp and src 172.65.204.203";

            device.OnPacketArrival += Device_OnPacketArrival;
            device.StartCapture();
        }

        private void Device_OnPacketArrival(object sender, PacketCapture packetCapture)
        {
            RawCapture packet = packetCapture.GetPacket();

            var parsedPacket = Packet.ParsePacket(packet.LinkLayerType, packet.Data);

            TcpPacket tcp = parsedPacket.Extract<TcpPacket>();
            if (tcp?.PayloadData == null || tcp.PayloadData.Length < 0)
            {
                return;
            }

            OnDofusPacketArrival(tcp.PayloadData);
        }

        private void OnDofusPacketArrival(byte[] bytes)
        {
            string data = Encoding.ASCII.GetString(bytes);

            Debug.WriteLine($"{DateTime.Now:hh:mm:ss.fff} - packet: {data.Replace("\0", "\\0")}");

            if (IsGameTurnPacket(data, out FightTurnEventArgs? fightTurnEventArgs))
            {
                Debug.WriteLine("Invoke FightTurnPacketReceived");
                FightTurnPacketReceived?.Invoke(this, fightTurnEventArgs!);
            }
            else if (IsChatMessagePacket(data, out ChatMessageEventArgs? chatMessageEventArgs))
            {
                Debug.WriteLine("Invoke ChatMessagePacketReceived");
                ChatMessagePacketReceived?.Invoke(this, chatMessageEventArgs!);
            }
            else if (IsGroupInvitationPacket(data, out GroupInvitationEventArgs? groupInvitationEventArgs)
                && _lastGroupInvitationInvoked.AddMilliseconds(250) < DateTimeOffset.Now)
            {
                Debug.WriteLine("Invoke GroupInvitationReceived");
                GroupInvitationReceived?.Invoke(this, groupInvitationEventArgs!);
                _lastGroupInvitationInvoked = DateTimeOffset.Now;
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

        private bool IsGroupInvitationPacket(string data, out GroupInvitationEventArgs? groupInvitationEventArgs)
        {
            groupInvitationEventArgs = null;
            Match match = _groupInvitationRegex.Match(data);

            if (match.Success)
            {
                groupInvitationEventArgs = new GroupInvitationEventArgs(match.Groups[1].Value, match.Groups[2].Value);
            }

            return match.Success;
        }
    }
}
