using NUnit.Framework;
using ArtnetEmu;
using System.Net;
using ArtnetEmuTest.ArtnetSender;
using ArtnetEmu.Model.ArtnetPackets;
using System.Collections.Generic;
using System.Threading;
using ArtnetEmu.Libraries;

namespace Tests
{
    public class ArtnetServerTest
    {
        private ArtnetSender Client;
        private IPEndPoint EndpointSender;
        private IPEndPoint EndpointReceiver;
        private MediaPlayerManager Manager;
        private ManualResetEvent ManualReset;
        private Thread ClientThread;

        [SetUp]
        public void Setup()
        {
            var ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndpointSender = new IPEndPoint(ip, 6454);
            EndpointReceiver = new IPEndPoint(ip, 6454);
        }

        [Test]
        public void TestDMXPacket()
        {
            bool dataValid = false;
            bool packetValid = false;

            var server = new ArtnetReceiver(EndpointSender, EndpointReceiver);
            Manager = new MediaPlayerManager(server, new List<ArtnetEmu.Model.Configs.PlayerConfiguration>());
            Manager.OnThreadStateChanged += Server_OnThreadStateChanged;
            server.OnArtDMXPacketReceived += new ArtDMXPacketReceivedEvent((ArtDMXPacket packet) =>
            {
                packetValid = packet.IsValid;
                dataValid = true;
                for (var i = 0; i < packet.Data.Length && dataValid; i++)
                {
                    dataValid = packet.Data[i] == (i & 0xFF);
                }
                ManualReset.Set();
            });
            Manager.Start();
            ManualReset = new ManualResetEvent(false);
            Assert.IsTrue(ManualReset.WaitOne(6000), "Server took too long.");
            Assert.IsTrue(packetValid, "Packed was invalid");
            Assert.IsTrue(dataValid, "Data packet incorrect");
        }

        private void ServerRunning()
        {
            Client = new ArtnetSender(EndpointSender);
            var dmx = new byte[512];
            for (var i = 0; i < dmx.Length; i++)
            {
                dmx[i] = (byte)(i & 0xFF);
            }
            Client.SendDMXPacket(0, 1, 1, dmx);
        }

        private void Server_OnThreadStateChanged(ArtnetServerState running, string message)
        {
            switch (running)
            {
                case ArtnetServerState.Running:
                    ClientThread = new Thread(ServerRunning);
                    ClientThread.Start();
                    break;
                case ArtnetServerState.Terminated:
                    break;
                case ArtnetServerState.Aborted:
                default:
                    Assert.Fail("Server failed to start. Message: " + message);
                    break;
            }
        }
    }
}