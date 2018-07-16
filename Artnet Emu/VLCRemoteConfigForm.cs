using ArtnetEmu.Model;
using ArtnetEmu.Model.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtnetEmu
{
    public partial class VLCRemoteConfigForm : ConfigForm
    {
        public VLCRemoteConfigForm()
        {
            InitializeComponent();
            var remove = FileScanMethod.Filelist;
            object found = null;
            foreach (var item in comboFileScanMethod.Items)
            {
                if (((ComboBoxOption<FileScanMethod>)item).Value == remove)
                {
                    found = item;
                }
            }
            if (found != null)
            {
                comboFileScanMethod.Items.Remove(found);
            }
            txtFolderPath.Text = "file:///C:/";
            comboFileEncoding.Visible = false;
            lblEncoding.Visible = false;
            buttonBrowse.Visible = false;
            txtUri.Text = System.Configuration.ConfigurationManager.AppSettings["VLCLocalHttpUrl"];
        }

        private void comboFileScanMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
