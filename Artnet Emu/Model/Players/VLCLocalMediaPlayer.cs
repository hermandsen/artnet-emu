using ArtnetEmu.Exceptions;
using ArtnetEmu.Model.Configs;
using ArtnetEmu.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public class VLCLocalMediaPlayer : VLCMediaPlayer<VLCLocalPlayerConfiguration>
    {
        public VLCLocalMediaPlayer(VLCLocalPlayerConfiguration config) : base(config) { }

        protected override string Url
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["VLCLocalHttpUrl"]; }
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

    }
}
