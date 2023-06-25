using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.entity
{
    public class Namespace
    {
        public string artifactName;
        public Dictionary<string, string> attributes = new Dictionary<string, string>();
        public bool isMain = false;

        public Namespace( string artifactName)
        {
            this.artifactName = artifactName;
        }
    }
}
