using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;

namespace MDGBuilder.mdg
{
    /// <summary>
    /// Una implementación bastante robusta, para ver todas las posibilidades consultar: 
    /// http://www.sparxsystems.com/enterprise_architect_user_guide/14.0/modeling_tools/quick_linker_definition_format.html
    /// 
    /// 
    /// </summary>
    public class QuickLinkerLine
    {
        
        public const string HEADER_LINE = "//Source Element Type,Source Stereotype Filter,Target Element Type,Target Stereotype Filter,Diagram Filter,New Element Type,New Element Stereotype,New Link Type,New Link Stereotype,New Link Direction,New Link Caption,New Link & Element Caption,Create Link,Create Element,Disallow Self connector,Exclusive to ST Filter + No inherit from Metatype,Menu Group,Complexity Level,Target Must Be Parent,Embed element,Precedes Separator LEAF,Precedes Separator GROUP,Dummy Column";

        public JObject quickLinkerJson;

        /// <summary>
        /// Este dato se obtiene de la dirección de la relación (si la punta de flecha lo apunta).
        /// Se busca el clasificador y si es un estereotipo se busca a su metaclass para poner ese nombre en este lugar.
        /// Si es bidireccional se debe hacer otra instancia de esta clase.
        /// </summary>
        public string SourceElementType
        {
            get
            {
                return (string) quickLinkerJson["Source_Element_Type"]["value"];
            }
            set
            {
                quickLinkerJson["Source_Element_Type"]["value"] = value;
            }
        }
        /// <summary>
        /// Se lo identifica por la dirección (si la punta de flecha lo apunta)
        /// Se busca al clasificador de este elemento y si es un estereotipo se toma su nombre.
        /// </summary>
        public string SourceStereotypeFilter
        {
            get
            {
                return  (string)quickLinkerJson["Source_Stereotype_Filter"]["value"];
            }
            set
            {
                quickLinkerJson["Source_Stereotype_Filter"]["value"] = value;
            }
        }

        /// <summary>
        /// Se busca el otro extremo de la realación y si es una instancia se busca su clasificador y si este es un estereotipo se toma su nombre
        /// </summary>
        public string TargetElementType
        {
            get
            {
                return (string)quickLinkerJson["Target_Element_Type"]["value"];
            }
            set
            {
                quickLinkerJson["Target_Element_Type"]["value"] = value;
            }

        }

        /// <summary>
        /// Se lo identifica por la dirección (el otro extremo de la punta de flecha -sin importar si también es apuntado-)
        /// Se busca al clasificador de este elemento y si es un estereotipo se toma su nombre.
        /// </summary>
        public string TargetStereotypeFilter
        {
            get
            {
                return (string)quickLinkerJson["Target_Stereotype_Filter"]["value"];
            }
            set
            {
                quickLinkerJson["Target_Stereotype_Filter"]["value"] = value;
            }
        }

        /// <summary>
        /// Puede ser una lista de diagramas cada uno finaliza en ;
        /// Si el diagrama no es UML entonces debe tener el nombre del MDG por ej: BPMN2.0::Business Process;
        /// Si se quieren agregar todos los diagrama de una tecnología entonces por ej: BPMN2.0::*;
        /// Si se quiere excluir un diagrama se debe anteponer el signo de exclamación por ej: !Sequence;
        /// Notas del conector.
        /// </summary>
        public string DiagramFilter
        {
            get
            {
                return (string)quickLinkerJson["Diagram_Filter"]["value"];
            }
            set
            {
                quickLinkerJson["Diagram_Filter"]["value"] = value;
            }
        }

        /// <summary>
        /// la cardinalidad es una restricción: 0 = no permite crear el elemento destino. 1 = se permite crear elemento destino.
        /// Buscamos el clasificador y desde este vamos a la metaclass.
        /// </summary>
        public string NewElementType
        {
            get
            {
                return (string)quickLinkerJson["New_Element_Type"]["value"];
            }
            set
            {
                quickLinkerJson["New_Element_Type"]["value"] = value;
            }
        }

        /// <summary>
        /// Buscamos el clasificador y tomamos su nombre.
        /// </summary>
        public string NewElementStereotype
        {
            get
            {
                return (string)quickLinkerJson["New_Element_Stereotype"]["value"];
            }
            set
            {
                quickLinkerJson["New_Element_Stereotype"]["value"] = value;
            }
        }

        /// <summary>
        /// El tipo de conector a crear, tomamos el nombre del conector que tiene la relación. Es un connector UML
        /// </summary>
        public string NewLinkType
        {
            get
            {
                return (string)quickLinkerJson["New_Link_Type"]["value"];
            }
            set
            {
                quickLinkerJson["New_Link_Type"]["value"] = value;
            }
        }

        /// <summary>
        /// Tomamos el estereotipo del conector.
        /// </summary>
        public string NewLinkStereotype
        {
            get
            {
                return (string)quickLinkerJson["New_Link_Stereotype"]["value"];
            }
            set
            {
                quickLinkerJson["New_Link_Stereotype"]["value"] = value;
            }
        }

        /// <summary>
        /// este dato sale del nombre del rol en el extremo que estamos analizando.
        /// directed (always creates an Association from source to target)
        /// from(always creates an Association from target to source)
        /// undirected(always creates an Association with unspecified direction)
        /// bidirectional(always creates a bi-directional Association), or
        /// to(creates either a directed or undirected Association, depending on the value of the 'Association Direction' field)
        /// </summary>
        public string NewLinkDirection
        {
            get
            {
                return (string)quickLinkerJson["New_Link_Direction"]["value"];
            }
            set
            {
                quickLinkerJson["New_Link_Direction"]["value"] = value;
            }
        }

