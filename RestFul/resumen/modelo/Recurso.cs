using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace RestFul.resumen.modelo
{
    public class Recurso
    {
        public const string style = "Classbold=1;BFol=8355711;font=Calibri;fontsz=120;black=0;italic=0;ul=0;charset=0;pitch=34;LCol=16777215;BCol=16777215;DUID=12CB57F0;";
        public const int height = 24;
        public const int width = 128;
        public const int offsetLeft = 20;

        EAUtils.EAUtils eaUtils;
        public Recurso parent = null;
        public Element fromElement = null;
        public Element element = null;
        public DiagramObjectExt doe = null;
        public string nombre = null;
        public List<Recurso> recursos = new List<Recurso>();
        public List<Metodo> metodos = new List<Metodo>();
        private Uri uri;
        private DiagramObject diagramObjectParent;
        private DiagramObject diagramObjectLast;
        private Element parentElement;
        private ResumenUtils resumenUtils;

        public Recurso(string nombre, ResumenUtils resumenUtils)
        {
            this.nombre = nombre;
            this.eaUtils = resumenUtils.eaUtils;
            this.resumenUtils = resumenUtils;
        }

        public Recurso(Element fromElement, ResumenUtils resumenUtils)
        {
            this.fromElement = fromElement;
            this.eaUtils = resumenUtils.eaUtils;
            this.resumenUtils = resumenUtils;
        }

        public Recurso(Element fromElement, Diagram diagram, EAUtils.EAUtils eaUtils)
        {
            this.fromElement = fromElement;
            this.eaUtils = eaUtils;
            this.resumenUtils = new ResumenUtils( this.eaUtils );
            this.resumenUtils.diagram = diagram;
        }

        public Recurso buscarPadre ()
        {
            if ( this.parentElement == null )
            {
                // si no tiene una relacion con un parent se asume que este es la uri
                List<ElementConnectorInfo> elementsAndConnectors = this.eaUtils.connectorUtils.get(this.element, null, "APIRestConnector", null, "Resource", true, null);

                if( elementsAndConnectors.Count != 0)
                {
                    this.parentElement = elementsAndConnectors[0].element;
                }
                else
                {
                    this.uri = resumenUtils.getUri();

                    this.parentElement = this.uri.element;
                }
            }
            return this;
        }

        public Recurso crearAbm( Package package, DiagramObject diagramObjectParent )
        {
            this.diagramObjectParent = diagramObjectParent;

            // el recurso principal
            crear( package ).buscarPadre().dibujar().relacionar();

            // los métodos get y post
            this.metodos.Add( new Metodo( this.resumenUtils ).crearGet(  this ).dibujar().relacionar() );

            this.metodos.Add( new Metodo( this.resumenUtils ).crearPost( this ).dibujar().relacionar() );

            // recurso id
            Recurso recursoId = new Recurso( "id", this.resumenUtils ).crear( this.element ).dibujar().relacionar();

            recursoId.metodos.Add( new Metodo( this.resumenUtils ).crearGet(    recursoId ).dibujar().relacionar() );
            recursoId.metodos.Add( new Metodo( this.resumenUtils ).crearPut(    recursoId ).dibujar().relacionar() );
            recursoId.metodos.Add( new Metodo( this.resumenUtils ).crearDelete( recursoId ).dibujar().relacionar() );

            return this;
        }

        private Recurso crear( Collection elements)
        {
            establecerNombre();

            element = elements.AddNew("/" + nombre, "Class");

            element.StereotypeEx = "Service::Resource";

            element.Update();

            return this;
        }

        public Recurso crear(Element parent)
        {
            this.parentElement = parent;

            crear(parent.Elements);

            return this;
        }

        public Recurso crear( Package package )
        {
            crear( package.Elements) ;

            if( fromElement != null )
            {
                this.eaUtils.connectorUtils.addRealization(element, fromElement);
            }
            return this;
        }

        private void establecerNombre()
        {
            if (nombre == null)
            {
                string pluralName = fromElement.Name + "s";

                if (fromElement.Stereotype == "DomainClass" || fromElement.Stereotype == "LogicClass")
                {
                    pluralName = this.eaUtils.taggedValuesUtils.get(fromElement, "plural_name", "").asString();
                }
                nombre = EAUtils.StringUtils.toCamel(pluralName).Replace("_", "-");
            }
        }

        public Recurso relacionar()
        {
            eaUtils.connectorUtils.addConnectorAssociation(this.parentElement, this.element, null, "APIRestConnector");

            this.resumenUtils.diagramLinkStyle();

            return this;
        }

        public Recurso dibujar()
        {
            this.resumenUtils.resourceStyle(doe);

            doe.position.calculateVerticalTree( diagramObjectLast, diagramObjectParent );

            doe.diagramObject = this.eaUtils.diagramUtils.addDiagramElement( this.resumenUtils.diagram, this.element, doe);

            return this;
        }
    }
}
