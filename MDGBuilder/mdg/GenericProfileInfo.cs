using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{
    public class GenericProfileInfo
    {
        public string directory = null;
        public List<string> files = new List<string>();

        public void addFromGenfile( string genfile)
        {
            if(directory == null)
            {
                directory = Path.GetDirectoryName(genfile);
            }
            files.Add( Path.GetFileName(genfile) );
        }
    }
}
