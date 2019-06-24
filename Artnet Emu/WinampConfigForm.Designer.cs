namespace ArtnetEmu
{
    partial class WinampConfigForm
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
            this.checkBoxAlwaysAdd = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(127, 209);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 209);
            // 
            // checkBoxAlwaysAdd
            // 
            this.checkBoxAlwaysAdd.AutoSize = true;
            this.checkBoxAlwaysAdd.Checked = true;
            this.checkBoxAlwaysAdd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAlwaysAdd.Location = new System.Drawing.Point(6, 181);
            this.checkBoxAlwaysAdd.Name = "checkBoxAlwaysAdd";
            this.checkBoxAlwaysAdd.Size = new System.Drawing.Size(101, 17);
            this.checkBoxAlwaysAdd.TabIndex = 12;
            this.checkBoxAlwaysAdd.Text = "Always add files";
            this.checkBoxAlwaysAdd.UseVisualStyleBackColor = true;
            // 
            // WinampConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 244);
            this.Controls.Add(this.checkBoxAlwaysAdd);
            this.Name = "WinampConfigForm";
            this.Text = "Winamp Config";
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
            this.Controls.SetChildIndex(this.checkBoxAlwaysAdd, 0);
            ((System.ComponentModel.ISupportInitialize)(this.numericUniverse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBoxAlwaysAdd;
    }
}