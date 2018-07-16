using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Xml;
using System.Threading;
using ArtnetEmu.Model;
using ArtnetEmu.Model.Configs;
using System.IO;
using System.Xml.Serialization;
using ArtnetEmu.Clients;
using ArtnetEmu.Exceptions;
using System.Reflection;
using System.Diagnostics;
using System.Deployment.Application;

namespace ArtnetEmu
{
    public partial class MainWindow : Form
    {
        protected ApplicationConfiguration Config;
        protected ArtnetServer Server;
        protected Thread ServerThread;
        protected System.Timers.Timer TimeoutTimer;
        protected long LastReceivedPacketTick = 0;
        protected bool PacketSequence = true;
        protected List<MediaPlayer<Fixture, PlayerConfiguration>> Players = new List<MediaPlayer<Fixture, PlayerConfiguration>>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Config = ApplicationConfiguration.Load();
            if (Config == null)
            {
                Config = new ApplicationConfiguration();
            }
            listConfigurations.Items.Clear();
            txtIP.Text = Config.Address;

            string version = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "debug";
            Text += " v." + version;
            foreach (PlayerConfiguration config in Config.Items)
            {
                AddConfigurationToListView(config);
            }
        }

        private void AddConfigurationToListView(PlayerConfiguration config)
        {
            ListViewItem item = new ListViewItem(config.ToString());
            item.SubItems.Add(config.Universe.ToString("D3") + "-" + config.Address.ToString("D3"));
            item.SubItems.Add(config.ExtraInfo());
            item.Tag = config;
            listConfigurations.Items.Add(item);
        }
    
        private void UpdateConfiguration(ListViewItem item, PlayerConfiguration oldConfig, PlayerConfiguration newConfig)
        {
            int index = Config.Items.IndexOf(oldConfig);
            Config.Items[index] = newConfig;
            item.Text = newConfig.ToString();
            item.SubItems[1].Text = newConfig.Universe.ToString("D3") + "-" + newConfig.Address.ToString("D3");
            item.SubItems[2].Text = newConfig.ExtraInfo();
            item.Tag = newConfig;
        }

