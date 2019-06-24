using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtnetEmu.Model;
using ArtnetEmu.Model.Configs;
using System.Reflection;
using System.Globalization;

namespace ArtnetEmu
{
    public interface IConfigurationForm
    {
        void SetConfiguration<Config>(Config config) where Config : PlayerConfiguration, new();
        Config GetConfiguration<Config>() where Config : PlayerConfiguration, new();
        DialogResult ShowDialog();
    }
    public partial class ConfigForm : Form, IConfigurationForm
    {
        public ConfigForm()
        {
            InitializeComponent();
            foreach (FileScanMethod method in Enum.GetValues(typeof(FileScanMethod)))
            {
                comboFileScanMethod.Items.Add(new ComboBoxOption<FileScanMethod>(method.ToString(), method));
            }
            comboFileScanMethod.SelectedIndex = 0;
            foreach (FileEncoding encoding in Enum.GetValues(typeof(FileEncoding)))
            {
                comboFileEncoding.Items.Add(new ComboBoxOption<FileEncoding>(encoding.ToString(), encoding));
            }
            comboFileEncoding.SelectedIndex = 0;
        }

        public void SetConfiguration<Config>(Config config) where Config : PlayerConfiguration, new()
        {
            Type configType = config.GetType();
            PropertyInfo[] properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                ConfigAttribute attr = property.GetCustomAttributes<ConfigAttribute>().First();
                if (attr != null)
                {
                    MethodInfo getter = property.GetGetMethod();
                    Control control = Controls[attr.Name];
                    object result = getter.Invoke(config, new object[] { });
                    switch (attr.Type)
                    {
                        case ControlType.CheckBox:
                            ((CheckBox)control).Checked = (bool)result;
                            break;
                        case ControlType.ComboBox:
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            var index = result == null ? -1 : (byte)Convert.ChangeType(result, t);
                            ((ComboBox)control).SelectedIndex = index;
                            break;
                        case ControlType.TextBox:
                            ((TextBox)control).Text = (string)result;
                            break;
                        case ControlType.NumericUpDown:
                            ((NumericUpDown)control).Value = Convert.ToDecimal(result);
                            break;
                    }
                }
            }
        }

        public Config GetConfiguration<Config>() where Config : PlayerConfiguration, new()
        {
            Type configType = typeof(Config);
            Config result = (Config)Activator.CreateInstance(configType);
            PropertyInfo[] properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties) {
                ConfigAttribute attr = property.GetCustomAttributes<ConfigAttribute>().First();
                if (attr != null)
                {
                    MethodInfo setter = property.GetSetMethod();
                    Control control = Controls[attr.Name];
                    switch (attr.Type)
                    {
                        case ControlType.CheckBox:
                            setter.Invoke(result, new object[] { ((CheckBox)control).Checked });
                            break;
                        case ControlType.ComboBox:
                            Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            var a = Enum.GetValues(underlyingType);
                            byte? value = null;
                            if (((ComboBox)control).SelectedIndex != -1)
                            {
                                value = (byte?)((ComboBox)control).SelectedIndex;
                            }
                            setter.Invoke(result, new object[] { Enum.ToObject(underlyingType, value) });
                            break;
                        case ControlType.TextBox:
                            setter.Invoke(result, new object[] { ((TextBox)control).Text });
                            break;
                        case ControlType.NumericUpDown:
                            decimal d = ((NumericUpDown)control).Value;
                            object integer = 0;
                            switch (attr.IntType)
                            {
                                case IntType.S1:
                                    integer = Convert.ToSByte(d);
                                    break;
                                case IntType.S2:
                                    integer = Convert.ToInt16(d);
                                    break;
                                case IntType.S4:
                                    integer = Convert.ToInt32(d);
                                    break;
                                case IntType.U1:
                                    integer = Convert.ToByte(d);
                                    break;
                                case IntType.U2:
                                    integer = Convert.ToUInt16(d);
                                    break;
                                case IntType.U4:
                                    integer = Convert.ToUInt32(d);
                                    break;
                            }
                            setter.Invoke(result, new object[] { integer });
                            break;
                    }
                }
            }
            return result;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var folder = new FolderBrowserDialog())
            {
                folder.SelectedPath = txtFolderPath.Text;
                DialogResult result = folder.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
                {
                    txtFolderPath.Text = folder.SelectedPath;
                }
            }
        }

        private void comboFileScanMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((ComboBoxOption<FileScanMethod>)comboFileScanMethod.SelectedItem).Value)
            {
                case FileScanMethod.Filelist:
                    txtFileScanRegex.Enabled = false;
                    comboFileEncoding.Enabled = true;
                    lblEncoding.Enabled = true;
                    break;
                case FileScanMethod.Filestructure:
                    txtFileScanRegex.Enabled = false;
                    comboFileEncoding.Enabled = false;
                    lblEncoding.Enabled = false;
                    break;
                case FileScanMethod.Regex:
                    txtFileScanRegex.Enabled = true;
                    comboFileEncoding.Enabled = false;
                    lblEncoding.Enabled = false;
                    break;
            }
        }
    }
}
