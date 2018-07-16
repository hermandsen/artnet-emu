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
    public class VLCRemoteMediaPlayer : VLCMediaPlayer<VLCRemotePlayerConfiguration>
    {
        public VLCRemoteMediaPlayer(VLCRemotePlayerConfiguration config) : base(config) { }

        protected override string Url
        {
            get
            {
                return config.Uri;
            }
        }

        protected override void LoadFiles()
        {
            List<BrowseElement> list = Client.BrowseDirRecursive(config.FolderPath);
            Regex regex = null;
            switch (config.FileScanMethod)
            {
                case FileScanMethod.Filestructure:
                    regex = new Regex(@"(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^/]*/[^/]*?(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d).*?\.[A-Za-z0-9]+$");
                    break;
                case FileScanMethod.Regex:
                    regex = new Regex(config.Regex);
                    break;
                case FileScanMethod.Filelist:
                    // Not supported, you can't read filelists on a remote machine
                    return;
            }
            Dictionary<int, bool> added = new Dictionary<int, bool>();
            foreach (BrowseElement element in list)
            {
                string url = System.Web.HttpUtility.UrlDecode(element.Uri);
                Match match = regex.Match(url);
                if (match.Success)
                {
                    byte group = Convert.ToByte(match.Groups[1].Value);
                    byte index = Convert.ToByte(match.Groups[2].Value);
                    if (Files.Exists(group, index))
                    {
                        Duplicates.Add(new Exceptions.FileIndexItem(group, index, url));
                        if (!added.ContainsKey((group << 8) | index))
                        {
                            Duplicates.Add(new Exceptions.FileIndexItem(group, index, Files[group, index]));
                            added.Add((group << 8) | index, true);
                        }
                    }
                    else
                    {
                        Files.Add(group, index, url);
                    }
                }
            }
        }
    }
}
