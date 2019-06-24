using ArtnetEmu.Clients;
using ArtnetEmu.Exceptions;
using ArtnetEmu.Model.Configs;
using ArtnetEmu.Model.Fixtures;
using ArtnetEmu.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public class WinampMediaPlayer : MediaPlayer<WinampFixture, WinampPlayerConfiguration>
    {
        protected WinampClient Client;
        public WinampMediaPlayer(WinampPlayerConfiguration config) : base(config) {
        }

        protected override void Initialize()
        {
            Client = new WinampClient();
        }

        private void AddAndPlay(string filename)
        {
            Client.Stop();
            Client.EnqueueFile(filename);
            Client.PlaylistPosition = Client.PlaylistLength - 1;
            Client.Play();
        }

        public override void AddToPlaylistAndPlay(string filename, PlayMode mode)
        {
            if (config.AlwaysAddFiles)
            {
                AddAndPlay(filename);
            }
            else
            {
                int pos = Client.PlaylistPosition;
                int length = Client.PlaylistLength;
                int found = -1;
                string clientFile;
                for (int i = pos; i < length && found == -1; i++)
                {
                    clientFile = Client.PlaylistFilename(i);
                    if (clientFile == filename)
                    {
                        found = i;
                    }
                }
                for (int i = 0; i < pos && found == -1; i++)
                {
                    clientFile = Client.PlaylistFilename(i);
                    if (clientFile == filename)
                    {
                        found = i;
                    }
                }
                if (found == -1)
                {
                    AddAndPlay(filename);
                }
                else
                {
                    Client.Stop();
                    Client.PlaylistPosition = found;
                    Client.Play();
                }
            }
            switch (mode)
            {
                case PlayMode.PlayFile:
                    Client.ManualPlaylistAdvance = false;
                    break;
                case PlayMode.PlayFileAndStop:
                    Client.ManualPlaylistAdvance = true;
                    break;
            }
        }

        public override void Next()
        {
            Client.Next();
        }

        public override void PacketReceived(WinampFixture packet)
        {
            base.PacketReceived((CommonFixture)packet);
        }

        public override void Pause()
        {
            Client.TogglePause();
        }

        public override void Previous()
        {
            Client.Previous();
        }

        public override void ResumePlayback()
        {
            Client.Play();
        }

        public override void SetVolume(byte volume)
        {
            Client.Volume = volume;
        }

        public override void Stop()
        {
            Client.Stop();
        }

        protected override void LoadFiles()
        {
            try
            {
                switch (config.FileScanMethod)
                {
                    case FileScanMethod.Filelist:
                        Files.LoadFromFilelists(config.FolderPath, config.FileEncoding);
                        break;
                    case FileScanMethod.Filestructure:
                        Files.LoadFromFilestructure(config.FolderPath);
                        break;
                    case FileScanMethod.Regex:
                        Files.LoadFromFilestructure(config.FolderPath, new Regex(config.Regex, RegexOptions.IgnoreCase));
                        break;
                }
            }
            catch (DuplicateFileIndexException exception)
            {
                Duplicates = exception.Duplicates;
            }
        }

        public override string GetPlayingFilename()
        {
            return Client.PlaylistFilename(Client.PlaylistPosition);
        }

        public override string GetPlayingTitle()
        {
            return Client.PlaylistTitle(Client.PlaylistPosition);
        }
    }
}
