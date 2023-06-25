﻿using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAUtils;
using UIResources;

namespace RestFul
{
    class SwaggerResumen
    {
        EAUtils.EAUtils eaUtils;
        Package summaryPackage;
        Package definitionPackage;
        Dictionary<string,string> resourcesAndOperationsGUIDs = new Dictionary<string, string>();
        ResumenUtils resumenUtils;

        public SwaggerResumen(EAUtils.EAUtils eaUtils, Package summaryPackage, Package definitionPackage)
        {
            this.eaUtils           = eaUtils;
            this.summaryPackage    = summaryPackage;
            this.definitionPackage = definitionPackage;
            this.resumenUtils      = new ResumenUtils(this.eaUtils);
        }

        public void makeSummary()
        {
            Diagram diagram;
            if (summaryPackage.Diagrams.Count != 0)
            {
                diagram = summaryPackage.Diagrams.GetAt(0);
            }
            else
            {
                string diagramStereotype = "";
                if (this.eaUtils.repositoryConfiguration.getLanguage() == "es")
                {
                    diagramStereotype = "APIRestResumen";
                }
                else
                {
                    diagramStereotype = "APIRestSummary";
                }

                diagram = summaryPackage.Diagrams.AddNew(summaryPackage.Name, diagramStereotype);
                diagram.Stereotype = diagramStereotype;
                diagram.Update();
            }

            // buscar los datos desde el modelo
            Element element;
            Element uri = null;
            List<string> interfaces = new List<string>();
            EA.Method method;

            for (short i = 0; i < definitionPackage.Elements.Count; i++)
            {
                element = definitionPackage.Elements.GetAt(i);
                if (element.Stereotype == "URI")
                {
                    uri = element;
                }
                if (element.Type == "Interface" && element.Stereotype == "")
                {
                    interfaces.Add(element.Alias);
                    resourcesAndOperationsGUIDs.Add( element.Alias.Substring(1), element.ElementGUID);

                    for (short m = 0; m < element.Methods.Count; m++)
                    {
                        method = element.Methods.GetAt(m);
                        interfaces.Add(element.Alias + "/__" + method.Name);
                        resourcesAndOperationsGUIDs.Add(element.ElementGUID + "/__" + method.Name, method.MethodGUID);
                    }
                }
            }

            // generar el elemento root del arbol.
            if( uri != null)
            {
                Element elementRoot;
                var rootDOE = new DiagramObjectExt();

                elementRoot = generateSummaryRoot(summaryPackage, diagram, uri, rootDOE);

                List<DiagramObjectExt> diagramObjectExtList = new List<DiagramObjectExt>();

                var treeList = new TreeList('/');
                treeList.build(interfaces);

                //Session.Output(JSON.stringify(treeList.tree));
                //treeList.printFormat( treeList.tree, 1 );

                var diagramObjectExtParent = new DiagramObjectExt(diagram.DiagramObjects.GetAt(0));
                diagramObjectExtList.Add(diagramObjectExtParent);

                InfoBranch infoBranch = new InfoBranch();

                infoBranch.diagramObjectExtList = diagramObjectExtList;
                infoBranch.packageSummary = summaryPackage;
                infoBranch.diagram = diagram;
                infoBranch.vars.treelist = treeList.tree;
                infoBranch.vars.diagramObjectExtParent = diagramObjectExtParent;
                infoBranch.vars.elementParent = elementRoot;

                generateSummaryMakeBranch(infoBranch, 1);

                // establecemos el estilo de las relaciones.

                EA.DiagramLink diagramLink;

                for (short i = 0; i < diagram.DiagramLinks.Count; i++)
                {
                    diagramLink = diagram.DiagramLinks.GetAt(i);

                    resumenUtils.diagramLinkStyle();
                }
                this.eaUtils.repository.OpenDiagram(diagram.DiagramID);
            }
            else
            {
                Alert.Error("URI");
            }
        }

