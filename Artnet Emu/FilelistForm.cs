using ArtnetEmu.Exceptions;
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
    public partial class FilelistForm : Form
    {
        List<ListViewItem> Items = new List<ListViewItem>();
        public FilelistForm()
        {
            InitializeComponent();
            listFiles_Resize(null, null);
        }

        public void ClearList()
        {
            listFiles.Items.Clear();
        }

        public void AddFileIndexItem(FileIndexItem file)
        {
            var item = listFiles.Items.Add(file.Group.ToString("D3")+"-" + file.FileIndex.ToString("D3"));
            item.SubItems.Add(file.Filename);
            Items.Add(item);
        }

        public void SortFiles()
        {
            listFiles.Sort();
        }

        private void listFiles_Resize(object sender, EventArgs e)
        {
            listFiles.Columns[1].Width = listFiles.Width - listFiles.Columns[0].Width - 10;
        }

        private void FilelistForm_Shown(object sender, EventArgs e)
        {
            UpdateLabel();
            txtSearch.Focus();
        }

        private void UpdateLabel()
        {
            int count = Items.Count;
            int found = listFiles.Items.Count;
            lblSearch.Text = "Search for text ("+found+"/"+count+")";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string[] query = txtSearch.Text.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            listFiles.BeginUpdate();
            listFiles.Items.Clear();
            bool addAll = query.Count() == 0;
            foreach (ListViewItem item in Items)
            {
                if (addAll || InText(query, item.SubItems[1].Text))
                {
                    listFiles.Items.Add(item);
                }
            }
            SortFiles();
            listFiles.EndUpdate();
            UpdateLabel();
        }
        
        private bool InText(string[] query, string text)
        {
            text = text.ToLower();
            return query.All(x => text.IndexOf(x) != -1);
        }
    }
}