        /// <summary>
        /// Se obtiene del alias del rol del extremo que estamos analizando.
        /// Se muestra cuando se crea solamente una relación (la estamos soltando sobre un elemento)
        /// </summary>
        public string NewLinkCaption
        {
            get
            {
                return (string)quickLinkerJson["New_Link_Caption"]["value"];
            }
            set
            {
                quickLinkerJson["New_Link_Caption"]["value"] = value;
            }
        }

        /// <summary>
        /// se toma de las notas del rol en el extremo que estamos analizando.
        /// Se muestra cuando soltamos la relación en el vacío.
        /// </summary>
        public string NewLinkElementCaption
        {
            get
            {
                return (string)quickLinkerJson["New_Link__Element_Caption"]["value"];
            }
            set
            {
                quickLinkerJson["New_Link__Element_Caption"]["value"] = value;
            }
        }

        /// <summary>
        /// 
        /// Un valor TRUE permite crear el link, en caso que no se permita entonces dejarlo en blanco.
        /// </summary>
        public string CreateLink
        {
            get
            {
                return (string)quickLinkerJson["Create_Link"]["value"];
            }
            set
            {
                quickLinkerJson["Create_Link"]["value"] = value;
            }
        }

        /// <summary>
        /// Si hay algo en newElementType entonces quiere decir que es posible crear un elemento.
        /// </summary>
        public string CreateElement
        {
            get
            {
                return (string)quickLinkerJson["Create_Element"]["value"];
            }
            set
            {
                quickLinkerJson["Create_Element"]["value"] = value;
            }
        }

        /// <summary>
        /// El valor se obtiene de "Restricciones" en el rol que se está evaluando.
        /// </summary>
        public string DisallowSelfConnector
        {
            get
            {
                return (string)quickLinkerJson["Disallow_Self_connector"]["value"];
            }
            set
            {
                quickLinkerJson["Disallow_Self_connector"]["value"] = value;
            }
        }

        /// <summary>
        /// nahh en blanco
        /// </summary>
        public string ExclusiveToSTFilterNoinheritFromMetatype
        {
            get
            {
                return (string)quickLinkerJson["Exclusive_to_ST_Filter__No_inherit_from_Metatype_Menu_Group"]["value"];
            }
            set
            {
                quickLinkerJson["Exclusive_to_ST_Filter__No_inherit_from_Metatype_Menu_Group"]["value"] = value;
            }
        }

        /// <summary>
        /// Se obtiene de el tipo de miembro del rol que se está evaluando.
        /// </summary>
        public string MenuGroup
        {
            get
            {
                return (string)quickLinkerJson["Menu_Group"]["value"];
            }
            set
            {
                quickLinkerJson["Menu_Group"]["value"] = value;
            }
        }

        /// <summary>
        /// Queda en 0, Sparx dice que no está implementado.
        /// </summary>
        public string ComplexityLevel
        {
            get
            {
                return (string)quickLinkerJson["Complexity_Level"]["value"];
            }
            set
            {
                quickLinkerJson["Complexity_Level"]["value"] = value;
            }
        }

        /// <summary>
        /// Si es true entonces el destino debería ser un elemento que contiene al origen, por ej: desde un port al componente que lo contiene.
        /// </summary>
        public string TargetMustBeParent
        {
            get
            {
                return (string)quickLinkerJson["Target_Must_Be_Parent"]["value"];
            }
            set
            {
                quickLinkerJson["Target_Must_Be_Parent"]["value"] = value;
            }
        }

        /// <summary>
        /// Si es TRUE entonces el elemento será creado como un elemento embebido.
        /// </summary>
        public string EmbedElement
        {
            get
            {
                return (string)quickLinkerJson["Embed_element"]["value"];
            }
            set
            {
                quickLinkerJson["Embed_element"]["value"] = value;
            }
        }

        /// <summary>
        /// Si es TRUE agrega un separador en el menú 
        /// </summary>
        public string PrecedesSeparatorLEAF
        {
            get
            {
                return (string)quickLinkerJson["Precedes_Separator_LEAF"]["value"];
            }
            set
            {
                quickLinkerJson["Precedes_Separator_LEAF"]["value"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PrecedesSeparatorGROUP
        {
            get
            {
                return (string)quickLinkerJson["Precedes_Separator_GROUP"]["value"];
            }
            set
            {
                quickLinkerJson["Precedes_Separator_GROUP"]["value"] = value;
            }
        }

        public string dummyColumn = "";
        internal Connector connector;

        public string toCsv()
        {
            return this.SourceElementType + "," + this.SourceStereotypeFilter + "," + this.TargetElementType + "," + this.TargetStereotypeFilter
                + "," + this.DiagramFilter + "," + this.NewElementType + "," + this.NewElementStereotype
                + "," + this.NewLinkType + "," + this.NewLinkStereotype + "," + this.NewLinkDirection
                + "," + this.NewLinkCaption + "," + this.NewLinkElementCaption + "," + this.CreateLink + "," + this.CreateElement
                + "," + this.DisallowSelfConnector + "," + this.ExclusiveToSTFilterNoinheritFromMetatype
                + "," + this.MenuGroup + "," + this.ComplexityLevel + "," + this.TargetMustBeParent + "," + this.EmbedElement
                + "," + this.PrecedesSeparatorLEAF + "," + this.PrecedesSeparatorGROUP + "," + this.dummyColumn; 
        }
    }
}
