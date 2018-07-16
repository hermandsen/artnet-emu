namespace ArtnetEmu
{
    partial class ConfigForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.numericPhysical = new System.Windows.Forms.NumericUpDown();
            this.lblPhysical = new System.Windows.Forms.Label();
            this.lblUniverse = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.numericUniverse = new System.Windows.Forms.NumericUpDown();
            this.numericAddress = new System.Windows.Forms.NumericUpDown();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.comboFileEncoding = new System.Windows.Forms.ComboBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.txtFileScanRegex = new System.Windows.Forms.TextBox();
            this.lblFileScanningMethod = new System.Windows.Forms.Label();
            this.comboFileScanMethod = new System.Windows.Forms.ComboBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericPhysical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // numericPhysical
            // 
            this.numericPhysical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericPhysical.Location = new System.Drawing.Point(137, 9);
            this.numericPhysical.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericPhysical.Name = "numericPhysical";
            this.numericPhysical.Size = new System.Drawing.Size(57, 20);
            this.numericPhysical.TabIndex = 1;
            // 
            // lblPhysical
            // 
            this.lblPhysical.AutoSize = true;
            this.lblPhysical.Location = new System.Drawing.Point(3, 11);
            this.lblPhysical.Name = "lblPhysical";
            this.lblPhysical.Size = new System.Drawing.Size(46, 13);
            this.lblPhysical.TabIndex = 0;
            this.lblPhysical.Text = "Physical";
            // 
            // lblUniverse
            // 
            this.lblUniverse.AutoSize = true;
            this.lblUniverse.Location = new System.Drawing.Point(3, 38);
            this.lblUniverse.Name = "lblUniverse";
            this.lblUniverse.Size = new System.Drawing.Size(49, 13);
            this.lblUniverse.TabIndex = 2;
            this.lblUniverse.Text = "Universe";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(3, 65);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(72, 13);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "DMX Address";
            // 
            // numericUniverse
            // 
            this.numericUniverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUniverse.Location = new System.Drawing.Point(137, 36);
            this.numericUniverse.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUniverse.Name = "numericUniverse";
            this.numericUniverse.Size = new System.Drawing.Size(57, 20);
            this.numericUniverse.TabIndex = 3;
            // 
            // numericAddress
            // 
            this.numericAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericAddress.Location = new System.Drawing.Point(137, 63);
            this.numericAddress.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numericAddress.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericAddress.Name = "numericAddress";
            this.numericAddress.Size = new System.Drawing.Size(57, 20);
            this.numericAddress.TabIndex = 5;
            this.numericAddress.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblEncoding
            // 
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Enabled = false;
            this.lblEncoding.Location = new System.Drawing.Point(134, 96);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(52, 13);
            this.lblEncoding.TabIndex = 20;
            this.lblEncoding.Text = "Encoding";
            // 
            // comboFileEncoding
            // 
            this.comboFileEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFileEncoding.Enabled = false;
            this.comboFileEncoding.FormattingEnabled = true;
            this.comboFileEncoding.Location = new System.Drawing.Point(133, 112);
            this.comboFileEncoding.Name = "comboFileEncoding";
            this.comboFileEncoding.Size = new System.Drawing.Size(69, 21);
            this.comboFileEncoding.TabIndex = 21;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(142, 182);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(52, 23);
            this.buttonBrowse.TabIndex = 25;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // lblFolderPath
            // 
            this.lblFolderPath.AutoSize = true;
            this.lblFolderPath.Location = new System.Drawing.Point(3, 168);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(57, 13);
            this.lblFolderPath.TabIndex = 23;
            this.lblFolderPath.Text = "Folderpath";
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderPath.Location = new System.Drawing.Point(6, 184);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(133, 20);
            this.txtFolderPath.TabIndex = 24;
            // 
            // txtFileScanRegex
            // 
            this.txtFileScanRegex.Enabled = false;
            this.txtFileScanRegex.Location = new System.Drawing.Point(6, 139);
            this.txtFileScanRegex.Name = "txtFileScanRegex";
            this.txtFileScanRegex.Size = new System.Drawing.Size(131, 20);
            this.txtFileScanRegex.TabIndex = 22;
            // 
            // lblFileScanningMethod
            // 
            this.lblFileScanningMethod.AutoSize = true;
            this.lblFileScanningMethod.Location = new System.Drawing.Point(3, 96);
            this.lblFileScanningMethod.Name = "lblFileScanningMethod";
            this.lblFileScanningMethod.Size = new System.Drawing.Size(107, 13);
            this.lblFileScanningMethod.TabIndex = 18;
            this.lblFileScanningMethod.Text = "File scanning method";
            // 
            // comboFileScanMethod
            // 
            this.comboFileScanMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFileScanMethod.FormattingEnabled = true;
            this.comboFileScanMethod.Location = new System.Drawing.Point(6, 112);
            this.comboFileScanMethod.Name = "comboFileScanMethod";
            this.comboFileScanMethod.Size = new System.Drawing.Size(121, 21);
            this.comboFileScanMethod.TabIndex = 19;
            this.comboFileScanMethod.SelectedIndexChanged += new System.EventHandler(this.comboFileScanMethod_SelectedIndexChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(121, 225);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 101;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(6, 225);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 100;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(206, 260);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.lblEncoding);
            this.Controls.Add(this.comboFileEncoding);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.txtFileScanRegex);
            this.Controls.Add(this.lblFileScanningMethod);
            this.Controls.Add(this.comboFileScanMethod);
            this.Controls.Add(this.numericAddress);
            this.Controls.Add(this.numericUniverse);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblUniverse);
            this.Controls.Add(this.lblPhysical);
            this.Controls.Add(this.numericPhysical);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Config Form";
            ((System.ComponentModel.ISupportInitialize)(this.numericPhysical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.NumericUpDown numericPhysical;
        protected System.Windows.Forms.Label lblPhysical;
        protected System.Windows.Forms.Label lblUniverse;
        protected System.Windows.Forms.Label lblAddress;
        protected System.Windows.Forms.NumericUpDown numericUniverse;
        protected System.Windows.Forms.NumericUpDown numericAddress;
        protected System.Windows.Forms.Button buttonBrowse;
        protected System.Windows.Forms.Label lblFolderPath;
        protected System.Windows.Forms.TextBox txtFolderPath;
        protected System.Windows.Forms.TextBox txtFileScanRegex;
        protected System.Windows.Forms.Label lblFileScanningMethod;
        protected System.Windows.Forms.ComboBox comboFileScanMethod;
        protected System.Windows.Forms.Button buttonCancel;
        protected System.Windows.Forms.Button buttonOK;
        protected System.Windows.Forms.ComboBox comboFileEncoding;
        protected System.Windows.Forms.Label lblEncoding;
    }
}
