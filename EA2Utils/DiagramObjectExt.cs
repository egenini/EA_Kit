using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    public class DiagramObjectExt
    {
        public DiagramObject diagramObject;
        public DiagramObjectPosition position ;
        public string style = null;

        public DiagramObjectExt()
        {
            this.position = new DiagramObjectPosition();
        }

        public DiagramObjectExt(DiagramObject diagramObject)
        {
            this.diagramObject = diagramObject;
            this.position = new DiagramObjectPosition(diagramObject);
        }
    }
}
