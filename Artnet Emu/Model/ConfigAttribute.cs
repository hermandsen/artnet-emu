using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public enum ControlType
    {
        TextBox,
        CheckBox,
        ComboBox,
        NumericUpDown
    }
    public enum IntType
    {
        U1,
        U2,
        U4,
        S1,
        S2,
        S4
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfigAttribute : Attribute
    {
        private string name;
        private ControlType type;
        private IntType intType;
        public ConfigAttribute(string name, ControlType type, IntType intType = IntType.S4)
        {
            this.name = name;
            this.type = type;
            this.intType = intType;
        }

        public string Name { get { return name; } set { name = value; } }
        public ControlType Type { get { return type; } set { type = value; } }
        public IntType IntType { get { return intType; } set { intType = value; } }
    }
}
