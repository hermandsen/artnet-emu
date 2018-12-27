using System;
using System.Collections.Generic;
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

namespace ArtnetEmu
{
    [Struct(Alignment = Alignment.PACKED)]
    public class ArtNetPacket
    {
        [StructField(N = 0, Type = FieldType.FIXSTRNULTERM, Length = 8)]
        public string Name; // always "Art-Net"0
        [StructField(N = 1, Type = FieldType.UINT2)]
        public ushort Opcode; // 0x5000
        [StructField(N = 2, Type = FieldType.UINT2)]
        public ushort ProtocolVersion; // 14
        [StructField(N = 3, Type = FieldType.UINT1)]
        public byte Sequence;
        [StructField(N = 4, Type = FieldType.UINT1)]
        public byte Physical;
        [StructField(N = 5, Type = FieldType.UINT2)]
        public ushort Universe;
        [StructField(N = 6, Type = FieldType.UINT2)]
        public ushort Length; // mostly 512
        [ArrayField(Elements = 512)]
        [StructField(N = 7, Type = FieldType.UINT1)]
        public byte[] Data = new byte[512];
        public bool IsValid
        {
            get
            {
                return Name == "Art-Net"
                    && Opcode == 0x5000
                    && ProtocolVersion == 14 << 8;
            }
        }
    }
    /*
    [StructLayout(LayoutKind.Explicit, Size = 530, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ArtNetPacket
    {
        [FieldOffset(0)]
        public unsafe fixed byte Header[8]; // always "Art-Net"0

        [FieldOffset(8)]
        public ushort Opcode; // 0x5000

        [FieldOffset(10)]
        public ushort ProtocolVersion; // 14

        [FieldOffset(12)]
        public byte Sequence;

        [FieldOffset(13)]
        public byte Physical;

        [FieldOffset(14)]
        public ushort Universe;

        [FieldOffset(16)]
        public ushort Length; // mostly 512

        [FieldOffset(18)]
        public unsafe fixed byte Data[512];

        public ArtNetPacket(byte[] data)
        {
            if (data.Length != 530)
            {
                throw new ArgumentException("Data must be 530 bytes");
            }
            unsafe
            {
                fixed (byte* packet = &data[0])
                {
                    this = *(ArtNetPacket*)packet;
                }
            }
        }
    }
    */

    public enum ArtnetServerState
    {
        Terminated,
        Running,
        Aborted
    }

    public delegate void PacketReceivedEventHandler(ArtNetPacket packet);
    public delegate void ThreadStateChanged(ArtnetServerState running);

    public class ArtnetServer
    {
        private IPAddress SenderIp;
        private IPAddress ReceiverIp;
        private int Port;
        private bool StopRequested = false;
        private byte[] UniverseSequences;
        private static UdpClient Socket;
        public ArtnetServer(IPAddress sender, IPAddress receiver, int port)
        {
            SenderIp = sender;
            ReceiverIp = receiver;
            Port = port;
        }

        public event PacketReceivedEventHandler PacketReceived;
        public event ThreadStateChanged ThreadState;

        protected virtual void OnPacketReceived(ArtNetPacket packet)
        {
            PacketReceived?.Invoke(packet);
        }

        protected virtual void OnThreadState(ArtnetServerState running)
        {
            ThreadState?.Invoke(running);
        }

        public void StopThread()
        {
            StopRequested = true;
        }

        public void Terminate()
        {
            if (Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
        }

        public void Run()
        {
            UniverseSequences = new byte[256*256];
            IPEndPoint listenerIp = new IPEndPoint(ReceiverIp, Port);
            if (Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
            try
            {
                Socket = new UdpClient(listenerIp);
            } catch (SocketException)
            {
                // port may already be in use
                Socket = null;
                OnThreadState(ArtnetServerState.Aborted);
                return;
            }
            IPEndPoint senderIp = new IPEndPoint(SenderIp, 0);
            OnThreadState(ArtnetServerState.Running);
            while (!StopRequested)
            {
                byte[] data = Socket.Receive(ref senderIp);
                StructReader sr = new StructReader(data);
                ArtNetPacket packet = sr.Read<ArtNetPacket>(typeof(ArtNetPacket));

                // Confirm that packet sequences are in order
                if (packet.IsValid && packet.Sequence - UniverseSequences[packet.Universe] > 0 || packet.Sequence - UniverseSequences[packet.Universe] < -127)
                {
                    UniverseSequences[packet.Universe] = packet.Sequence;
                    OnPacketReceived(packet);
                }
                else
                {
                    Console.WriteLine("Packet sequence: " + packet.Sequence + "/" + UniverseSequences);
                }
                //ArtNetPacket packet = sr.Read<ArtNetPacket>(typeof(ArtNetPacket), null, null, (o, n) => n == 7 ? ((ArtNetPacket)o).Length : -1);
            }
            Terminate();
            OnThreadState(ArtnetServerState.Terminated);
        }
    }
}
