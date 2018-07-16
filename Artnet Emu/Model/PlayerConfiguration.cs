using ArtnetEmu.Model.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public enum FileScanMethod : byte
    {
        Filelist,
        Filestructure,
        Regex
    }

    public enum FileEncoding : byte
    {
        Default,
        Ascii,
        UTF8
    }

    [Serializable]
    public class PlayerConfiguration
    {
        [Config("numericPhysical", ControlType.NumericUpDown, IntType.U1)]
        public byte Physical { get; set; }

        [Config("numericUniverse", ControlType.NumericUpDown, IntType.U2)]
        public ushort Universe { get; set; }

        [Config("numericAddress", ControlType.NumericUpDown, IntType.U2)]
        public ushort Address { get; set; }
        [Config("txtFolderPath", ControlType.TextBox)]
        public string FolderPath { get; set; }
        [Config("comboFileScanMethod", ControlType.ComboBox)]
        public FileScanMethod? FileScanMethod { get; set; }
        [Config("comboFileEncoding", ControlType.ComboBox)]
        public FileEncoding? FileEncoding { get; set; }
        [Config("txtFileScanRegex", ControlType.TextBox)]
        public string Regex { get; set; }
        public virtual IFileContainer GetFilesContainer()
        {
            throw new NotImplementedException("Must implement for new configurations.");
        }
        public virtual IConfigurationForm GetConfigurationForm()
        {
            return new ConfigForm();
        }
        public override string ToString()
        {
            return "Media player";
        }
        public virtual string ExtraInfo()
        {
            return "";
        }
    }
}
