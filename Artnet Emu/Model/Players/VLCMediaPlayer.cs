using ArtnetEmu.Model.Configs;
using ArtnetEmu.Model.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Players
{
    public abstract class VLCMediaPlayer<Config> : MediaPlayer<VLCFixture, Config> where Config : VLCPlayerConfiguration, new()
    {
        protected VLCHttpClient Client;
        protected VLCFixture lastExecutedPacket = null;

        public VLCMediaPlayer(Config config) : base(config) { }
        protected abstract string Url { get; }

        protected override void Initialize()
        {
            Client = new VLCHttpClient(Url, config.Password);
        }

        protected void AddAndPlay(string filename)
        {
            Uri uri = new Uri(filename);
            filename = uri.AbsoluteUri;
            Client.AddToPlaylistAndPlay(filename);
        }
        public override void AddToPlaylistAndPlay(string filename, PlayMode mode)
        {
            if (config.AlwaysAddFiles)
            {
                AddAndPlay(filename);
            }
            else
            {
                Uri uri = new Uri(filename);
                filename = uri.AbsoluteUri;
                string id = Client.GetPlaylistId(filename);
                if (id == "")
                {
                    AddAndPlay(filename);
                }
                else
                {
                    Client.PlayPlaylistItem(id);
                }
            }
        }

        public override void Next()
        {
            Client.Next();
        }

        public override void Pause()
        {
            Client.Pause();
        }

        public override void Previous()
        {
            Client.Previous();
        }

        public override void ResumePlayback()
        {
            Client.ResumePlayback();
        }

        public override void SetVolume(byte volume)
        {
            Client.SetVolume(volume);
        }

        public override void Stop()
        {
            Client.Stop();
        }

        public override void PacketReceived(VLCFixture packet)
        {
            base.PacketReceived((CommonFixture)packet);
        }
    }
}
