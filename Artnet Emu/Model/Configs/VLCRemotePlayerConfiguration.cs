using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Configs
{
    public class VLCRemotePlayerConfiguration : VLCPlayerConfiguration
    {
        [Config("txtUri", ControlType.TextBox)]
        public string Uri { get; set; }
        public override string ToString()
        {
            return "VLC Remote";
        }
        public override IFileContainer GetFilesContainer()
        {
            return new VLCRemoteMediaPlayer(this);
        }
        public override IConfigurationForm GetConfigurationForm()
        {
            return new VLCRemoteConfigForm();
        }
        public override string ExtraInfo()
        {
            return "Server: " + Uri;
        }
    }
}
