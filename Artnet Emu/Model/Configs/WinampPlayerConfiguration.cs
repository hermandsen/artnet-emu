using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Configs
{
    public class WinampPlayerConfiguration : PlayerConfiguration
    {
        [Config("checkBoxAlwaysAdd", ControlType.CheckBox)]
        public bool AlwaysAddFiles { get; set; }
        public override string ToString()
        {
            return "Winamp";
        }
        public override IFileContainer GetFilesContainer()
        {
            return new WinampMediaPlayer(this);
        }
        public override IConfigurationForm GetConfigurationForm()
        {
            return new WinampConfigForm();
        }
        public override string ExtraInfo()
        {
            return "Folder: \"" + FolderPath;
        }
    }
}
