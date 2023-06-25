using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAUtils;
using UserInterface.frw;
using Newtonsoft.Json.Serialization;
using EAUtils.framework2;
using Newtonsoft.Json;

namespace UserInterface.html
{
    public interface HtmlTag{ }

    public class HtmlCommon : Jsonable, HtmlTag
    {
        [JsonIgnore]
        protected EAUtils.EAUtils eaUtils;
        [JsonIgnore]
        protected Element element;
        [JsonIgnore]
        protected Framework frameworkInstance;
        [JsonIgnore]
        protected Absolute parentPosition;

        public Dictionary<string, string> events = null;
        public List<HtmlTag> tags = new List<HtmlTag>();
        public string Metatype { set; get; }
        public string Name { set; get;}
        public string Id { set; get; }
        public string Style { set; get; }
        public string CssClass { set; get; }
        public string Title { set; get; }
        public string Hidden { set; get; }
        public string Data { set; get; }
        public string Accesskey { set; get; }
        public string Draggable { set; get; }
        public string Dropzone { set; get; }
        public string Contenteditable { set; get; }
        public string Contextmenu { set; get; }
        public string Dir { set; get; }
        public string Lang { set; get; }
        public string Spellcheck { set; get; }
        public string Tabindex { set; get; }
        public string Translate { set; get; }

        public EAUtils.model.Attribute Attribute { set; get; } = null;

        public Absolute Absolute { set; get; }

        public HtmlCommon(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;

            Metatype = element.Stereotype;

            this.setProperties();
        }

        public string stringfity()
        {
            throw new NotImplementedException();
        }

        public string walk()
        {
            HtmlCommon child;
            bool buildThis = true;

            Absolute = new Absolute();

            Absolute.build( this.element, parentPosition, this.eaUtils );

            if( esRelevanteParaPosicion( this.element ))
            {
                parentPosition = Absolute;
            }

            analizarCargaRecursos();

            this.analizarAtributo();

            foreach (Element elementChild in this.element.Elements)
            {
                child     = null;
                buildThis = true;

                if( elementChild.Stereotype == "Div" && elementChild.Connectors.Count != 0 )
                {
                    List < ConnectorUtils.ElementConnectorInfo > includes = this.eaUtils.connectorUtils.get(elementChild, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, "Include", "Class", null, false, null);

                    buildThis = includes.Count == 0;

                    foreach(ConnectorUtils.ElementConnectorInfo include in includes)
                    {
                        child = buildTag(include.element);

                        if (child != null)
                        {
                            tags.Add(child);
                            child.walk();
                        }
                    }
                }

                if (buildThis)
                {
                    child = buildTag(elementChild);

                    if (child != null)
                    {
                        tags.Add(child);
                        child.walk();
                    }
                }
            }
            return "";
        }

        private HtmlCommon buildTag( Element elementChild)
        {
            HtmlCommon child = null;

            switch (elementChild.Stereotype)
            {
                
                case "Button":

                    child = new Button(elementChild, frameworkInstance, eaUtils);
                    break;
                
                case "Checkbox":

                    child = new Checkbox(elementChild, frameworkInstance, eaUtils);
                    break;
                
                case "Select":

                    child = new Select(elementChild, frameworkInstance, eaUtils);
                    break;

                case "FileUpload":

                    child = new FileUpload(elementChild, frameworkInstance, eaUtils);
                    break;
                
                case "Form":

                    child = new Form(elementChild, frameworkInstance, eaUtils);
                    break;

                case "Input":

                    child = new Input(elementChild, frameworkInstance, eaUtils);
                    break;
                
                case "Radio":

                    child = new Radio(elementChild, frameworkInstance, eaUtils);
                    break;
                
                case "TextArea":

                    child = new TextArea(elementChild, frameworkInstance, eaUtils);
                    break;

                case "Table":

                    child = new Table(elementChild, frameworkInstance, eaUtils);
                    break;
                /*
                case "Div":

                    child = new Div(elementChild, frameworkInstance, eaUtils);
                    break;
                */
                default:
                    child = new HtmlCommon(elementChild, frameworkInstance, eaUtils);
                    break;
            }

            return child;
        }

        private bool esRelevanteParaPosicion(Element element)
        {
            return element.Stereotype == "Screen";
        }

        protected void setProperties()
        {
            this.Name            = element.Name;
            this.Id              = this.eaUtils.taggedValuesUtils.get(element, "id", this.Name).asString();
            this.Style           = this.eaUtils.taggedValuesUtils.get(element, "style", "").asString();
            this.CssClass        = this.eaUtils.taggedValuesUtils.get(element, "cssClass", "").asString();
            this.Title           = this.eaUtils.taggedValuesUtils.get(element, "title", "").asString();
            this.Hidden          = this.eaUtils.taggedValuesUtils.get(element, "hidden", "").asString();
            this.Data            = this.eaUtils.taggedValuesUtils.get(element, "data", "").asString();
            this.Accesskey       = this.eaUtils.taggedValuesUtils.get(element, "accesskey", "").asString();
            this.Draggable       = this.eaUtils.taggedValuesUtils.get(element, "draggable", "").asString();
            this.Dropzone        = this.eaUtils.taggedValuesUtils.get(element, "dropzone", "").asString();
            this.Contenteditable = this.eaUtils.taggedValuesUtils.get(element, "contexteditable", "").asString();
            this.Contextmenu     = this.eaUtils.taggedValuesUtils.get(element, "contextmenu", "").asString();
            this.Dir             = this.eaUtils.taggedValuesUtils.get(element, "dir", "").asString();
            this.Lang            = this.eaUtils.taggedValuesUtils.get(element, "lang", "").asString();
            this.Spellcheck      = this.eaUtils.taggedValuesUtils.get(element, "spellcheck", "").asString();
            this.Tabindex        = this.eaUtils.taggedValuesUtils.get(element, "tabindex", "").asString();
            this.Translate       = this.eaUtils.taggedValuesUtils.get(element, "translate", "").asString();

            events = this.eaUtils.taggedValuesUtils.getByPrefix(element, "ev.", new string[] { "ev.", "" } );
        }

