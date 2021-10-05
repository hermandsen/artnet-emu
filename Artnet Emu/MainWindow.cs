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
using ArtnetEmu.Model.ArtnetPackets;
using System.Timers;
using ArtnetEmu.Libraries;
using System.Net.NetworkInformation;

namespace ArtnetEmu
{
    public enum StatusType
    {
        PacketsPerSecond,
        UniverseSequence,
        LightingController
    }
    public partial class MainWindow : Form
    {
        protected ApplicationConfiguration Config;
        protected MediaPlayerManager Manager;
        protected Dictionary<ushort, ToolStripItem> Indicators = new Dictionary<ushort, ToolStripItem>();
        protected long LastReceivedPacketTick = 0;
        protected bool PacketSequence = true;
        private int PacketCounter = 0;
        private StatusType StatusIndicator = StatusType.PacketsPerSecond;
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

            CreateStatusMenu();

            timerFileinfo.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["FileinfoPullMiliseconds"]);
            listConfigurations.Items.Clear();
            txtRemoteIP.Text = Config.SenderIP;
            checkAutoListen.Checked = Config.ListenOnStartup;
            RefreshNetworkInterfaces();
            menuITunes.Enabled = false;
            switch (Config.WindowState)
            {
                case FormWindowState.Maximized:
                    WindowState = FormWindowState.Maximized;
                    break;
                case FormWindowState.Minimized:
                case FormWindowState.Normal:
                default:
                    Width = Math.Min(Math.Max(Config.Width, 367), 600);
                    Height = Math.Min(Math.Max(Config.Height, 286), 600);
                    Location = Config.Location;
                    break;
            }
            Text += " v." + this.GetVersion();
            foreach (PlayerConfiguration config in Config.Items)
            {
                AddConfigurationToListView(config);
            }

            if (Config.ListenOnStartup)
            {
                btnStartListener_Click(sender, e);
            }
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private void CreateStatusMenu()
        {
            var regex = new System.Text.RegularExpressions.Regex("(?<!^)[A-Z]");
            foreach (StatusType value in Enum.GetValues(typeof(StatusType)))
            {
                string text = value.ToString();
                text = regex.Replace(text, (x) => " " + x.Value.ToLower());
                var strip = toolStripStatusDropDownButton.DropDownItems.Add(text);
                strip.Tag = value;
                strip.Click += toolStripStatusClick;
            }
            UpdateStatusMenu();
        }

        private void UpdateStatusMenu()
        {
            var regex = new System.Text.RegularExpressions.Regex("(?<!^)[A-Z]");
            string text = StatusIndicator.ToString();
            text = regex.Replace(text, (x) => " " + x.Value.ToLower());
            toolStripStatusDropDownButton.Text = text;
        }

