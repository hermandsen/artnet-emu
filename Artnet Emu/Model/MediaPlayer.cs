using ArtnetEmu.Exceptions;
using ArtnetEmu.Model.ArtnetPackets;
using ArtnetEmu.Model.Fixtures;
using ArtnetEmu.Model.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model
{
    public interface IFileContainer
    {
        FilelistForm ShowFiles(bool show = true);
        FilelistForm ShowDuplicates(bool show = true);
        FilelistForm ShowMissing(bool show = true);
    }

    public interface IPlayingMediaPlayer
    {
        string GetPlayingFilename();
        string GetPlayingTitle();
        FileIndexItem GetIndex(string filename);
    }

    public abstract class MediaPlayer<FixtureType, Config> : IFileContainer, IPlayingMediaPlayer
        where FixtureType : Fixture, new()
        where Config : PlayerConfiguration, new()
    {
        protected Config config;
        private CommonFixture lastExecutedPacket;
        public MediaPlayer(Config config)
        {
            this.config = config;
            Files = new FileStructure();
            Duplicates = new List<FileIndexItem>();
            Initialize();
            LoadFiles();
        }
        public FileStructure Files
        {
            get;
        }
        public virtual FileIndexItem GetIndex(string filename)
        {
            return Files.Find(filename);
        }
        protected List<FileIndexItem> Duplicates;
        protected abstract void LoadFiles();
        protected virtual void Initialize() { }
        public void PacketReceived(ArtDMXPacket packet)
        {
            if (packet.Universe == config.Universe)
            {
                StructReader sr = new StructReader(packet.Data);
                sr.Position = config.Address - 1;
                FixtureType result = sr.Read<FixtureType>(typeof(FixtureType));
                PacketReceived(result);
            }
        }
        
        public abstract void PacketReceived(FixtureType packet);
        public abstract void AddToPlaylistAndPlay(string filename, PlayMode mode);
        public abstract string GetPlayingFilename();
        public abstract string GetPlayingTitle();
        public abstract void Next();
        public abstract void Pause();
        public abstract void Previous();
        public abstract void ResumePlayback();
        public abstract void Stop();
        public abstract void SetVolume(byte volume);
        protected void PacketReceived(CommonFixture packet)
        {
            if (lastExecutedPacket == null || packet.HasFileChanges(lastExecutedPacket) && packet.Control == ExecuteControl.Execute)
            {
                switch (packet.Mode)
                {
                    case PlayMode.Ignore:
                        break;
                    case PlayMode.PlayFile:
                    case PlayMode.PlayFileAndStop:
                        if (Files.Exists(packet.Group, packet.File))
                        {
                            AddToPlaylistAndPlay(Files[packet.Group, packet.File], packet.Mode);
                        }
                        break;
                    case PlayMode.Next:
                        Next();
                        break;
                    case PlayMode.Pause:
                        Pause();
                        break;
                    case PlayMode.Previous:
                        Previous();
                        break;
                    case PlayMode.Resume:
                        ResumePlayback();
                        break;
                    case PlayMode.Stop:
                        Stop();
                        break;
                }
            }
            if (lastExecutedPacket == null || packet.HasVolumeChanges(lastExecutedPacket))
            {
                SetVolume(packet.Volume);
            }
            lastExecutedPacket = packet;
        }
        public Config Configuration { get { return config; } }
        public FilelistForm ShowFiles(bool show = true)
        {
            FilelistForm form = new FilelistForm();
            form.ClearList();
            foreach (var item in Files)
            {
                form.AddFileIndexItem(item);
            }
            form.SortFiles();
            if (show)
            {
                form.ShowDialog();
            }
            return form;
        }
        public FilelistForm ShowDuplicates(bool show = true)
        {
            FilelistForm form = new FilelistForm();
            form.ClearList();
            foreach (var item in Duplicates)
            {
                form.AddFileIndexItem(item);
            }
            form.SortFiles();
            if (show)
            {
                form.ShowDialog();
            }
            return form;
        }
        public FilelistForm ShowMissing(bool show = true)
        {
            FilelistForm form = new FilelistForm();
            form.ClearList();
            foreach (var item in Files.Missing)
            {
                form.AddFileIndexItem(item);
            }
            form.SortFiles();
            if (show)
            {
                form.ShowDialog();
            }
            return form;
        }
    }
}
