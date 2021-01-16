using ArtnetEmu.Model.Configs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ArtnetEmu.Model
{
    public class ApplicationConfiguration
    {
        public List<PlayerConfiguration> Items;
        public string SenderIP;
        public string NetworkInterface;
        public int Width;
        public int Height;
        public Point Location;
        public FormWindowState WindowState;

        public ApplicationConfiguration()
        {
            Items = new List<PlayerConfiguration>();
            SenderIP = "127.0.0.1";
            NetworkInterface = "";
            Width = 378;
            Height = 329;
            Location = new Point(300, 200);
            WindowState = FormWindowState.Normal;
        }
        private static string GetConfigurationFilename()
        {
            string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Artnet Emu");
            Directory.CreateDirectory(filename);
            return Path.Combine(filename, "configurations.xml");
        }
        public void Save()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            TextWriter writer = new StreamWriter(GetConfigurationFilename());
            XmlSerializer xml = new XmlSerializer(this.GetType(), GetConfigurationTypes());
            xml.Serialize(writer, this);
            writer.Close();
        }
        private static Type[] GetConfigurationTypes()
        {
            return new Type[]
            {
                typeof(PlayerConfiguration),
                typeof(VLCPlayerConfiguration),
                typeof(VLCLocalPlayerConfiguration),
                typeof(VLCRemotePlayerConfiguration),
                typeof(WinampPlayerConfiguration),
                typeof(ITunesPlayerConfiguration)
            };
        }
        public bool DeleteItem(PlayerConfiguration config)
        {
            return Items.Remove(config);
        }
        public static ApplicationConfiguration Load()
        {
            ApplicationConfiguration result = null;
            string filename = GetConfigurationFilename();
            if (File.Exists(filename))
            {
                var f = new FileInfo(filename);
                if (f.Length == 0)
                {
                    f.Delete();
                }
                else
                {
                    try {
                        XmlSerializer xml = new XmlSerializer(typeof(ApplicationConfiguration), GetConfigurationTypes());
                        TextReader reader = new StreamReader(filename);
                        result = (ApplicationConfiguration)xml.Deserialize(reader);
                        reader.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result;
        }
    }
}
