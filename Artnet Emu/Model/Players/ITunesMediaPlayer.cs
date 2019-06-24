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
/**
 * Remove comments in this file, and add iTunesLib as reference, to add support for iTunes.
 */
//using iTunesLib;

namespace ArtnetEmu.Model
{
    public class ITunesMediaPlayer : MediaPlayer<ITunesFixture, ITunesPlayerConfiguration>
    {
        //protected iTunesApp Client;
        public ITunesMediaPlayer(ITunesPlayerConfiguration config) : base(config) {
        }

        protected override void Initialize()
        {
            //Client = new iTunesApp();
        }

        public override void AddToPlaylistAndPlay(string filename, PlayMode mode)
        {
            //Client.PlayFile(filename);
        }

        public override void Next()
        {
            //Client.NextTrack();
        }

        public override void PacketReceived(ITunesFixture packet)
        {
            base.PacketReceived((CommonFixture)packet);
        }

        public override void Pause()
        {
            //Client.Pause();
        }

        public override void Previous()
        {
            //Client.PreviousTrack();
        }

        public override void ResumePlayback()
        {
            //Client.Resume();
        }

        public override void SetVolume(byte volume)
        {
            //Client.SoundVolume = Convert.ToInt32(Math.Floor((volume/255.0)*100));
        }

        public override void Stop()
        {
            //Client.Stop();
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
            return null; // missing implementation
        }

        public override string GetPlayingTitle()
        {
            return null; // missing implementation
        }
    }
}
