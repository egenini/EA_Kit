using EA;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.html
{
    public class OptionGroup
    {
        public string Label { set; get; }
        public List<Option> Options;

        public OptionGroup( string label )
        {
            this.Label   = label;
            this.Options = new List<Option>();
        }

        public OptionGroup()
        {
        }
    }
}
