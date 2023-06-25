using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.html
{
    public class Option
    {
        public string Label { set; get; }
        public string Value { set; get; }

        public Option( string value)
        {
            this.Label = value;
            this.Value = value;
        }
        public Option(string label, string value)
        {
            this.Label = label;
            this.Value = value;
        }
    }
}
