using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Configs
{
    public class VLCLocalPlayerConfiguration : VLCPlayerConfiguration
    {
        public override string ToString()
        {
            return "VLC Local";
        }
        public override IFileContainer GetFilesContainer()
        {
            return new VLCLocalMediaPlayer(this);
        }
        public override IConfigurationForm GetConfigurationForm()
        {
            return new VLCLocalConfigForm();
        }
        public override string ExtraInfo()
        {
            return "Folder: \"" + FolderPath;
        }
    }
}
