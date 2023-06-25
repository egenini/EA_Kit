using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFul.resumen.modelo
{
    public class Uri
    {
        public List<Recurso> recursos = new List<Recurso>();
        public Element element;
        public DiagramObjectExt diagramObjectExt;

        public Uri( Element element, DiagramObject diagramObject )
        {
            this.element = element;
            this.diagramObjectExt = new DiagramObjectExt(diagramObject);
        }
    }
}
