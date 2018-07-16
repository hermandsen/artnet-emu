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
            this.lblArtNetIpServer = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
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
            this.pictureStatus = new System.Windows.Forms.PictureBox();
            this.contextMenuListView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // lblArtNetIpServer
            // 
            this.lblArtNetIpServer.AutoSize = true;
            this.lblArtNetIpServer.Location = new System.Drawing.Point(9, 9);
            this.lblArtNetIpServer.Name = "lblArtNetIpServer";
            this.lblArtNetIpServer.Size = new System.Drawing.Size(204, 13);
            this.lblArtNetIpServer.TabIndex = 0;
            this.lblArtNetIpServer.Text = "ArtNet sender (127.0.0.1 is local machine)";
            // 
            // txtIP
            // 
            this.txtIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIP.Location = new System.Drawing.Point(12, 25);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(237, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "127.0.0.1";
            // 
            // btnStartListener
            // 
            this.btnStartListener.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartListener.Location = new System.Drawing.Point(255, 9);
            this.btnStartListener.Name = "btnStartListener";
            this.btnStartListener.Size = new System.Drawing.Size(112, 36);
            this.btnStartListener.TabIndex = 2;
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
            this.listConfigurations.Location = new System.Drawing.Point(12, 51);
            this.listConfigurations.MultiSelect = false;
            this.listConfigurations.Name = "listConfigurations";
            this.listConfigurations.Size = new System.Drawing.Size(355, 247);
            this.listConfigurations.TabIndex = 3;
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
            this.columnDescription.Width = 190;
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
            // pictureStatus
            // 
            this.pictureStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureStatus.Image = ((System.Drawing.Image)(resources.GetObject("pictureStatus.Image")));
            this.pictureStatus.Location = new System.Drawing.Point(237, 9);
            this.pictureStatus.Name = "pictureStatus";
            this.pictureStatus.Size = new System.Drawing.Size(12, 12);
            this.pictureStatus.TabIndex = 15;
            this.pictureStatus.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 310);
            this.Controls.Add(this.pictureStatus);
            this.Controls.Add(this.listConfigurations);
            this.Controls.Add(this.btnStartListener);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblArtNetIpServer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Artnet Emu";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.contextMenuListView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblArtNetIpServer;
        private System.Windows.Forms.TextBox txtIP;
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
        private System.Windows.Forms.PictureBox pictureStatus;
        private System.Windows.Forms.ToolStripMenuItem menuViewMissing;
    }
}

