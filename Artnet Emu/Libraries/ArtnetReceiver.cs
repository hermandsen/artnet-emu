using ArtnetEmu.Model.ArtnetPackets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Libraries
{
    public enum ArtnetServerState
    {
        Terminated,
        Running,
        Aborted
    }

    public delegate void ArtDMXPacketReceivedEvent(ArtDMXPacket packet);
    public delegate void ArtPollReplyPacketReceivedEvent(ArtPollReplyPacket packet);
    public delegate void ArtPollPacketReceivedEvent(ArtPollPacket packet);
    public delegate void ThreadStateChangedEvent(ArtnetServerState running, string message);

    public class ArtnetReceiver
    {
        private IPEndPoint RemoteEndPoint;
        private IPEndPoint LocalEndPoint;
        private bool StopRequested = false;
        private byte[] UniverseSequences;
        private static UdpClient ServerSocket;
        private static UdpClient ClientSocket;
        private System.Timers.Timer PollTimer;
        private bool pollEnabled = false;
        public ArtnetReceiver(IPEndPoint sender, IPEndPoint receiver)
        {
            RemoteEndPoint = sender;
            LocalEndPoint = receiver;

            ClientSocket = new UdpClient();
            ClientSocket.EnableBroadcast = true;
            ClientSocket.Client.Blocking = false;
        }

        public bool PollEnabled
        {
            get
            {
                return pollEnabled;
            }
            set
            {
                if (value)
                {
                    StartPollTimer();
                }
                else
                {
                    StopPollTimer();
                }
                pollEnabled = value;
            }
        }

        public event ArtDMXPacketReceivedEvent OnArtDMXPacketReceived;
        public event ArtPollPacketReceivedEvent OnArtPollPacketReceived;
        public event ArtPollReplyPacketReceivedEvent OnArtPollReplyPacketReceived;
        public event ThreadStateChangedEvent OnThreadStateChanged;

        private void PollTimerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendArtPollPacket();
        }

        protected virtual void InvokeOnThreadState(ArtnetServerState running, string message)
        {
            OnThreadStateChanged?.Invoke(running, message);
        }

        public void Stop()
        {
            StopRequested = true;
        }

        public void Terminate()
        {
            StopPollTimer();
            if (ServerSocket != null)
            {
                ServerSocket.Close();
                ServerSocket = null;
            }
        }

        private void StartPollTimer()
        {
            StopPollTimer();

            PollTimer = new System.Timers.Timer();
            PollTimer.Elapsed += PollTimerCallback;
            PollTimer.AutoReset = true;
            PollTimer.Interval = 2500;
            PollTimer.Enabled = true;
            SendArtPollPacket();
        }

        private void StopPollTimer()
        {
            if (PollTimer != null)
            {
                PollTimer.Enabled = false;
                PollTimer.Dispose();
                PollTimer = null;
            }
        }
        public void Start()
        {
            Terminate();
            UniverseSequences = new byte[256*256];
            Array.Clear(UniverseSequences, 0, UniverseSequences.Length);
            try
            {
                ServerSocket = new UdpClient();
                ServerSocket.ExclusiveAddressUse = false;
                ServerSocket.EnableBroadcast = true;
                ServerSocket.Client.ExclusiveAddressUse = false;
                ServerSocket.Client.Bind(LocalEndPoint);
            }
            catch (SocketException e)
            {
                // port may already be in use
                ServerSocket = null;
                string message = "Another instance is blocking the port.\nThe program might be running in the background.";
                try
                {
                    var blockingApp = UdpTableReader.GetAllUdpConnections().FirstOrDefault(x => x.LocalPort == LocalEndPoint.Port && x.LocalAddress.Equals(LocalEndPoint.Address));
                    if (blockingApp != null)
                        message = $"{blockingApp.ProcessTitle} is blocking the port.\nClose the program, or choose another network adapter.";
                }
                catch {}
                InvokeOnThreadState(ArtnetServerState.Aborted, message);
                return;
            }
            catch (Exception e)
            {
                ServerSocket = null;
                InvokeOnThreadState(ArtnetServerState.Aborted, e.Message);
                return;
            }
            InvokeOnThreadState(ArtnetServerState.Running, null);
            while (!StopRequested)
            {
                byte[] data = ServerSocket.Receive(ref RemoteEndPoint);
                BytesReceived(data);
            }
            Terminate();
            InvokeOnThreadState(ArtnetServerState.Terminated, null);
        }
        public void SendArtPollPacket()
        {
            ArtPollPacket packet = ArtPollPacket.Create();
            SendArtnetPacket(packet);
        }
        public void SendArtnetPacket(ArtnetPacket packet)
        {
            StructWriter sw = new StructWriter();
            sw.Write(packet);
            var buffer = sw.GetBytes();
            var endpoint = new IPEndPoint(new IPAddress(new byte[] { 255, 255, 255, 255 }), 6454);
            ClientSocket.Send(buffer, buffer.Length, endpoint);
        }
        private void BytesReceived(byte[] data)
        {
            StructReader sr = new StructReader(data);
            ArtnetPacket packet = sr.Read<ArtnetPacket>(typeof(ArtnetPacket));
            if (packet.IsValid)
            {
                sr.Position = 0;
                PacketReceived(sr, packet.Opcode);
            }
        }
        protected virtual void InvokeOnPacketReceived(ArtDMXPacket packet)
        {
            if (packet.IsValid &&
                (
                    // Confirm that packet sequences are in order
                    packet.Sequence == 0 || // If Sequence = 0, order is disabled.
                    UniverseSequences[packet.Universe] == 0 || // If UniverseSequence = 0, first packet
                    packet.Sequence - UniverseSequences[packet.Universe] > 0 ||
                    packet.Sequence - UniverseSequences[packet.Universe] < -127)
                )
            {
                UniverseSequences[packet.Universe] = packet.Sequence;
                OnArtDMXPacketReceived?.Invoke(packet);
            }
        }
        protected virtual void InvokeOnPacketReceived(ArtPollReplyPacket packet)
        {
            if (packet.IsValid)
            {
                OnArtPollReplyPacketReceived?.Invoke(packet);
            }
        }
        protected virtual void InvokeOnPacketReceived(ArtPollPacket packet)
        {
            if (packet.IsValid)
            {
                OnArtPollPacketReceived?.Invoke(packet);
            }
        }
        private void SendArtPollReply()
        {
            var packet = ArtPollReplyPacket.Create(LocalEndPoint.Address, (ushort)LocalEndPoint.Port);
            packet.NetSwitch = 0;
            packet.SubSwitch = 0;
            packet.Oem = 0xffff;
            packet.UbeaVersion = 0;
            packet.Status1 = 0;
            packet.Esta = 0;
            packet.ShortName = "Artnet Emu";
            packet.LongName = "Artnet Emu media control";
            packet.NodeReport = "";
            packet.NumPorts = 0;
            packet.PortTypes = new byte[4] { 0, 0, 0, 0};
            packet.GoodInput = new byte[4] { 0x80, 0x80, 0x80, 0x80 };
            packet.GoodOutput = new byte[4] { 0, 0, 0, 0 };
            packet.SwIn = new byte[4] { 0, 1, 2, 3 };
            packet.SwOut = new byte[4] { 0, 0, 0, 0 };
            packet.SwVideo = 0;
            packet.SwMacro = 0;
            packet.SwRemote = 0;
            packet.Spare1 = 0;
            packet.Spare2 = 0;
            packet.Spare3 = 0;
            packet.NodeStyle = ArtPollReplyStyle.Media;
            packet.MacAddress = new byte[6] { 0, 0, 0, 0, 0, 0 };
            packet.BindIp = new byte[4] { 0, 0, 0, 0 };
            packet.BindIndex = 0;
            packet.Status2 = 0;
            SendArtnetPacket(packet);
        }
        private void PacketReceived(StructReader reader, ushort opcode)
        {
            ArtnetPacket packet = null;
            switch (opcode)
            {
                case ArtDMXPacket.OPCODE:
                    packet = reader.Read<ArtDMXPacket>(typeof(ArtDMXPacket));
                    InvokeOnPacketReceived((ArtDMXPacket)packet);
                    break;
                case ArtPollPacket.OPCODE:
                    packet = reader.Read<ArtPollPacket>(typeof(ArtPollPacket));
                    InvokeOnPacketReceived((ArtPollPacket)packet);
                    break;
                case ArtPollReplyPacket.OPCODE:
                    packet = reader.Read<ArtPollReplyPacket>(typeof(ArtPollReplyPacket));
                    InvokeOnPacketReceived((ArtPollReplyPacket)packet);
                    break;
            }
        }
    }
}