        public Element generateSummaryRoot(Package packageSummary, Diagram diagram, Element element, DiagramObjectExt rootDOE)
        {
            Element elementRoot;
            elementRoot = packageSummary.Elements.AddNew(element.Name, "Class");
            elementRoot.Stereotype = "URI";
            elementRoot.Update();

            // le agregamos el guid del elemento que le da origen.
            this.eaUtils.taggedValuesUtils.set(elementRoot, "source.guid", element.ElementGUID);

            rootDOE.position.offset.top = 10;
            rootDOE.position.offset.left = 10;
            rootDOE.position.height = 20;
            rootDOE.position.width = 320;

            rootDOE.style = "BCol=16777215;LCol=16777215;font=Calibri;fontsz=100;bold=1;black=0;italic=0;ul=0;charset=0;pitch=34;BFol=0;DUID=92294773;";

            rootDOE.position.calculate();

            Dictionary<string, int> info = new Dictionary<string, int>();
            info.Add("top", rootDOE.position.top);
            info.Add("bottom", rootDOE.position.bottom);
            info.Add("right", rootDOE.position.right);
            info.Add("left", rootDOE.position.left);

            DiagramObject newDiagramObject = this.eaUtils.diagramUtils.addDiagramElement(diagram, elementRoot, info, rootDOE.style);
            rootDOE.diagramObject = newDiagramObject;
            rootDOE.position = new DiagramObjectPosition(newDiagramObject);

            return elementRoot;
        }

        public void generateSummaryMakeBranch(InfoBranch infoBranch, int level)
        {
            Element elementBranch;

            var branch = infoBranch.vars.treelist;
            var elementParent = infoBranch.vars.elementParent;
            var diagramObjectExtParent = infoBranch.vars.diagramObjectExtParent;
            string label;
            DiagramObject parentDiargamObject = infoBranch.vars.diagramObjectExtParent.diagramObject;
            string keyForGuid;

            for (int i = 0; i < branch.Count; i++)
            {
                DiagramObjectExt diagramObjectExt = null;

                // metodo
                if (branch[i].label.IndexOf("__") != -1)
                {
                    label         = branch[i].label.Substring(2);
                    keyForGuid    = this.eaUtils.taggedValuesUtils.get(elementParent, "source.guid", "").asString() + "/" + branch[i].label;
                    label         = label.ToUpper();
                    elementBranch = elementParent.Elements.AddNew(label, "Class");

                    elementBranch.StereotypeEx = "Services::" + label;

                    diagramObjectExt = resumenUtils.methodStyle();
                }
                else
                {
                    // path
                    label      = ("/" + branch[i].label );
                    keyForGuid = branch[i].label;

                    diagramObjectExt = resumenUtils.resourceStyle();

                    elementBranch              = elementParent.Elements.AddNew(label, "Class");
                    elementBranch.StereotypeEx = "Services::Resource";
                }

                elementBranch.Update();

                // agregamos source guid
                try
                {
                    this.eaUtils.taggedValuesUtils.set(elementBranch, "source.guid", resourcesAndOperationsGUIDs[keyForGuid]);
                }
                catch (Exception) { }

                if (level == 1 && i != 0)
                {
                    diagramObjectExtParent = infoBranch.diagramObjectExtList[ infoBranch.diagramObjectExtList.Count - 1 ];
                }

                //diagramObjectExt.position.offset.left = 20;

                diagramObjectExt.position.calculateVerticalTree( diagramObjectExtParent.diagramObject, parentDiargamObject );

                Dictionary<string, int> info = new Dictionary<string, int>();

                info.Add( "top"   , diagramObjectExt.position.top    );
                info.Add( "bottom", diagramObjectExt.position.bottom );
                info.Add( "right" , diagramObjectExt.position.right  );
                info.Add( "left"  , diagramObjectExt.position.left   );

                DiagramObject newDiagramObject = this.eaUtils.diagramUtils.addDiagramElement(infoBranch.diagram, elementBranch, info, diagramObjectExt.style);
                diagramObjectExt.diagramObject = newDiagramObject;

                infoBranch.diagramObjectExtList.Add(diagramObjectExt);

                eaUtils.connectorUtils.addConnectorAssociation(elementParent, elementBranch, null,"APIRestConnector");

                infoBranch.vars.treelist               = branch[i].nodes;
                infoBranch.vars.diagramObjectExtParent = diagramObjectExt;
                infoBranch.vars.elementParent          = elementBranch;

                generateSummaryMakeBranch(infoBranch, ( level + i ) );
            }
        }
    }
    public class InfoBranch
    {
        public List<DiagramObjectExt> diagramObjectExtList;
        public Package packageSummary;
        public Diagram diagram;
        public Vars vars = new Vars();

    }
    public class Vars
    {
        public List<TreeNode> treelist;
        public DiagramObjectExt diagramObjectExtParent;
        public Element elementParent;
    }
}
