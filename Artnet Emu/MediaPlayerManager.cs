using ArtnetEmu.Exceptions;
using ArtnetEmu.Model;
using ArtnetEmu.Libraries;
using ArtnetEmu.Model.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ArtnetEmu.Model.Interfaces;

namespace ArtnetEmu
{
    public class MediaPlayerManager : IPreloader
    {
        private List<PlayerConfiguration> PlayersConfigurations;
        private List<IPlayingMediaPlayer> runningPlayers = new List<IPlayingMediaPlayer>();
        private ArtnetReceiverThread thread;
        private ArtnetReceiver server;
        public event ThreadStateChangedEvent OnThreadStateChanged
        {
            add
            {
                thread.OnThreadStateChanged += value;
            }
            remove
            {
                thread.OnThreadStateChanged -= value;
            }
        }
        public ArtnetReceiver Server
        {
            get
            {
                return server;
            }
        }
        public MediaPlayerManager(ArtnetReceiver server, List<PlayerConfiguration> players)
        {
            thread = new ArtnetReceiverThread(server);
            this.server = server;
            PlayersConfigurations = players;
        }
        public IReadOnlyList<IPlayingMediaPlayer> Players
        {
            get
            {
                return runningPlayers;
            }
        }
        public void Start()
        {
            thread.Start(this);
        }
        public void Stop()
        {
            thread.Stop();
        }
        public void Preload()
        {
            foreach (PlayerConfiguration config in PlayersConfigurations)
            {
                switch (config)
                {
                    case VLCLocalPlayerConfiguration x:
                        VLCLocalMediaPlayer vlcLocal = new VLCLocalMediaPlayer(x);
                        server.OnArtDMXPacketReceived += vlcLocal.PacketReceived;
                        runningPlayers.Add(vlcLocal);
                        break;
                    case VLCRemotePlayerConfiguration x:
                        VLCRemoteMediaPlayer vlcRemote = new VLCRemoteMediaPlayer(x);
                        server.OnArtDMXPacketReceived += vlcRemote.PacketReceived;
                        runningPlayers.Add(vlcRemote);
                        break;
                    case WinampPlayerConfiguration x:
                        WinampMediaPlayer winamp = new WinampMediaPlayer(x);
                        server.OnArtDMXPacketReceived += winamp.PacketReceived;
                        runningPlayers.Add(winamp);
                        break;
                    case ITunesPlayerConfiguration x:
                        ITunesMediaPlayer itunes = new ITunesMediaPlayer(x);
                        server.OnArtDMXPacketReceived += itunes.PacketReceived;
                        runningPlayers.Add(itunes);
                        break;
                    default:
                        throw new Exception("You're missing a configuration stupid.");
                }
            }
        }
    }
}
