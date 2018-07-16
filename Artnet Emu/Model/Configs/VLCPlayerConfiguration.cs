using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model.Configs
{
    [Serializable]
    public class VLCPlayerConfiguration : PlayerConfiguration
    {
        [Config("checkBoxAlwaysAdd", ControlType.CheckBox)]
        public bool AlwaysAddFiles { get; set; }
        [Config("txtPassword", ControlType.TextBox)]
        public string Password { get; set; }
        public override string ToString()
        {
            return "VLC";
        }
    }
}
