using EA;
using EAUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace MDGBuilder.mdg
{
    public class QuickLinkerBuilder
    {
        const string TGV_QUICKLINKER__NAME = "Quicklinker";
        const string TGV_QUICKLINKER__VALUE = "{\"Source_Element_Type\":{\"description\":\"Identifies a valid source element in the Profile.\",\"behavior\":\"If a connector is being dragged away from this type of element, the row is evaluated. Otherwise, the row is ignored.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"A\"},\"Source_Stereotype_Filter\":{\"description\":\"Identifies a stereotype of the source element base type\",\"behavior\":\"If set, and if a connector is being dragged away from an element of this stereotype, the row is evaluated. Otherwise, the row is ignored.\",\"example\":\"Event source element can be a normal Event, or a Start Event, Intermediate Event or End Event stereotyped element\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"B\"},\"Target_Element_Type\":{\"description\":\"Identifies a valid target element in the Profile.\",\"behavior\":\"If set, and if a connector is being dragged onto this type of element, the row is evaluated. If blank, and if a connector is being dragged onto an empty space on the diagram, the row is evaluated. Otherwise the row is ignored.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"C\"},\"Target_Stereotype_Filter\":{\"description\":\"Identifies a stereotype of the target element base type.\",\"behavior\":\"If set, if Target Element Type is also set, and if a connector is being dragged onto an element of this stereotype, the row is evaluated. Otherwise, the row is ignored.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"D\"},\"Diagram_Filter\":{\"description\":\"Contains either an inclusive list or an exclusive list of diagram types.\",\"behavior\":\"limits the diagrams the specified connector can be created on.\",\"example\":\"Collaboration;Object;Custom;  		             					- Custom diagram types from MDG Technologies can be referenced using the fully qualified diagram type (DiagramProfile::DiagramType); for example:                           BPMN2.0::Business Process;BPMN2.0::Choreography;BPMN2.0::Collaboration 					 					- As a shorthand for all diagram types in a diagram profile you can use the '*' wildcard, which must be preceded by the diagram profile ID; for example:   					    BPMN2.0::*; 						 					- Each excluded diagram name is preceded by an exclamation mark; for example:                            !Sequence; 						 					--- Each diagram name is terminated by a semi-colon                     \",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"E\"},\"New_Element_Type\":{\"description\":\"Defines the type of element to be created.\",\"behavior\":\"The connector is dragged into open space, provided that the Create Element field is set to TRUE.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"F\"},\"New_Element_Stereotype\":{\"description\":\"Defines the type of element stereotype to be created.\",\"behavior\":\"The connector is dragged into open space, provided that the Create Element field is set to TRUE.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"G\"},\"New_Link_Type\":{\"description\":\"Defines the type of connector to create.\",\"behavior\":\"If Create Link is also set to TRUE.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"H\"},\"New_Link_Stereotype\":{\"description\":\"Defines the type of connector stereotype to create.\",\"behavior\":\"If Create Link is also set to TRUE.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"I\"},\"New_Link_Direction\":{\"description\":\"Defines the connector direction.\",\"behavior\":\"directed (always creates an association from source to target), from (always creates an association from target to source), undirected (always creates an association with unspecified direction), bidirectional (always creates a bi-directional association), to (creates either a directed or undirected association, depending on the value of the Association Direction field) \",\"example\":\"\",\"allowed_values\":[\"directed\",\"from\",\"undirected\",\"bidirectional\",\"to\"],\"value\":\"to\",\"csv_column\":\"J\"},\"New_Link_Caption\":{\"description\":\"Defines the text to display in the Quick Linker menu.\",\"behavior\":\"A new connector is being created but not a new element.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"K\"},\"New_Link__Element_Caption\":{\"description\":\"Defines the text to display in the Quick Linker menu.\",\"behavior\":\"A new connector AND a new element are being created.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"L\"},\"Create_Link\":{\"description\":\"\",\"behavior\":\"If set to TRUE, results in the creation of a new connector; leave blank to stop the creation of a connector.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"TRUE\",\"csv_column\":\"M\"},\"Create_Element\":{\"description\":\"\",\"behavior\":\"If set to TRUE and a connector is being dragged onto an empty space on the diagram,results in the creation of a new element. Leave blank to stop the element from being created. This overrides the values of Target Element Type and Target Stereotype Filter.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"TRUE\",\"csv_column\":\"N\"},\"Disallow_Self_connector\":{\"description\":\"\",\"behavior\":\"Set to TRUE if self connectors are invalid for this kind of connector; otherwise leave this field blank.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"\",\"csv_column\":\"O\"},\"Exclusive_to_ST_Filter__No_inherit_from_Metatype_Menu_Group\":{\"description\":\"\",\"behavior\":\"Set to TRUE to indicate that elements of type Source Element Type with the stereotype Source Stereotype Filter do not display the Quick Linker definitions of the equivalent unstereotyped element.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"TRUE\",\"csv_column\":\"P\"},\"Menu_Group\":{\"description\":\"Indicates the name of the submenu in which a menu item is created.\",\"behavior\":\"applies when creating a new element; that is, the user is dragging from an element to an empty space on the diagram, or over a target element to create  a new embedded element.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"Q\"},\"Complexity_Level\":{\"description\":\"Not implemented, always set to 0.\",\"behavior\":\"\",\"example\":\"\",\"allowed_values\":[0],\"value\":\"\",\"csv_column\":\"R\"},\"Target_Must_Be_Parent\":{\"description\":\"\",\"behavior\":\"Set to TRUE if the menu item should only appear when dragging from a child element to its parent; for example, from a Port to its containing Class. Otherwise leave this field blank\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"\",\"csv_column\":\"S\"},\"Embed_element\":{\"description\":\"\",\"behavior\":\"Set to TRUE to embed the element being created in the target element; otherwise leave this field blank.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"\",\"csv_column\":\"T\"},\"Precedes_Separator_LEAF\":{\"description\":\"\",\"behavior\":\"Set to TRUE to add a menu item separator to the Quick Linker menu, underneath this entry; otherwise leave this field blank.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"TRUE\",\"csv_column\":\"U\"},\"Precedes_Separator_GROUP\":{\"description\":\"\",\"behavior\":\"Set to TRUE to add a menu item group separator to the Quick Linker sub-menu; otherwise leave this field blank.\",\"example\":\"\",\"allowed_values\":[\"TRUE\",\"\"],\"value\":\"TRUE\",\"csv_column\":\"\"},\"Dummy_Column\":{\"description\":\"\",\"behavior\":\"Depending on which spreadsheet application you use, this column might require a value in every cell to force a CSV export to work correctly with trailing blank values.\",\"example\":\"\",\"allowed_values\":[],\"value\":\"\",\"csv_column\":\"W\"}}";

        public EAUtils.EAUtils eaUtils;
        public List<QuickLinkerLine> lines = new List<QuickLinkerLine>();
        ModelUtil modelUtil;
        private string mdgName = "";

        public QuickLinkerBuilder( EAUtils.EAUtils eaUtils )
        {
            this.eaUtils = eaUtils;
            this.modelUtil = new ModelUtil(this.eaUtils);
            Package mdgPackage = null;

            if (this.eaUtils.repository.GetCurrentDiagram() != null)
            {
                Package currentPackage = this.eaUtils.repository.GetPackageByID(this.eaUtils.repository.GetCurrentDiagram().PackageID);
                
                mdgPackage = this.eaUtils.repository.GetPackageByID(currentPackage.ParentID);
            }
            else
            {
                mdgPackage = this.eaUtils.repository.GetTreeSelectedPackage();
            }

            mdgName = mdgPackage.Name;
        }

        public string getLinesAsString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(QuickLinkerLine.HEADER_LINE).Append("\r\n");

            foreach ( QuickLinkerLine line in lines)
            {
                builder.Append(line.toCsv()).Append("\r\n");
            }
            return builder.ToString();
        }

        public bool build(Package package)
        {
            bool canBuild = package.Elements.Count != 0;
            if ( canBuild)
            {
                Element currentClassifier;
                Element elementTarget;
                Connector connector;

                foreach( Element current in package.Elements)
                {
                    currentClassifier = null;

                    if ( current.ClassifierID != 0)
                    {
                        currentClassifier = this.eaUtils.repository.GetElementByID(current.ClassifierID);
                    }

                    // busco la otra punta de la relación.
                    List < ElementConnectorInfo > elementConnectorInfoList = this.eaUtils.connectorUtils.get(current, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, null, null, null, false, null);

                    foreach(ElementConnectorInfo elementConnectorInfo in elementConnectorInfoList)
                    {
                        elementTarget = elementConnectorInfo.element;
                        connector     = elementConnectorInfo.connector;

                        this.lines.Add(buildLine(current, elementTarget, connector));
                    }
                }
            }
            return canBuild;
        }

        public QuickLinkerLine buildLine(Element source, Element target, Connector connector)
        {
            QuickLinkerLine line = new QuickLinkerLine();

            EA.Element metaclassElement;
            string sourceMetaclassName = "";
            string targetMetaclassName = "";

            bool hasTv = false;
            // si no tiene valor etiquetado lo inicializo.
            string jsonString = this.eaUtils.taggedValuesUtils.get(connector, TGV_QUICKLINKER__NAME, "").asString();

            hasTv = jsonString != "";

            if ( ! hasTv )
            {
                jsonString = TGV_QUICKLINKER__VALUE;
            }

            line.quickLinkerJson = JObject.Parse(jsonString);
            line.connector = connector;

            // @todo analizar las relaciones de quicklinker que no son con elementos del mismo mdg.
            this.modelUtil.reset();

            metaclassElement = this.modelUtil.getMetaClass(source, mdgName);
            StereotypeInfo sourceInfo = this.modelUtil.info;
            metaclassElement = metaclassElement == null ? sourceInfo.deepTypeAsElement() : metaclassElement;

            if (metaclassElement != null)
            {
                if (metaclassElement.Stereotype == "metaclass")
                {
                    sourceMetaclassName = metaclassElement.Name;
                }
                else
                {
                    sourceMetaclassName = metaclassElement.Tag;
                }
            }

            this.modelUtil.reset();

            metaclassElement = this.modelUtil.getMetaClass(target, mdgName);
            StereotypeInfo targetInfo = this.modelUtil.info;
            metaclassElement = metaclassElement == null ? targetInfo.deepTypeAsElement() : metaclassElement;

            if (metaclassElement != null)
            {
                if (metaclassElement.Stereotype == "metaclass")
                {
                    targetMetaclassName = metaclassElement.Name;
                }
                else
                {
                    targetMetaclassName = metaclassElement.Tag;
                }
            }

            line.SourceElementType      = sourceMetaclassName;
            line.SourceStereotypeFilter = sourceInfo.getStereotype();

            line.TargetElementType      = targetMetaclassName;
            line.TargetStereotypeFilter = targetInfo.getStereotype();

            line.NewElementType       = line.TargetElementType;
            line.NewElementStereotype = line.TargetStereotypeFilter;

            line.NewLinkType       = connector.Type;
            line.NewLinkStereotype = connector.Stereotype;

            if (!hasTv)
            {
                this.eaUtils.taggedValuesUtils.set(connector, TGV_QUICKLINKER__NAME, line.quickLinkerJson.ToString(), true);
            }

            return line;
        }

        public void update( QuickLinkerLine line)
        {
            this.eaUtils.taggedValuesUtils.set(line.connector, TGV_QUICKLINKER__NAME, line.quickLinkerJson.ToString(), true);

        }
    }
}
