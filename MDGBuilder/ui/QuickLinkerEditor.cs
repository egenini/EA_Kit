using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EA;
using MDGBuilder.mdg;

namespace MDGBuilder.ui
{
    public partial class QuickLinkerEditor : UserControl
    {
        internal static string NAME = "QuickLinker Editor";

        public EAUtils.EAUtils eaUtils;
        QuickLinkerBuilder quickLinkerBuilder = null;
        Element source = null;
        Element target = null;
        QuickLinkerLine quickLinkerLine = null;
        Connector connector = null;

        public bool isChanged = false;

        public QuickLinkerEditor()
        {
            InitializeComponent();

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.label_SourceElementType, "Identifies a valid source element in the Profile.\nIf a connector is being dragged away from this type of element, the row is evaluated. Otherwise, the row is ignored.");

            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.label_SourceStereotypeFilter, "Identifies a stereotype of the source element base type.\nIf set, and if a connector is being dragged away from an element of this stereotype, the row is evaluated. Otherwise, the row is ignored.\nEvent source element can be a normal Event, or a Start Event, Intermediate Event or End Event stereotyped element");

            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.label_TargetElementType, "Identifies a valid target element in the Profile.\nIf set, and if a connector is being dragged onto this type of element, the row is evaluated. If blank, and if a connector is being dragged onto an empty space on the diagram, the row is evaluated. Otherwise the row is ignored.");

            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.label_TargetStreotypeFilter, "Identifies a stereotype of the target element base type.\nIf set, if Target Element Type is also set, and if a connector is being dragged onto an element of this stereotype, the row is evaluated. Otherwise, the row is ignored.");

            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.label_DiagramFilter, "Contains either an inclusive list or an exclusive list of diagram types.\nlimits the diagrams the specified connector can be created on.\nExamples:\nCollaboration;Object;Custom;\n- Custom diagram types from MDG Technologies can be referenced using the fully qualified diagram type (DiagramProfile::DiagramType); for example:BPMN2.0::Business Process;BPMN2.0::Choreography;BPMN2.0::Collaboration\nAs a shorthand for all diagram types in a diagram profile you can use the '*' wildcard, which must be preceded by the diagram profile ID; for example:BPMN2.0::*;\nEach excluded diagram name is preceded by an exclamation mark; for example:!Sequence;\nEach diagram name is terminated by a semi-colon");

            System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            ToolTip6.SetToolTip(this.label_NewElementType, "Defines the type of element to be created.\nThe connector is dragged into open space, provided that the Create Element field is set to TRUE.");

            System.Windows.Forms.ToolTip ToolTip7 = new System.Windows.Forms.ToolTip();
            ToolTip7.SetToolTip(this.label_NewElementStereotype, "Defines the type of element stereotype to be created.\nThe connector is dragged into open space, provided that the Create Element field is set to TRUE.");

            System.Windows.Forms.ToolTip ToolTip8 = new System.Windows.Forms.ToolTip();
            ToolTip8.SetToolTip(this.label_NewLinkType, "Defines the type of connector to create.\nIf Create Link is also set to TRUE.");

            System.Windows.Forms.ToolTip ToolTip9 = new System.Windows.Forms.ToolTip();
            ToolTip9.SetToolTip(this.label_NewLinkStereotype, "Defines the type of connector stereotype to create.\nIf Create Link is also set to TRUE.");

            System.Windows.Forms.ToolTip ToolTip10 = new System.Windows.Forms.ToolTip();
            ToolTip10.SetToolTip(this.label_NewLinkStereotype, "Defines the connector direction.\nDirected (always creates an association from source to target), from (always creates an association from target to source), undirected (always creates an association with unspecified direction), bidirectional (always creates a bi-directional association), to (creates either a directed or undirected association, depending on the value of the Association Direction field)");

            System.Windows.Forms.ToolTip ToolTip11 = new System.Windows.Forms.ToolTip();
            ToolTip11.SetToolTip(this.label_NewLinkCaption, "Defines the text to display in the Quick Linker menu.\nA new connector is being created but not a new element.");

            System.Windows.Forms.ToolTip ToolTip22 = new System.Windows.Forms.ToolTip();
            ToolTip22.SetToolTip(this.label_NewLinkDirection, "Defines the connector direction.\ndirected (always creates an association from source to target), from (always creates an association from target to source), undirected (always creates an association with unspecified direction), bidirectional (always creates a bi-directional association), to (creates either a directed or undirected association, depending on the value of the Association Direction field)");

            System.Windows.Forms.ToolTip ToolTip12 = new System.Windows.Forms.ToolTip();
            ToolTip12.SetToolTip(this.label_NewLinkElementCaption, "Defines the text to display in the Quick Linker menu.\nA new connector AND a new element are being created.");

            System.Windows.Forms.ToolTip ToolTip13 = new System.Windows.Forms.ToolTip();
            ToolTip13.SetToolTip(this.label_CreateLink, "If set to TRUE, results in the creation of a new connector; leave blank to stop the creation of a connector.");

            System.Windows.Forms.ToolTip ToolTip14 = new System.Windows.Forms.ToolTip();
            ToolTip14.SetToolTip(this.label_CreateElement, "If set to TRUE and a connector is being dragged onto an empty space on the diagram,results in the creation of a new element. Leave blank to stop the element from being created. This overrides the values of Target Element Type and Target Stereotype Filter.");

            System.Windows.Forms.ToolTip ToolTip15 = new System.Windows.Forms.ToolTip();
            ToolTip15.SetToolTip(this.label_DisallowSelfConnector, "Set to TRUE if self connectors are invalid for this kind of connector; otherwise leave this field blank.");

            System.Windows.Forms.ToolTip ToolTip16 = new System.Windows.Forms.ToolTip();
            ToolTip15.SetToolTip(this.label_Exclusive, "Set to TRUE to indicate that elements of type Source Element Type with the stereotype Source Stereotype Filter do not display the Quick Linker definitions of the equivalent unstereotyped element.");

            System.Windows.Forms.ToolTip ToolTip17 = new System.Windows.Forms.ToolTip();
            ToolTip17.SetToolTip(this.label_MenuGroup, "Indicates the name of the submenu in which a menu item is created.\nApplies when creating a new element; that is, the user is dragging from an element to an empty space on the diagram, or over a target element to create  a new embedded element.");

            System.Windows.Forms.ToolTip ToolTip18 = new System.Windows.Forms.ToolTip();
            ToolTip18.SetToolTip(this.label_TargetMustBeParent, "Set to TRUE if the menu item should only appear when dragging from a child element to its parent; for example, from a Port to its containing Class. Otherwise leave this field blank");

            System.Windows.Forms.ToolTip ToolTip19 = new System.Windows.Forms.ToolTip();
            ToolTip19.SetToolTip(this.label_EmbedElement, "Set to TRUE to embed the element being created in the target element; otherwise leave this field blank.");

            System.Windows.Forms.ToolTip ToolTip20 = new System.Windows.Forms.ToolTip();
            ToolTip20.SetToolTip(this.label_PrecedesSeparatorLEAF, "Set to TRUE to add a menu item separator to the Quick Linker menu, underneath this entry; otherwise leave this field blank.");

            System.Windows.Forms.ToolTip ToolTip21 = new System.Windows.Forms.ToolTip();
            ToolTip21.SetToolTip(this.label_PrecedesSeparatorGROUP, "Set to TRUE to add a menu item group separator to the Quick Linker sub-menu; otherwise leave this field blank.");

            System.Windows.Forms.ToolTip ToolTip23 = new System.Windows.Forms.ToolTip();
            ToolTip23.SetToolTip(this.label_ComplexityLevel, "Contains numerical bitmask values that identify complex functionality.");
        }

        public void show(Connector connector)
        {
            this.connector = connector;

            if(quickLinkerBuilder == null)
            {
                quickLinkerBuilder = new QuickLinkerBuilder(this.eaUtils);
            }

            source          = this.eaUtils.repository.GetElementByID( connector.ClientID);
            target          = this.eaUtils.repository.GetElementByID( connector.SupplierID);
            quickLinkerLine = quickLinkerBuilder.buildLine( source, target, connector );

            control_CreateElement.Text          = quickLinkerLine.CreateElement;
            control_CreateLink.Text             = quickLinkerLine.CreateLink;
            control_EmbedElement.Text           = quickLinkerLine.EmbedElement;
            control_Exclusive.Text              = quickLinkerLine.ExclusiveToSTFilterNoinheritFromMetatype;
            control_DiagramFilter.Text          = quickLinkerLine.DiagramFilter;
            control_DisallowSelfConnector.Text  = quickLinkerLine.DisallowSelfConnector;
            control_MenuGroup.Text              = quickLinkerLine.MenuGroup;
            control_NewElementStereotype.Text   = quickLinkerLine.NewElementStereotype;
            control_NewElementType.Text         = quickLinkerLine.NewElementType;
            control_NewElementStereotype.Text   = quickLinkerLine.NewElementStereotype;
            control_NewLinkCaption.Text         = quickLinkerLine.NewLinkCaption;
            control_NewLinkDirection.Text       = quickLinkerLine.NewLinkDirection;
            control_NewLinkElementCaption.Text  = quickLinkerLine.NewLinkElementCaption;
            control_NewLinkStereotype.Text      = quickLinkerLine.NewLinkStereotype;
            control_NewLinkType.Text            = quickLinkerLine.NewLinkType;
            control_PrecedesSeparatorGROUP.Text = quickLinkerLine.PrecedesSeparatorGROUP;
            control_PrecedesSeparatorLEAF.Text  = quickLinkerLine.PrecedesSeparatorLEAF;
            control_SoruceElementType.Text      = quickLinkerLine.SourceElementType;
            control_SourceStereotypeFilter.Text = quickLinkerLine.SourceStereotypeFilter;
            control_TargetElementType.Text      = quickLinkerLine.TargetElementType;
            control_TargetStreotypeFilter.Text  = quickLinkerLine.TargetStereotypeFilter;
            control_TargetMustBeParent.Text     = quickLinkerLine.TargetMustBeParent;
            control_ComplexityLevel.Text        = quickLinkerLine.ComplexityLevel;
        }

        public void save()
        {

            isChanged = false;

            quickLinkerBuilder.update(quickLinkerLine);

        }

        private void control_CreateElement_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.CreateElement = control_CreateElement.Text;

            isChanged = true;
        }

        private void control_NewLinkDirection_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.NewLinkDirection = control_NewLinkDirection.Text;
            isChanged = true;
        }

        private void control_DiagramFilter_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.DiagramFilter = control_DiagramFilter.Text;
            isChanged = true;
        }

        private void control_TargetMustBeParent_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.TargetMustBeParent = control_TargetMustBeParent.Text;
            isChanged = true;
        }

        private void control_EmbedElement_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.EmbedElement = control_EmbedElement.Text;
            isChanged = true;
        }

        private void control_CreateLink_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.CreateLink = control_CreateLink.Text;
            isChanged = true;
        }

        private void control_NewLinkCaption_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.NewLinkCaption = control_NewLinkCaption.Text;
            isChanged = true;
        }

        private void control_NewLinkElementCaption_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.NewLinkElementCaption = control_NewLinkElementCaption.Text;
            isChanged = true;
        }

        private void control_DisallowSelfConnector_SelectedIndexChanged(object sender, EventArgs e)
        {
            quickLinkerLine.DisallowSelfConnector = control_DisallowSelfConnector.Text;
            isChanged = true;
        }

        private void control_Exclusive_SelectedValueChanged(object sender, EventArgs e)
        {
            quickLinkerLine.ExclusiveToSTFilterNoinheritFromMetatype = control_Exclusive.Text;
            isChanged = true;
        }

        private void control_MenuGroup_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.MenuGroup = control_MenuGroup.Text;
            isChanged = true;
        }

        private void control_PrecedesSeparatorLEAF_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.PrecedesSeparatorLEAF = control_PrecedesSeparatorLEAF.Text;
            isChanged = true;
        }

        private void control_PrecedesSeparatorGROUP_SelectedIndexChanged(object sender, EventArgs e)
        {
            quickLinkerLine.PrecedesSeparatorGROUP = control_PrecedesSeparatorGROUP.Text;
            isChanged = true;
        }

        private void control_NewLinkType_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.NewLinkType = control_NewLinkType.Text;
            isChanged = true;
        }

        private void control_NewLinkStereotype_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.NewLinkStereotype = control_NewLinkStereotype.Text;
            isChanged = true;
        }
        private void control_ComplexityLevel_TextChanged(object sender, EventArgs e)
        {
            quickLinkerLine.ComplexityLevel = control_ComplexityLevel.Text;
            isChanged = true;
        }
    }
}
