﻿namespace ArtnetEmu
{
    partial class VLCRemoteConfigForm
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
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUri = new System.Windows.Forms.TextBox();
            this.lblUri = new System.Windows.Forms.Label();
            this.checkBoxAlwaysAdd = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(127, 298);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 298);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(3, 249);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(202, 13);
            this.lblPassword.TabIndex = 13;
            this.lblPassword.Text = "HTTP password for Lua (VLC http-server)";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(6, 265);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 14;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUri
            // 
            this.txtUri.Location = new System.Drawing.Point(6, 221);
            this.txtUri.Name = "txtUri";
            this.txtUri.Size = new System.Drawing.Size(131, 20);
            this.txtUri.TabIndex = 7;
            this.txtUri.Text = "http://127.0.0.1/";
            // 
            // lblUri
            // 
            this.lblUri.AutoSize = true;
            this.lblUri.Location = new System.Drawing.Point(3, 205);
            this.lblUri.Name = "lblUri";
            this.lblUri.Size = new System.Drawing.Size(43, 13);
            this.lblUri.TabIndex = 6;
            this.lblUri.Text = "VLC Url";
            // 
            // checkBoxAlwaysAdd
            // 
            this.checkBoxAlwaysAdd.AutoSize = true;
            this.checkBoxAlwaysAdd.Checked = true;
            this.checkBoxAlwaysAdd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAlwaysAdd.Location = new System.Drawing.Point(6, 181);
            this.checkBoxAlwaysAdd.Name = "checkBoxAlwaysAdd";
            this.checkBoxAlwaysAdd.Size = new System.Drawing.Size(101, 17);
            this.checkBoxAlwaysAdd.TabIndex = 29;
            this.checkBoxAlwaysAdd.Text = "Always add files";
            this.checkBoxAlwaysAdd.UseVisualStyleBackColor = true;
            // 
            // VLCRemoteConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 333);
            this.Controls.Add(this.checkBoxAlwaysAdd);
            this.Controls.Add(this.lblUri);
            this.Controls.Add(this.txtUri);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Name = "VLCRemoteConfigForm";
            this.Text = "VLC Config";
            this.Controls.SetChildIndex(this.comboFileScanMethod, 0);
            this.Controls.SetChildIndex(this.lblFileScanningMethod, 0);
            this.Controls.SetChildIndex(this.txtFileScanRegex, 0);
            this.Controls.SetChildIndex(this.txtFolderPath, 0);
            this.Controls.SetChildIndex(this.lblFolderPath, 0);
            this.Controls.SetChildIndex(this.buttonBrowse, 0);
            this.Controls.SetChildIndex(this.comboFileEncoding, 0);
            this.Controls.SetChildIndex(this.lblEncoding, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.lblUniverse, 0);
            this.Controls.SetChildIndex(this.lblAddress, 0);
            this.Controls.SetChildIndex(this.numericUniverse, 0);
            this.Controls.SetChildIndex(this.numericAddress, 0);
            this.Controls.SetChildIndex(this.lblPassword, 0);
            this.Controls.SetChildIndex(this.txtPassword, 0);
            this.Controls.SetChildIndex(this.txtUri, 0);
            this.Controls.SetChildIndex(this.lblUri, 0);
            this.Controls.SetChildIndex(this.checkBoxAlwaysAdd, 0);
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label lblPassword;
        protected System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUri;
        private System.Windows.Forms.Label lblUri;
        protected System.Windows.Forms.CheckBox checkBoxAlwaysAdd;
    }
}