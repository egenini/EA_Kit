using LicenceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    public class AddinInfo
    {
        public string name;
        public string ribbonCategory;
        public LicenceInformation licence;
        public List<string> menuItems = new List<string>();
        public string version;

        public AddinInfo(string name, string ribbonCategory, LicenceInformation licence, string version)
        {
            this.name = name;
            this.ribbonCategory = ribbonCategory;
            this.licence = licence;
            this.version = version;
        }

        public string menuHeader()
        {
            return "-&" + name;
        }

    }
}
