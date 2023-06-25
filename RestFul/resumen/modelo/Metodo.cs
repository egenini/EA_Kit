using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFul.resumen.modelo
{
    public class Metodo
    {
        const string GET_LABEL    = "GET";
        const string POST_LABEL   = "POST";
        const string PUT_LABEL    = "PUT";
        const string DELETE_LABEL = "DELETE";

        public const int height = 28;
        public const int width = 45;

        ResumenUtils resumenUtils;

        Element element;
        Recurso recurso;

        public string nombre;

        public DiagramObjectExt diagramObjectExt = new DiagramObjectExt();

        public Metodo( ResumenUtils resumenUtils )
        {
            this.resumenUtils = resumenUtils;
        }

        public Metodo crearGet(Recurso recurso)
        {
            return build( recurso, GET_LABEL);
        }
        public Metodo crearPost(Recurso recurso)
        {
            return build(recurso, POST_LABEL);
        }
        public Metodo crearPut(Recurso recurso)
        {
            return build(recurso, PUT_LABEL);
        }
        public Metodo crearDelete(Recurso recurso)
        {
            return build(recurso, DELETE_LABEL);
        }
        public Metodo build( Recurso recurso, string label)
        {
            this.recurso = recurso;
            this.nombre  = label;

            return this;
        }

        private Metodo create()
        {
            element = recurso.element.Elements.AddNew(nombre, "Class");

            element.StereotypeEx = "Services::" + nombre;

            element.Update();

            resumenUtils.methodStyle( this.diagramObjectExt ); 

            return this;
        }

        public Metodo relacionar()
        {
            resumenUtils.eaUtils.connectorUtils.addConnectorAssociation( recurso.element, this.element, null, "APIRestConnector");

            this.resumenUtils.diagramLinkStyle();

            return this;
        }

        public Metodo dibujar()
        {
            this.diagramObjectExt.position.calculate();

            diagramObjectExt.position.calculateVerticalTree( this.resumenUtils.ultimoDiagramObject() , this.recurso.doe.diagramObject);

            diagramObjectExt.diagramObject = resumenUtils.eaUtils.diagramUtils.addDiagramElement(resumenUtils.diagram, this.element, diagramObjectExt);

            return this;
        }

    }
}
