using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Configs
{
    public class ITunesPlayerConfiguration : PlayerConfiguration
    {
        public override string ToString()
        {
            return "iTunes";
        }
        public override IFileContainer GetFilesContainer()
        {
            return new ITunesMediaPlayer(this);
        }
        public override IConfigurationForm GetConfigurationForm()
        {
            return new ITunesConfigForm();
        }
        public override string ExtraInfo()
        {
            return "Folder: \"" + FolderPath;
        }
    }
}
