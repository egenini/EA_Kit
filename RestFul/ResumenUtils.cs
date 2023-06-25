using EA;
using EAUtils;
using RestFul.resumen.modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFul
{
    public class ResumenUtils
    {
        const string linkStyle = "Mode=3;EOID=86933FE9;SOID=92294773;Color=-1;LWidth=0;TREE=LH;";

        public EAUtils.EAUtils eaUtils;
        public Diagram diagram;

        public ResumenUtils(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public DiagramObject ultimoDiagramObject()
        {
            DiagramObject ultimoDiagramObject = null;

            foreach( DiagramObject diagramObject in diagram.DiagramObjects)
            {
                if(ultimoDiagramObject == null || diagramObject.top > ultimoDiagramObject.top)
                {
                    ultimoDiagramObject = diagramObject;
                }
            }

            return ultimoDiagramObject;
        }

        public resumen.modelo.Uri getUri( )
        {
            // busca la uri para ponerla como parent de este recurso.
            Element uriElement = null;
            DiagramObject uriDiagramObject = null;
            resumen.modelo.Uri uri = null;

            foreach (DiagramObject diagramObject in diagram.DiagramObjects)
            {
                uriElement = this.eaUtils.repository.GetElementByID(diagramObject.ElementID);

                if (uriElement.Stereotype == "URI")
                {
                    uriDiagramObject = diagramObject;
                    break;
                }
            }

            if (uriElement != null)
            {
                uri = new resumen.modelo.Uri(uriElement, uriDiagramObject);
            }
            return uri;
        }

        public DiagramObjectExt resourceStyle()
        {
            DiagramObjectExt doe = new DiagramObjectExt();
            resourceStyle(doe);
            return doe;
        }

        public void resourceStyle( DiagramObjectExt doe )
        {
            doe.style = Recurso.style;
            doe.position.height = Recurso.height;
            doe.position.width = Recurso.width;
            doe.position.offset.left = Recurso.offsetLeft;

            doe.position.calculate();
        }

        public DiagramObjectExt methodStyle()
        {
            DiagramObjectExt doe = new DiagramObjectExt();
            methodStyle(doe);
            return doe;
        }

        public void methodStyle(DiagramObjectExt doe )
        {
            doe.position.height = Metodo.height;
            doe.position.width = Metodo.width;

            doe.position.calculate();

        }
        public void diagramLinkStyle( )
        {
            DiagramLink diagramLink = lastDiagramLink();

            diagramLink.LineStyle = EA.LinkLineStyle.LineStyleLateralHorizontal;
            diagramLink.Style = linkStyle;

            diagramLink.Update();
        }

        public DiagramLink lastDiagramLink()
        {
            short last = diagram.DiagramLinks.Count;

            return diagram.DiagramLinks.GetAt(last--);
        }
    }
}