        private void RefreshNetworkInterfaces()
        {
            comboBoxLocalIP.Items.Clear();
            comboBoxLocalIP.Items.Add(new NetworkInterfaceItem("Any", IPAddress.Any));
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var item = new NetworkInterfaceItem(ni);
                comboBoxLocalIP.Items.Add(item);
                if (item.Name == Config.NetworkInterface)
                {
                    comboBoxLocalIP.SelectedItem = item;
                }
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
            if (txtRemoteIP.Text == "")
            {
                txtRemoteIP.Text = "0.0.0.0";
            }
            IPAddress remoteIP;
            IPAddress localIP;
            try
            {
                remoteIP = IPAddress.Parse(txtRemoteIP.Text);
                if (comboBoxLocalIP.SelectedItem == null)
                {
                    comboBoxLocalIP.SelectedItem = comboBoxLocalIP.Items[0];
                }
                localIP = ((NetworkInterfaceItem)comboBoxLocalIP.SelectedItem).Address;
            }
            catch (Exception exception)
            {
                ShowMessageBox(exception.Message);
                return;
            }

            EnableInterface(false);
            Application.DoEvents();
            if (Manager == null)
            {
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["ArtnetPort"]);
                IPEndPoint remote = new IPEndPoint(remoteIP, 0);
                IPEndPoint local = new IPEndPoint(localIP, port);
                var receiver = new ArtnetReceiver(remote, local);
                Manager = new MediaPlayerManager(receiver, Config.Items);
                Manager.OnThreadStateChanged += Server_ThreadState;
                receiver.OnArtDMXPacketReceived += Server_PacketReceived;
                receiver.OnArtPollReplyPacketReceived += Server_PollReplyPackedReceived;
                Manager.Start();
            }
            else
            {
                timerFileinfo.Enabled = false;
                Manager.Stop();
            }
        }

        private void EnableInterface(bool value)
        {
            btnStartListener.Enabled = value;
            comboBoxLocalIP.Enabled = value;
            txtRemoteIP.Enabled = value;
            btnRefresh.Enabled = value;
        }

        private void Server_PollReplyPackedReceived(ArtPollReplyPacket packet)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ArtPollPacketReceived(Server_PollReplyPackedReceived), packet);
            }
            else if (StatusIndicator == StatusType.LightingController)
            {
                if (!Indicators.Any())
                {
                    Indicators[0] = statusStrip.Items.Add("");
                }
                Indicators[0].Text = string.Format("{0} ({1})", packet.LongName, packet.IpAddress);
            }
        }

        private void Server_PacketReceived(ArtDMXPacket packet)
        {
            switch (StatusIndicator)
            {
                case StatusType.PacketsPerSecond:
                    var tick = DateTime.Now.Ticks;
                    PacketCounter++;
                    long dif = tick - LastReceivedPacketTick;
                    if (dif >= 10000000) // 1000 ms
                    {
                        LastReceivedPacketTick = tick;
                        InvokePacketsPerSecond(dif, PacketCounter);
                        PacketCounter = 0;
                    }
                    break;
                case StatusType.UniverseSequence:
                    InvokePacketReceived(packet);
                    break;
                case StatusType.LightingController:
                    break;
            }
        }

        private void InvokePacketsPerSecond(long ticks, int packetCounter)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PacketsPerSecondDelegate(InvokePacketsPerSecond), ticks, packetCounter);
            }
            else
            {
                if (!Indicators.Any())
                {
                    Indicators[0] = statusStrip.Items.Add("");
                }
                Indicators[0].Text = string.Format("{0,0:0.0} packets/second", packetCounter / (ticks / 10000000F));
            }
        }

        private void InvokePacketReceived(ArtDMXPacket packet)
        {
            if (this.InvokeRequired)
            {
                Invoke(new PacketReceivedDelegate(InvokePacketReceived), packet);
            }
            else
            {
                //lblStatus.Text = "Universe data: ";
                string txt = string.Format("{0}:{1}", packet.Universe, packet.Sequence);
                if (Indicators.ContainsKey(packet.Universe))
                {
                    Indicators[packet.Universe].Text = txt;
                }
                else
                {
                    int distance = 256*256;
                    int index = statusStrip.Items.Count;
                    foreach (ushort key in Indicators.Keys)
                    {
                        int dif = key - packet.Universe;
                        if (dif > 0 && dif < distance)
                        {
                            distance = dif;
                            index = statusStrip.Items.IndexOf(Indicators[key]);
                        }
                    }
                    ToolStripItem item = new ToolStripLabel(txt);
                    Indicators[packet.Universe] = item;
                    statusStrip.Items.Insert(index, item);
                }
            }
        }

        private delegate void ShowMessageBoxDelegate(string message);
        private delegate void PacketReceivedDelegate(ArtDMXPacket packet);
        private delegate void PacketsPerSecondDelegate(long ticks, int packetCounter);
        private delegate void ArtPollPacketReceived(ArtPollReplyPacket packet);
        private void Server_ThreadState(ArtnetServerState state, string message)
        {
            if (this.InvokeRequired)
            {
                Invoke(new ThreadStateChangedEvent(Server_ThreadState), state, message);
            }
            else
            {
                btnStartListener.Text = state == ArtnetServerState.Running ? "Stop listener" : "Start listener";
                //lblStatus.Text = Enum.GetName(typeof (ArtnetServerState), state);
                btnStartListener.Enabled = true;
                if (state == ArtnetServerState.Running)
                {
                    if (StatusIndicator == StatusType.LightingController && Manager != null)
                    {
                        Manager.Server.SendArtPollPacket();
                    }
                }
                else
                {
                    EnableInterface(true);
                    if (Manager != null)
                    {
                        Manager.OnThreadStateChanged -= Server_ThreadState;
                    }
                    Manager = null;
                    ClearStatus();
                }
                checkPlayingInfo_CheckedChanged(null, null);
                if (message != null)
                {
                    ShowMessageBox(message);
                }
            }
        }

        private void ClearStatus()
        {
            foreach (var x in Indicators.Values)
            {
                x.Dispose();
            }
            Indicators.Clear();
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
            timerFileinfo.Enabled = false;
            if (Manager != null)
            {
                Manager.Stop();
            }
            Config.SenderIP = txtRemoteIP.Text;
            Config.ListenOnStartup = checkAutoListen.Checked;
            var item = comboBoxLocalIP.SelectedItem as NetworkInterfaceItem;
            if (item != null)
            {
                Config.NetworkInterface = item.Name;
            }
            Config.Width = Width;
            Config.Height = Height;
            Config.Location = Location;
            Config.WindowState = WindowState;
            Config.Save();
        }

        private void contextMenuListView_Opening(object sender, CancelEventArgs e)
        {
            bool visible = listConfigurations.SelectedItems.Count >= 1;
            PlayerConfiguration configuration = visible ? (PlayerConfiguration)listConfigurations.SelectedItems[0].Tag : null;
            menuViewMissing.Visible = visible && configuration.FileScanMethod == FileScanMethod.Filelist && !(configuration is VLCRemotePlayerConfiguration);
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

        private void toolStripStatusClick(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            var status = (StatusType)item.Tag;

            StatusIndicator = status;

            PacketCounter = 0;
            LastReceivedPacketTick = DateTime.Now.Ticks;
            ClearStatus();

            if (Manager != null) {
                Manager.Server.SendArtPollPacket();
            }

            UpdateStatusMenu();
        }

        private void checkPlayingInfo_CheckedChanged(object sender, EventArgs e)
        {
            timerFileinfo.Enabled = checkPlayingInfo.Checked && Manager != null;
            if (timerFileinfo.Enabled)
            {
                timerFileinfo_Tick(null, null);
            }
            else
            {
                ResetFileinfo();
            }
        }

        private void timerFileinfo_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Manager.Players.Count; i++)
            {
                DisplayFileinfo(Manager.Players[i], listConfigurations.Items[i]);
            }
        }

        private void DisplayFileinfo(IPlayingMediaPlayer player, ListViewItem item)
        {
            string filename = player.GetPlayingFilename();
            if (filename != null)
            {
                var index = player.GetIndex(filename);
                item.SubItems[2].Text = string.Format(ConfigurationManager.AppSettings["FileinfoDescriptor"],
                    index == null ? "?" : index.Group.ToString(),
                    index == null ? "?" : index.FileIndex.ToString(),
                    player.GetPlayingTitle()
                );
            }
        }
        private void ResetFileinfo()
        {
            if (!InvokeRequired)
            {
                for (int i = 0; i < Math.Min(listConfigurations.Items.Count, Config.Items.Count); i++)
                {
                    listConfigurations.Items[i].SubItems[2].Text = Config.Items[i].ExtraInfo();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var item = (NetworkInterfaceItem)comboBoxLocalIP.SelectedItem;
            if (item != null)
            {
                Config.NetworkInterface = item.Name;
            }
            RefreshNetworkInterfaces();
        }
    }
}
