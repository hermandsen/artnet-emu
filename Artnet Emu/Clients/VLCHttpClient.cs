using ArtnetEmu.Exceptions;
using ArtnetEmu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ArtnetEmu
{
    public enum SortOrder
    {
        Id = 0,
        Name = 1,
        Author = 3,
        Random = 5,
        TrackNumber = 7
    }
    public enum ServiceDiscoveryModule
    {
        Sap,
        Shoutcast,
        Podcast,
        Hal
    }
    // currently missing from api:
    // Seek by percent
    // Receiving equalizer bands
    // Select title
    // Select chapter
    // Select audio track
    // Select video track
    // Select subtitle track
    public class VLCHttpClient
    {
        private string BaseUrl;
        private string Authorization;
        public VLCHttpClient(string baseUrl, string password)
        {
            BaseUrl = baseUrl;
            Authorization = "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", password)));
        }

        private ClassModel MakeRequest<ClassModel>(string uri)
        {
            HttpWebRequest request = WebRequest.Create(BaseUrl + uri) as HttpWebRequest;
            request.Headers.Add("Authorization", Authorization);
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                XmlSerializer serializer = new XmlSerializer(typeof(ClassModel));
                return (ClassModel)serializer.Deserialize(response.GetResponseStream());
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ConnectFailure)
                {
                    throw new ServiceUnavailableException("Unable to connect to VLC.\nVLC must be open, and web must be enabled in Interface->Main interfaces.", e);
                }
                else if (e.Response != null && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceUnavailableException("Unable to connect to VLC.\nPlease set the http password in Interface->Main interface->Lua.", e);
                }
                else
                {
                    throw new ServiceUnavailableException("Unable to connect to VLC.", e);
                }
            }
        }

        private VLCStatusModel MakeStatusRequest(string command, params string[] args)
        {
            if (args.Length > 0)
            {
                command += "&" + String.Join("&", args);
            }
            return MakeRequest<VLCStatusModel>("requests/status.xml?command=" + command);
        }

        public VLCBrowseModel BrowseDir(string uri)
        {
            return MakeBrowseRequest("uri="+uri);
        }

        public List<BrowseElement> BrowseDirRecursive(string uri)
        {
            List<BrowseElement> result = new List<BrowseElement>();
            VLCBrowseModel root = BrowseDir(uri);
            foreach (BrowseElement element in root.Elements)
            {
                switch (element.Type)
                {
                    case "dir":
                        if (element.Name != "..")
                        {
                            result.AddRange(BrowseDirRecursive(element.Uri));
                        }
                        break;
                    case "file":
                        result.Add(element);
                        break;
                }
            }
            return result;
        }

        private VLCPlaylistModel MakePlaylistRequest()
        {
            return MakeRequest<VLCPlaylistModel>("requests/playlist.xml");
        }

        private VLCBrowseModel MakeBrowseRequest(params string[] args)
        {
            string queryString = String.Join("&", args);
            return MakeRequest<VLCBrowseModel>("requests/browse.xml?" + queryString);
        }

        public string GetPlaylistId(string filename)
        {
            VLCPlaylistModel playlist = MakePlaylistRequest();
            List<string> ids = new List<string>();
            int currentIndex = -1;
            foreach (var list in playlist.Lists)
            {
                foreach (var file in list.Files)
                {
                    if (file.Current == "current")
                    {
                        currentIndex = ids.Count;
                    }
                    if (file.Uri == filename)
                    {
                        ids.Add(file.Id);
                    }
                }
            }
            if (ids.Count > 0)
            {
                if (currentIndex == -1 || currentIndex >= ids.Count)
                {
                    currentIndex = 0;
                }
                return ids[currentIndex];
            }
            return "";
        }

        private Leaf GetPlayingNode()
        {
            VLCPlaylistModel playlist = MakePlaylistRequest();
            foreach (var list in playlist.Lists)
            {
                foreach (var file in list.Files)
                {
                    if (file.Current == "current")
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        public string GetPlayingFilename()
        {
            Leaf leaf = GetPlayingNode();
            return leaf?.Uri;
        }

        public string GetPlayingTitle()
        {
            Leaf leaf = GetPlayingNode();
            return leaf?.Name;
        }

        public VLCStatusModel AddToPlaylistAndPlay(string fileUri)
        {
            return MakeStatusRequest("in_play", "input=" + System.Web.HttpUtility.UrlEncode(fileUri));
        }
        public VLCStatusModel AddToPlaylist(string fileUri)
        {
            return MakeStatusRequest("in_enqueue", "input=" + System.Web.HttpUtility.UrlEncode(fileUri));
        }
        public VLCStatusModel AddSubtitleToPlayingFile(string fileUri)
        {
            return MakeStatusRequest("addsubtitle", "val=" + System.Web.HttpUtility.UrlEncode(fileUri));
        }
        public VLCStatusModel PlayPlaylistItem(string id = "")
        {
            return MakeStatusRequest("pl_play", id == "" ? "" : "id=" + id);
        }
        public VLCStatusModel TogglePause(int id = 0)
        {
            return MakeStatusRequest("pl_pause", id == 0 ? "" : "id=" + id.ToString());
        }
        public VLCStatusModel ResumePlayback()
        {
            return MakeStatusRequest("pl_forceresume");
        }
        public VLCStatusModel Pause()
        {
            return MakeStatusRequest("pl_forcepause");
        }
        public VLCStatusModel Stop()
        {
            return MakeStatusRequest("pl_stop");
        }
        public VLCStatusModel Next()
        {
            return MakeStatusRequest("pl_next");
        }
        public VLCStatusModel Previous()
        {
            return MakeStatusRequest("pl_previous");
        }
        public VLCStatusModel DeleteFromPlaylist(int id)
        {
            return MakeStatusRequest("pl_delete", "id=" + id.ToString());
        }
        public VLCStatusModel EmptyPlaylist()
        {
            return MakeStatusRequest("pl_empty");
        }
        public VLCStatusModel SetAudioDelay(float delayInSeconds)
        {
            return MakeStatusRequest("audiodelay", "val=" + delayInSeconds.ToString());
        }
        public VLCStatusModel SetSubtitleDelay(float delayInSeconds)
        {
            return MakeStatusRequest("subdelay", "val=" + delayInSeconds.ToString());
        }
        public VLCStatusModel SetPlaybackRate(float rate)
        {
            if (rate <= 0)
            {
                throw new ArgumentException("Playback rate must be greater than zero.");
            }
            return MakeStatusRequest("rate", "val=" + rate.ToString());
        }
        public VLCStatusModel SetAspectRatio(string aspect)
        {
            string[] list = "1:1,4:3,5:4,16:9,16:10,221:100,235:100,239:100".Split(',');
            if (!list.Contains(aspect))
            {
                throw new ArgumentException("Invalid aspect ratio. Valid aspects are: 1:1 , 4:3 , 5:4 , 16:9 , 16:10 , 221:100 , 235:100 , 239:100");
            }
            aspect = System.Web.HttpUtility.UrlEncode(aspect);
            return MakeStatusRequest("aspectratio", "val="+aspect);
        }
        public VLCStatusModel SortPlaylist(SortOrder order, bool reverse = false)
        {
            return MakeStatusRequest("pl_sort", "id=" + Convert.ToInt32(!reverse).ToString() + "&val=" + ((int)order).ToString());
        }
        public VLCStatusModel ToggleRandom()
        {
            return MakeStatusRequest("pl_random");
        }
        public VLCStatusModel ToggleLoop()
        {
            return MakeStatusRequest("pl_loop");
        }
        public VLCStatusModel ToggleRepeat()
        {
            return MakeStatusRequest("pl_repeat");
        }
        public VLCStatusModel ToggleEnableServiceDiscoveryModule(ServiceDiscoveryModule module)
        {
            return MakeStatusRequest("pl_sd", "val=" + module.ToString().ToLower());
        }
        public VLCStatusModel ToggleFullscreen()
        {
            return MakeStatusRequest("fullscreen");
        }
        public VLCStatusModel SetVolume(uint value)
        {
            return MakeStatusRequest("volume", "val=" + value.ToString());
        }
        public VLCStatusModel SetVolumeRelative(int value)
        {
            string stringValue = value.ToString();
            if (value >= 0)
            {
                stringValue = "+" + stringValue;
            }
            return MakeStatusRequest("volume", "val=" + stringValue);
        }
        public VLCStatusModel SetVolumePercent(int value)
        {
            return MakeStatusRequest("volume", "val=" + value.ToString() + System.Web.HttpUtility.UrlEncode("%"));
        }
        public VLCStatusModel Seek(uint seconds)
        {
            return MakeStatusRequest("seek", "val=" + seconds.ToString());
        }
        public VLCStatusModel SeekRelative(int seconds)
        {
            string stringValue = seconds.ToString();
            if (seconds >= 0)
            {
                stringValue = "+" + stringValue;
            }
            return MakeStatusRequest("seek", "val=" + stringValue);
        }
        public VLCStatusModel Seek(uint hours, uint minutes, uint seconds = 0)
        {
            return MakeStatusRequest("seek", "val=" + System.Web.HttpUtility.UrlEncode(hours.ToString() + "H:" + minutes + "M:" + seconds.ToString()));
        }
        public VLCStatusModel SeekRelative(int hours, int minutes, int seconds)
        {
            seconds += hours * 3600 + minutes * 60;
            string value = seconds < 0 ? "-" : "+";
            seconds = Math.Abs(seconds);
            value += (seconds / 3600).ToString() + "H:";
            seconds = seconds % 3600;
            value += (seconds / 60).ToString() + "M:";
            seconds = seconds % 60;
            value += seconds.ToString() + "S";
            return MakeStatusRequest("seek", "val=" + System.Web.HttpUtility.UrlEncode(value));
        }
        public VLCStatusModel SetPreamp(float db)
        {
            if (db < -20 || db > 20)
            {
                throw new ArgumentException("Preamp must be between -20 and 20");
            }
            return MakeStatusRequest("preamp", "val=" + db.ToString());
        }
        public VLCStatusModel SetEqualizer(int band, float db)
        {
            if (band < 0 || band > 9)
            {
                throw new ArgumentException("Use an equalizer band between 0-9");
            }
            if (db < -20 || db > 20)
            {
                throw new ArgumentException("Equalizer value must be between -20 and 20");
            }
            return MakeStatusRequest("equalizer", "band=" + band.ToString() + "&val=" + db.ToString());
        }
        public VLCStatusModel EnableEqualizer(bool enabled)
        {
            return MakeStatusRequest("enableeq", "val=" + Convert.ToInt32(enabled).ToString());
        }
        public VLCStatusModel SetEqualizerPreset(int id)
        {
            return MakeStatusRequest("setpreset", "val=" + id);
        }
    }
}