        private void analizarCargaRecursos()
        {

            if (element.Stereotype == "Screen" && this.element.Connectors.Count != 0 )
            {
                List<ConnectorUtils.ElementConnectorInfo> includes = this.eaUtils.connectorUtils.get(this.element, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, "IncludeResource", "Class", null, false, null);

            }
        }

        private void analizarAtributo()
        {
            string guid = "";

            if( this.element.Attributes.Count != 0)
            {
                EA.Attribute eaAttribute = this.element.Attributes.GetAt(0);

                guid = this.eaUtils.taggedValuesUtils.get(eaAttribute, "source.guid", "").asString();

                this.eaUtils.taggedValuesUtils.set(this.element, "guid", guid);

                this.element.Attributes.DeleteAt(0, true);

            }
            else
            {
                guid = this.eaUtils.taggedValuesUtils.get(this.element, "source.guid", "").asString();
            }

            if (guid != "")
            {
                EA.Attribute referenced = this.eaUtils.repository.GetAttributeByGuid(guid);

                this.Attribute = new EAUtils.model.Attribute(referenced, this.eaUtils);
            }
        }
    }

    public class Absolute
    {
        public Position Position { set; get; }
        public Size Size { set; get; }
        public Label Label {set;get;}

        public void build( Element element, Absolute parent, EAUtils.EAUtils eaUtils)
        {
            DiagramObject diagramObject = lookingForDiagramObject(element, eaUtils);

            Position = new Position {
                left   = diagramObject.left,
                right  = diagramObject.right,
                top    = -1 * diagramObject.top,
                bottom = -1 * diagramObject.bottom
            };

            if( element.Stereotype == "Radio" || element.Stereotype == "Checkbox")
            {
                Size = new Size { width = Position.bottom - Position.top, height = Position.bottom - Position.top };
            }
            else
            {
                Size = new Size { width = Position.right - Position.left, height = Position.bottom - Position.top };
            }

            try {
                // buscamos la orientación de la etiqueta si la tuviera.
                string style = diagramObject.Style;

                int posIni = style.IndexOf("LBL=");

                if (posIni != -1)
                {
                    posIni += 4;

                    string lbl = style.Substring(posIni);

                    string[] lblSplitted = lbl.Split(':');

                    int ox = -1;
                    int oy = -1;
                    int cx = -1;
                    string[] oxoySplitted;

                    foreach (string oxoy in lblSplitted)
                    {
                        oxoySplitted = oxoy.Split('=');

                        if (oxoySplitted[0] == "OX")
                        {
                            ox = int.Parse(oxoySplitted[1]);
                        }
                        else if (oxoySplitted[0] == "OY")
                        {
                            oy = int.Parse(oxoySplitted[1]);
                        }
                        else if (oxoySplitted[0] == "CX")
                        {
                            cx = int.Parse(oxoySplitted[1]);
                        }

                        if (ox != -1 && oy != -1 && cx != -1)
                        {
                            break;
                        }
                    }

                    if (ox != -1 && oy != -1 && cx != -1)
                    {
                        if (oy == 0 && ox == 0 )
                        {
                            Label = new Label { orientation = "inside", length = cx};
                        }
                        else if (ox < 0)
                        {
                            Label = new Label { orientation = "left", length = cx };
                        }
                        else if (oy < 0)
                        {
                            Label = new Label { orientation = "top", length = cx };
                        }
                        else if (ox > 0)
                        {
                            Label = new Label { orientation = "rigth", length = cx };
                        }
                    }
                }
            }
            catch ( Exception) { }

            if ( parent != null)
            {
                Position.left   = Position.left   - parent.Position.left ;
                Position.right  = Position.right  - parent.Position.left;
                Position.top    = Position.top    - parent.Position.top;
                Position.bottom = Position.bottom - parent.Position.top;
            }
        }

        private DiagramObject lookingForDiagramObject( Element element, EAUtils.EAUtils eaUtils)
        {
            Diagram diagram = eaUtils.repository.GetCurrentDiagram();

            DiagramObject diagramObject = eaUtils.diagramUtils.findInDiagramObjects(element, diagram);

            if (diagramObject == null)
            {
                Package package = eaUtils.repository.GetPackageByID(element.PackageID);

                diagramObject = eaUtils.diagramUtils.findInDiagramObjects(element, package.Diagrams.GetAt(0));
            }

            return diagramObject;
        }
    }

    public class Position
    {
        public int left { set; get; }
        public int right { set; get; }
        public int top { set; get; }
        public int bottom { set; get; }

    }
    public class Size
    {
        public int width { set; get; }
        public int height { set; get; }
        public int offset { set; get; }
    }

    public class Label
    {
        public string orientation { set; get; }
        public int length { set; get; }
    }
}
