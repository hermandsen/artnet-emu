namespace ArtnetEmu
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.lblRemoteIP = new System.Windows.Forms.Label();
            this.txtRemoteIP = new System.Windows.Forms.TextBox();
            this.btnStartListener = new System.Windows.Forms.Button();
            this.listConfigurations = new System.Windows.Forms.ListView();
            this.columnApplication = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddMediaPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuITunes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWinamp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVLCLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVLCRemote = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewFilelist = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewDuplicates = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewMissing = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.checkPlayingInfo = new System.Windows.Forms.CheckBox();
            this.timerFileinfo = new System.Windows.Forms.Timer(this.components);
            this.comboBoxLocalIP = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.checkAutoListen = new System.Windows.Forms.CheckBox();
            this.contextMenuListView.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblRemoteIP
            // 
            this.lblRemoteIP.AutoSize = true;
            this.lblRemoteIP.Location = new System.Drawing.Point(12, 42);
            this.lblRemoteIP.Name = "lblRemoteIP";
            this.lblRemoteIP.Size = new System.Drawing.Size(57, 13);
            this.lblRemoteIP.TabIndex = 0;
            this.lblRemoteIP.Text = "Remote IP";
            // 
            // txtRemoteIP
            // 
            this.txtRemoteIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteIP.Location = new System.Drawing.Point(75, 39);
            this.txtRemoteIP.Name = "txtRemoteIP";
            this.txtRemoteIP.Size = new System.Drawing.Size(151, 20);
            this.txtRemoteIP.TabIndex = 1;
            this.txtRemoteIP.Text = "127.0.0.1";
            // 
            // btnStartListener
            // 
            this.btnStartListener.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartListener.Location = new System.Drawing.Point(232, 12);
            this.btnStartListener.Name = "btnStartListener";
            this.btnStartListener.Size = new System.Drawing.Size(112, 47);
            this.btnStartListener.TabIndex = 3;
            this.btnStartListener.Text = "Start listener";
            this.btnStartListener.UseVisualStyleBackColor = true;
            this.btnStartListener.Click += new System.EventHandler(this.btnStartListener_Click);
            // 
            // listConfigurations
            // 
            this.listConfigurations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listConfigurations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnApplication,
            this.columnAddress,
            this.columnDescription});
            this.listConfigurations.ContextMenuStrip = this.contextMenuListView;
            this.listConfigurations.FullRowSelect = true;
            this.listConfigurations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listConfigurations.HideSelection = false;
            this.listConfigurations.Location = new System.Drawing.Point(15, 88);
            this.listConfigurations.MultiSelect = false;
            this.listConfigurations.Name = "listConfigurations";
            this.listConfigurations.Size = new System.Drawing.Size(329, 177);
            this.listConfigurations.TabIndex = 4;
            this.listConfigurations.UseCompatibleStateImageBehavior = false;
            this.listConfigurations.View = System.Windows.Forms.View.Details;
            // 
            // columnApplication
            // 
            this.columnApplication.Text = "Application";
            this.columnApplication.Width = 81;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 80;
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 154;
            // 
            // contextMenuListView
            // 
            this.contextMenuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddMediaPlayer,
            this.menuSeperator1,
            this.menuViewFilelist,
            this.menuViewDuplicates,
            this.menuViewMissing,
            this.menuEditConfiguration,
            this.menuSeperator2,
            this.menuDelete});
            this.contextMenuListView.Name = "contextMenuListView";
            this.contextMenuListView.Size = new System.Drawing.Size(170, 148);
            this.contextMenuListView.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuListView_Opening);
            // 
            // menuAddMediaPlayer
            // 
            this.menuAddMediaPlayer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuITunes,
            this.menuWinamp,
            this.menuVLCLocal,
            this.menuVLCRemote});
            this.menuAddMediaPlayer.Name = "menuAddMediaPlayer";
            this.menuAddMediaPlayer.Size = new System.Drawing.Size(169, 22);
            this.menuAddMediaPlayer.Text = "Add media player";
            // 
            // menuITunes
            // 
            this.menuITunes.Name = "menuITunes";
            this.menuITunes.Size = new System.Drawing.Size(139, 22);
            this.menuITunes.Text = "iTunes";
            this.menuITunes.Click += new System.EventHandler(this.menuITunes_Click);
            // 
            // menuWinamp
            // 
            this.menuWinamp.Name = "menuWinamp";
            this.menuWinamp.Size = new System.Drawing.Size(139, 22);
            this.menuWinamp.Text = "Winamp";
            this.menuWinamp.Click += new System.EventHandler(this.menuWinamp_Click);
            // 
            // menuVLCLocal
            // 
            this.menuVLCLocal.Name = "menuVLCLocal";
            this.menuVLCLocal.Size = new System.Drawing.Size(139, 22);
            this.menuVLCLocal.Text = "VLC Local";
            this.menuVLCLocal.Click += new System.EventHandler(this.menuVLCLocal_Click);
            // 
            // menuVLCRemote
            // 
            this.menuVLCRemote.Name = "menuVLCRemote";
            this.menuVLCRemote.Size = new System.Drawing.Size(139, 22);
            this.menuVLCRemote.Text = "VLC Remote";
            this.menuVLCRemote.Click += new System.EventHandler(this.menuVLCRemote_Click);
            // 
            // menuSeperator1
            // 
            this.menuSeperator1.Name = "menuSeperator1";
            this.menuSeperator1.Size = new System.Drawing.Size(166, 6);
            // 
            // menuViewFilelist
            // 
            this.menuViewFilelist.Name = "menuViewFilelist";
            this.menuViewFilelist.Size = new System.Drawing.Size(169, 22);
            this.menuViewFilelist.Text = "View filelist";
            this.menuViewFilelist.Click += new System.EventHandler(this.menuViewFilelist_Click);
            // 
            // menuViewDuplicates
            // 
            this.menuViewDuplicates.Name = "menuViewDuplicates";
            this.menuViewDuplicates.Size = new System.Drawing.Size(169, 22);
            this.menuViewDuplicates.Text = "View duplicates";
            this.menuViewDuplicates.Click += new System.EventHandler(this.menuViewDuplicates_Click);
            // 
            // menuViewMissing
            // 
            this.menuViewMissing.Name = "menuViewMissing";
            this.menuViewMissing.Size = new System.Drawing.Size(169, 22);
            this.menuViewMissing.Text = "View missing";
            this.menuViewMissing.Click += new System.EventHandler(this.menuViewMissing_Click);
            // 
            // menuEditConfiguration
            // 
            this.menuEditConfiguration.Name = "menuEditConfiguration";
            this.menuEditConfiguration.Size = new System.Drawing.Size(169, 22);
            this.menuEditConfiguration.Text = "Edit configuration";
            this.menuEditConfiguration.Click += new System.EventHandler(this.menuEditConfiguration_Click);
            // 
            // menuSeperator2
            // 
            this.menuSeperator2.Name = "menuSeperator2";
            this.menuSeperator2.Size = new System.Drawing.Size(166, 6);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(169, 22);
            this.menuDelete.Text = "Delete";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // lblNetwork
            // 
            this.lblNetwork.AutoSize = true;
            this.lblNetwork.Location = new System.Drawing.Point(12, 15);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(47, 13);
            this.lblNetwork.TabIndex = 16;
            this.lblNetwork.Text = "Network";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusDropDownButton});
            this.statusStrip.Location = new System.Drawing.Point(0, 268);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(356, 22);
            this.statusStrip.TabIndex = 19;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusDropDownButton
            // 
            this.toolStripStatusDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStatusDropDownButton.Name = "toolStripStatusDropDownButton";
            this.toolStripStatusDropDownButton.Size = new System.Drawing.Size(52, 20);
            this.toolStripStatusDropDownButton.Text = "Status";
            // 
            // checkPlayingInfo
            // 
            this.checkPlayingInfo.AutoSize = true;
            this.checkPlayingInfo.Location = new System.Drawing.Point(15, 65);
            this.checkPlayingInfo.Name = "checkPlayingInfo";
            this.checkPlayingInfo.Size = new System.Drawing.Size(150, 17);
            this.checkPlayingInfo.TabIndex = 20;
            this.checkPlayingInfo.Text = "Display playing information";
            this.checkPlayingInfo.UseVisualStyleBackColor = true;
            this.checkPlayingInfo.CheckedChanged += new System.EventHandler(this.checkPlayingInfo_CheckedChanged);
            // 
            // timerFileinfo
            // 
            this.timerFileinfo.Interval = 1000;
            this.timerFileinfo.Tick += new System.EventHandler(this.timerFileinfo_Tick);
            // 
            // comboBoxLocalIP
            // 
            this.comboBoxLocalIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLocalIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLocalIP.FormattingEnabled = true;
            this.comboBoxLocalIP.Location = new System.Drawing.Point(75, 12);
            this.comboBoxLocalIP.Name = "comboBoxLocalIP";
            this.comboBoxLocalIP.Size = new System.Drawing.Size(118, 21);
            this.comboBoxLocalIP.TabIndex = 21;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Image = global::ArtnetEmu.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(199, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(27, 23);
            this.btnRefresh.TabIndex = 22;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // checkAutoListen
            // 
            this.checkAutoListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAutoListen.AutoSize = true;
            this.checkAutoListen.Location = new System.Drawing.Point(240, 65);
            this.checkAutoListen.Name = "checkAutoListen";
            this.checkAutoListen.Size = new System.Drawing.Size(104, 17);
            this.checkAutoListen.TabIndex = 23;
            this.checkAutoListen.Text = "Listen on startup";
            this.checkAutoListen.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 290);
            this.Controls.Add(this.checkAutoListen);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.comboBoxLocalIP);
            this.Controls.Add(this.checkPlayingInfo);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnStartListener);
            this.Controls.Add(this.lblNetwork);
            this.Controls.Add(this.listConfigurations);
            this.Controls.Add(this.txtRemoteIP);
            this.Controls.Add(this.lblRemoteIP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Artnet Emu";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.contextMenuListView.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblRemoteIP;
        private System.Windows.Forms.TextBox txtRemoteIP;
        private System.Windows.Forms.Button btnStartListener;
        private System.Windows.Forms.ListView listConfigurations;
        private System.Windows.Forms.ColumnHeader columnApplication;
        private System.Windows.Forms.ColumnHeader columnAddress;
        private System.Windows.Forms.ContextMenuStrip contextMenuListView;
        private System.Windows.Forms.ToolStripMenuItem menuAddMediaPlayer;
        private System.Windows.Forms.ToolStripMenuItem menuITunes;
        private System.Windows.Forms.ToolStripMenuItem menuVLCRemote;
        private System.Windows.Forms.ToolStripMenuItem menuWinamp;
        private System.Windows.Forms.ToolStripMenuItem menuVLCLocal;
        private System.Windows.Forms.ToolStripMenuItem menuEditConfiguration;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripMenuItem menuViewFilelist;
        private System.Windows.Forms.ToolStripMenuItem menuViewDuplicates;
        private System.Windows.Forms.ToolStripSeparator menuSeperator1;
        private System.Windows.Forms.ToolStripSeparator menuSeperator2;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private System.Windows.Forms.ToolStripMenuItem menuViewMissing;
        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripStatusDropDownButton;
        private System.Windows.Forms.CheckBox checkPlayingInfo;
        private System.Windows.Forms.Timer timerFileinfo;
        private System.Windows.Forms.ComboBox comboBoxLocalIP;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.CheckBox checkAutoListen;
    }
}

