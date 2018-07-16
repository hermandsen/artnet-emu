using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Model
{
    public class ComboBoxOption<X> where X : IConvertible
    {
        public X Value;
        public string Name;
        public ComboBoxOption()
        {
            Name = "";
            Value = default(X);
        }
        public ComboBoxOption(string name, X value)
        {
            Name = name;
            Value = value;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
