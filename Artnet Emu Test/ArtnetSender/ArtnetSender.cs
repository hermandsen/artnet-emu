using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ArtnetEmuTest.ArtnetSender
{
    internal class ArtnetSender
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        public ArtnetSender(IPEndPoint endpoint)
        {
            udpClient = new UdpClient();
            endPoint = endpoint;
        }
        private byte[] GeneratePacket(ushort opcode)
        {
            string header = "Art-Net";
            byte[] packet = new byte[12];
            int index = 0;
            foreach (char c in header.ToCharArray())
            {
                packet[index++] = (byte)c;
            }
            packet[index++] = 0; // header end

            packet[index++] = (byte)(opcode & 0xFF);
            packet[index++] = (byte)(opcode >> 8);

            packet[index++] = 0; // version
            packet[index++] = 14; // version
            return packet;
        }
        private void SendFullPacket(ushort opcode, byte[] data)
        {
            var packet = GeneratePacket(opcode);
            var fullPacket = new byte[packet.Length + data.Length];
            packet.CopyTo(fullPacket, 0);
            data.CopyTo(fullPacket, packet.Length);
            int bytes = udpClient.Send(fullPacket, fullPacket.Length, endPoint);
        }
        public void SendDMXPacket(byte sequence, byte physical, ushort universe, byte[] dmx)
        {
            var packet = new byte[]
            {
                sequence,
                physical,
                (byte)(universe & 0xFF),
                (byte)(universe >> 8),
                (byte)(dmx.Length & 0xFF),
                (byte)(dmx.Length >> 8)
            };
            var fullPacket = new byte[packet.Length + dmx.Length];
            packet.CopyTo(fullPacket, 0);
            dmx.CopyTo(fullPacket, packet.Length);
            SendFullPacket(ArtnetEmu.Model.ArtnetPackets.ArtDMXPacket.OPCODE, fullPacket);
        }
    }
}