        private void ShowConfigForm<Form, Configuration>() where Form : ConfigForm, new() where Configuration : PlayerConfiguration, new()
        {
            Form form = new Form();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Configuration config = form.GetConfiguration<Configuration>();
                AddConfigurationToListView(config);
                Config.Items.Add(config);
            }
        }

        private void menuITunes_Click(object sender, EventArgs e)
        {
            ShowConfigForm<ITunesConfigForm, ITunesPlayerConfiguration>();
        }

        private void menuVLCLocal_Click(object sender, EventArgs e)
        {
            ShowConfigForm<VLCLocalConfigForm, VLCLocalPlayerConfiguration>();
        }

        private void menuVLCRemote_Click(object sender, EventArgs e)
        {
            ShowConfigForm<VLCRemoteConfigForm, VLCRemotePlayerConfiguration>();
        }

        private void menuWinamp_Click(object sender, EventArgs e)
        {
            ShowConfigForm<WinampConfigForm, WinampPlayerConfiguration>();
        }

        private void btnStartListener_Click(object sender, EventArgs e)
        {
            if (txtIP.Text == "")
            {
                txtIP.Text = "127.0.0.1";
            }
            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(txtIP.Text);
            }
            catch (Exception exception)
            {
                ShowMessageBox(exception.Message);
                return;
            }
            btnStartListener.Enabled = false;
            Application.DoEvents();
            if (Server == null)
            {
                ServerThread = new Thread(new ThreadStart(StartServer));
                ServerThread.Start();
            }
            else
            {
                Server.StopThread();
                // give the server a change to terminate gracefully,
                // otherwise, just kill it after 3 seconds.
                TimeoutTimer = new System.Timers.Timer();
                TimeoutTimer.Elapsed += Timer_Elapsed;
                TimeoutTimer.Interval = 3000;
                TimeoutTimer.Enabled = true;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var t = (System.Timers.Timer)sender;
            t.Enabled = false;
            if (ServerThread != null)
            {
                ServerThread.Abort();
                Server_ThreadState(ArtnetServerState.Terminated);
                GC.Collect();
                btnStartListener.Enabled = true;
            }
        }

        private void StartServer()
        {
            Server = new ArtnetServer(IPAddress.Parse(txtIP.Text), Convert.ToInt32(ConfigurationManager.AppSettings["ArtnetPort"]));
            Server.ThreadState += Server_ThreadState;
            try {
                foreach (PlayerConfiguration config in Config.Items)
                {
                    switch (config)
                    {
                        case VLCLocalPlayerConfiguration x:
                            VLCLocalMediaPlayer vlcLocal = new VLCLocalMediaPlayer(x);
                            Server.PacketReceived += vlcLocal.PacketReceived;
                            break;
                        case VLCRemotePlayerConfiguration x:
                            VLCRemoteMediaPlayer vlcRemote = new VLCRemoteMediaPlayer(x);
                            Server.PacketReceived += vlcRemote.PacketReceived;
                            break;
                        case WinampPlayerConfiguration x:
                            WinampMediaPlayer winamp = new WinampMediaPlayer(x);
                            Server.PacketReceived += winamp.PacketReceived;
                            break;
                        case ITunesPlayerConfiguration x:
                            ITunesMediaPlayer itunes = new ITunesMediaPlayer(x);
                            Server.PacketReceived += itunes.PacketReceived;
                            break;
                        default:
                            throw new Exception("You're missing a configuration stupid.");
                    }
                }
                Server.PacketReceived += Server_PacketReceived;
                Server.Run();
            }
            catch (ServiceUnavailableException e)
            {
                ShowMessageBox(e.Message);
                Server_ThreadState(ArtnetServerState.Terminated);
            }
        }

        private void Server_PacketReceived(ArtNetPacket packet)
        {
            var tick = DateTime.Now.Ticks;
            if (tick - LastReceivedPacketTick > 10000000) // 1000 ms
            {
                LastReceivedPacketTick = tick;
                InvokePacketReceived(packet);
            }
        }

        private void InvokePacketReceived(ArtNetPacket packet)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PacketReceivedDelegate(InvokePacketReceived), packet);
            }
            else
            {
                if (pictureStatus.Image == null)
                {
                    Bitmap bmp = new Bitmap(pictureStatus.Width, pictureStatus.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Black);
                    }
                    pictureStatus.Image = bmp;
                }
                using (Graphics g = Graphics.FromImage(pictureStatus.Image))
                {
                    g.FillRectangle(PacketSequence ? Brushes.Green : Brushes.Black, new Rectangle(0, 0, pictureStatus.Width, pictureStatus.Height));
                    PacketSequence = !PacketSequence;
                }
                pictureStatus.Invalidate();
            }
        }

        private delegate void UpdateThreadStatusDelegate(ArtnetServerState state);
        private delegate void ShowMessageBoxDelegate(string message);
        private delegate void PacketReceivedDelegate(ArtNetPacket packet);
        private void Server_ThreadState(ArtnetServerState state)
        {
            if (this.InvokeRequired)
            {
                Invoke(new UpdateThreadStatusDelegate(Server_ThreadState), state);
            }
            else
            {
                btnStartListener.Text = state == ArtnetServerState.Running ? "Stop listener" : "Start listener";
                btnStartListener.Enabled = true;
                if (state != ArtnetServerState.Running)
                {
                    Server.Terminate();
                    Server = null;
                    ServerThread = null;
                    if (TimeoutTimer != null)
                    {
                        TimeoutTimer.Enabled = false;
                        TimeoutTimer = null;
                    }
                }
                if (state == ArtnetServerState.Aborted)
                {
                    ShowMessageBox("Another instance is blocking the port.\nThe program might be running in the background.");
                }
            }
        }

        public void ShowMessageBox(string message)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ShowMessageBoxDelegate(ShowMessageBox), message);
            }
            else
            {
                MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ServerThread != null)
            {
                ServerThread.Abort();
            }
            if (Server != null)
            {
                Server.StopThread();
                Server.Terminate();
                Server = null;
            }
            Config.Address = txtIP.Text;
            Config.Save();
        }

        private void contextMenuListView_Opening(object sender, CancelEventArgs e)
        {
            bool visible = listConfigurations.SelectedItems.Count >= 1;
            menuViewFilelist.Visible = visible;
            menuViewDuplicates.Visible = visible;
            menuEditConfiguration.Visible = visible;
            menuDelete.Visible = visible;
            menuSeperator1.Visible = visible;
            menuSeperator2.Visible = visible;
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            if (listConfigurations.SelectedItems.Count == 1)
            {
                ListViewItem item = listConfigurations.SelectedItems[0];
                var config = (PlayerConfiguration)item.Tag;
                Config.DeleteItem(config);
                item.Remove();
            }
        }

        private void menuViewFilelist_Click(object sender, EventArgs e)
        {
            if (listConfigurations.SelectedItems.Count == 1)
            {
                ListViewItem item = listConfigurations.SelectedItems[0];
                PlayerConfiguration config = (PlayerConfiguration)item.Tag;
                config.GetFilesContainer().ShowFiles();
            }
        }

        private void menuViewMissing_Click(object sender, EventArgs e)
        {
            if (listConfigurations.SelectedItems.Count == 1)
            {
                ListViewItem item = listConfigurations.SelectedItems[0];
                PlayerConfiguration config = (PlayerConfiguration)item.Tag;
                config.GetFilesContainer().ShowMissing();
            }
        }

        private void menuViewDuplicates_Click(object sender, EventArgs e)
        {
            if (listConfigurations.SelectedItems.Count == 1)
            {
                ListViewItem item = listConfigurations.SelectedItems[0];
                PlayerConfiguration config = (PlayerConfiguration)item.Tag;
                config.GetFilesContainer().ShowDuplicates();
            }
        }

        private void menuEditConfiguration_Click(object sender, EventArgs e)
        {
            if (listConfigurations.SelectedItems.Count == 1)
            {
                ListViewItem item = listConfigurations.SelectedItems[0];
                PlayerConfiguration oldConfig = (PlayerConfiguration)item.Tag;
                var form = oldConfig.GetConfigurationForm();
                form.SetConfiguration(oldConfig);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    PlayerConfiguration newConfig = null;
                    Type type = oldConfig.GetType();
                    if (type == typeof(VLCLocalPlayerConfiguration))
                    {
                        newConfig = form.GetConfiguration<VLCLocalPlayerConfiguration>();
                    } else if (type == typeof(VLCRemotePlayerConfiguration))
                    {
                        newConfig = form.GetConfiguration<VLCRemotePlayerConfiguration>();
                    } else if (type == typeof(WinampPlayerConfiguration))
                    {
                        newConfig = form.GetConfiguration<WinampPlayerConfiguration>();
                    }
                    else
                    {
                        throw new Exception("You're missing a configuration");
                    }
                    if (newConfig != null)
                    {
                        UpdateConfiguration(item, oldConfig, newConfig);
                    }
                }
            }
        }

    }
}
