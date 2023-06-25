using DMN.dominio;
using EA;
using DMN.classtable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using UIResources;

namespace DMN
{
    /// <summary>
    /// Responsable de implementar el comportamiento particular del addin, normalmente delegándolo.
    /// </summary>
    public class Main : MainCommons
    {
        Diagram currentDiagram = null;

        VariableEditor variableEditor = null;
        DMN.ui.JsonXmlViewer jsonXmlViewer = null;

        /// <summary>
        /// Dejamos este método acá para tener a mano la llamada que se hace.
        /// </summary>
        /// <param name="Repository"></param>
        /// <param name="Location"></param>
        /// <param name="MenuName"></param>
        /// <param name="ItemName"></param>
        public void EA_MenuClick(EA.Repository repository, string location, string MenuName, string ItemName)
        {
            if (ItemName == Properties.Resources.ITEM_MENU__GENERAR_DMN_XML)
            {
                exportDmn(repository, location);
            }
            else
            if (ItemName == Properties.Resources.ITEM_MENU__AGREGAR_VARIABLE)
            {
                this.addVar(repository, location);
            }
            else
            if (ItemName == Properties.Resources.ITEM_MENU__EDITOR_VARIABLE)
            {
                this.showEditor(repository, location);
            }
            else
            if (ItemName == Properties.Resources.ITEM_MENU__ENUM2CSV__ALIAS)
            {
                this.pasteClipboard(repository, location, true);
            }

            else
            if (ItemName == Properties.Resources.ITEM_MENU__ENUM2CSV__NAME)
            {
                this.pasteClipboard(repository, location, false);
            }

            else
            if (ItemName == Properties.Resources.ITEM_MENU__GENERAR_CODIGO)
            {
                this.generarCodigo(repository, location);
            }

            else
            if (ItemName == Properties.Resources.ITEM_MENU__CREAR_ENUMERACION)
            {
                this.crearEnum(repository, location);
            }

        }

        // ****************************************
        // *** Invocaciones desde shape script. ***
        // ****************************************

        /// <summary>
        /// Le indica al shape script si tiene que esconder o no la decoración que indica que la tabla no está definida.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="eaGuid"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string hasDecisionTable( Repository repository, string eaGuid, Object args)
        {
            Element element   = repository.GetElementByGuid(eaGuid);

            Decision decision = new Decision(element);
            DecisionBuilder builder = new DecisionBuilder(decision);

            builder.buildXmlEaTable(true);

            //Exporter exporter = new Exporter(eaUtils);

            //exporter.setDecisionFrom(element);

            return builder.buildXmlEaTable(true) ? "YES" : "NO";
        }

        public string markAsOutputDecision(Repository repository, string eaGuid, Object args)
        {
            try
            {
                Element element = repository.GetElementByGuid(eaGuid);

                EA.Attribute attr = null;
                foreach (EA.Attribute attrCurrent in element.Attributes)
                {
                    if (attrCurrent.Name == "isOutput")
                    {
                        attr = attrCurrent;
                        break;
                    }
                }

                if ( attr == null)
                {
                    // creamos atributo
                    attr = element.Attributes.AddNew("isOutput", "char");
                }
                // asignamos valor y guardamos los cambios.
                string value = ((string[])args)[0];
                if( attr.Default != value)
                {
                    attr.Default = value;
                    attr.Update();
                }
            }
            catch( Exception e ) { this.eaUtils.printOut(e.ToString()); }
            // la resppuesta no tiene relevancia.
            return "";
        }

        // *************************
        // *** Opciones de menú / Editor window. ***
        // *************************

        public void exportDmn( Repository repository, string location)
        {
            Alert.Error(Properties.Resources.no_implementada);
        }

        public void crearEnum( Element element)
        {
            Element businessKnowledgeElement = this.eaUtils.repository.GetElementByID(element.ParentID);

            ModelManager modelManager = new ModelManager(eaUtils);

            modelManager.createEnumeration(element, framework);
        }

        private void crearEnum(Repository repository, string location)
        {
            // listar las columnas para permitir elegir desde cual/cuales columnas se quiere crear la enumeración.
            Element element = this.getVariableSelected(repository, location);

            this.crearEnum(element);
        }

        private void pasteClipboard(Repository repository, string location, bool useAlias)
        {
            Element element = this.getSelected(repository, location, "Enumeration", null);
            string csv = this.eaUtils.elementUtils.enumeration2Csv(element, useAlias);

            Clipboard.SetText(csv);

            Alert.Success(Properties.Resources.copiado_portapapeles);
        }

        /// <summary>
        /// Llamado desde VariableEditor para que abra la ventana ViewJson.
        /// </summary>
        public void showAsJson( Element businessKnowledge, Package languageTargetPackage )
        {
            if (this.jsonXmlViewer == null)
            {
                this.jsonXmlViewer = (DMN.ui.JsonXmlViewer)eaUtils.repository.AddWindow(DMN.ui.JsonXmlViewer.WINDOW_NAME, "DMN.ui.JsonXmlViewer");
                eaUtils.repository.ShowAddinWindow(DMN.ui.JsonXmlViewer.WINDOW_NAME);
            }

            framework.loadTemplates(languageTargetPackage);

            DecisionJsonBuilder jsonBuilder = new DecisionJsonBuilder(this.eaUtils, businessKnowledge, framework);

            eaUtils.repository.ShowAddinWindow(DMN.ui.JsonXmlViewer.WINDOW_NAME);
            this.jsonXmlViewer.setJson( jsonBuilder );
        }

        private void showEditor(Element element)
        {
            if (this.variableEditor == null)
            {
                this.variableEditor = (VariableEditor)eaUtils.repository.AddWindow(VariableEditor.NAME, "DMN.VariableEditor");
                eaUtils.repository.ShowAddinWindow(VariableEditor.NAME);
            }
            if (element != null)
            {
                eaUtils.repository.ShowAddinWindow(VariableEditor.NAME);
                this.variableEditor.show(element, this.eaUtils, this);
            }
        }

        private void showEditor(Repository repository, string location)
        {
            this.eaUtils.setRepositorio(repository);
            this.showEditor(null);
        }

        public void addVar( Element element )
        {
            this.framework.createVariable(element);

            this.showEditor(element);

            Alert.Success(Properties.Resources.variable_creada);
        }
        public void addVar(Repository repository, string location )
        {
            
            Element rule = this.getBusinessKnowledgeSelected(repository, location);
            this.addVar(rule);
        }

        public void generarCodigo(Repository repository, string location)
        {
            Element businessKnowledgeElement = this.getBusinessKnowledgeSelected(repository, location);

            Alert.Error( Properties.Resources.no_implementada_desde_menu);
        }

        public void generarCodigo( Package languageTargetPackage, Element businessKnowledgeSelected )
        {
            ModelManager modelManager = new ModelManager(eaUtils);
            Element businessKnowledgeElement = this.getBusinessKnowledgeSelected(this.eaUtils.repository, Main.MENU_LOCATION__TREEVIEW);

            if (businessKnowledgeSelected != null)
            {
                businessKnowledgeElement = businessKnowledgeSelected;
            }
            else if( businessKnowledgeElement == null)
            {
                businessKnowledgeElement = this.getBusinessKnowledgeSelected(this.eaUtils.repository, Main.MENU_LOCATION__DIAGRAM);
            }

            framework.loadTemplates(languageTargetPackage);

            modelManager.exportCode(businessKnowledgeElement, framework);
        }

        // ******************************
        // *** Suscripcion a eventos. ***
        // ******************************
        public void EA_OnContextItemChanged(EA.Repository repository, String GUID, EA.ObjectType ot)
        {
            if( isProjectOpen(repository))
            {
                switch (ot)
                {
                    case EA.ObjectType.otElement:
                        try
                        {
                            Element element = repository.GetContextObject();
                            if (element.Type == "ActivityParameter" || element.Stereotype == "BusinessKnowledge")
                            {
                                //this.eaUtils.setRepositorio(repository);

                                Package packageOwner = repository.GetPackageByID(element.PackageID);
                                framework.setCurrentLanguage(packageOwner);


                                this.showEditor(element);
                            }
                            else
                            {
                                if (this.variableEditor != null)
                                {
                                    this.variableEditor.disable();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            this.eaUtils.printOut(e.ToString());
                            Alert.Error(Properties.Resources.error_indeterminado);
                        }

                        break;

                    case EA.ObjectType.otPackage:
                        try
                        {
                            Package package = repository.GetContextObject();
                            framework.setCurrentLanguage(package);
                        }
                        catch (Exception e)
                        {
                            this.eaUtils.printOut(e.ToString());
                        }
                        break;

                    case EA.ObjectType.otDiagram:
                        try
                        {
                            // si el diagrama es del tipo DMN entonces actualizar el framework.
                            Diagram diagram = repository.GetContextObject();
                            if (diagram.MetaType == "DMN::DMN" && (currentDiagram == null || currentDiagram.DiagramID != diagram.DiagramID || this.framework.currentLanguage == null))
                            {
                                currentDiagram = diagram;
                                framework.setCurrentLanguage(diagram);
                            }
                        }
                        catch (Exception e)
                        {
                            this.eaUtils.printOut(e.ToString());
                        }
                        break;

                    case EA.ObjectType.otAttribute:
                        break;

                    case EA.ObjectType.otMethod:
                        break;

                    case EA.ObjectType.otConnector:
                        break;

                    case EA.ObjectType.otRepository:
                        break;

                }
            }
        }


        /// <summary>
        /// http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_onpredeleteelement.html
        /// </summary>
        /// <param name="repository"></param>
        /// <param name=""></param>
        /// <param name="info"></param>
        public bool EA_OnPreDeleteElement(Repository repository, EventProperties info)
        {
            bool canDelete = true;

            try
            {
                Element elementToDelete = repository.GetElementByID(info.Get("ElementID").Value);

                if( elementToDelete.Stereotype == "Language")
                {
                    this.framework.currentLanguage = null;
                }

            }catch(Exception ) { }


            return canDelete;
        }


        /// <summary>
        /// http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/broadcastpostnewelement.html
        /// Elemento creado en un diagrama.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public Boolean EA_OnPostNewElement(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Element elemento = repository.GetElementByID(int.Parse((string)info.Get("ElementID").Value));

                // Se asigna un nombre uniforme y luego en el evento que avisa de la creación de un conector se le asigna el nombre de la decisión + "tabla".
                if( elemento.Stereotype == "BusinessKnowledge")
                {
                        elemento.Name = "BusinessKnowledge";
                        elemento.Update();
                        changed = true;
                }
               
                //MessageBox.Show("EA_OnPostNewElement - Conectores del elemento: " + elemento.Connectors.Count);

            }
            return changed;
        }

        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info )
        {
            Boolean doIt = true;

            if (repository != null && info != null)
            {
                Element element = null;
                EA.Attribute attribute = null;
                EA.Method method = null;
                try
                {
                    string type = info.Get("Type").Value;

                    if (type == "23")
                    {

                        attribute = repository.GetAttributeByID(info.Get("ID").Value);
                    }
                    else if (type == "24")
                    {
                        method = repository.GetMethodByID(info.Get("ID").Value);
                    }

                    else if (type == "4")
                    {

                        element = repository.GetElementByID(info.Get("ID").Value);

                    }
                    Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);

                    Element dropped = null;

                    try
                    {
                        dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                    }
                    catch (Exception) { }

                    if (dropped != null && dropped.Stereotype == "BusinessKnowledge")
                    {
                        if (element != null && (element.Stereotype == "Attribute" || element.Stereotype == "Data"))
                        {
                            Element e = dropped.EmbeddedElements.AddNew(element.Alias.Length != 0 ? element.Alias : element.Name, "ActivityParameter");
                            e.ClassifierID = element.ElementID;
                            e.Alias = element.Name;

                            e.Update();
                            dropped.EmbeddedElements.Refresh();

                            Alert.Success(String.Format(Properties.Resources.instanciado, element.Name));

                            doIt = false;
                        }
                        else if (attribute != null)
                        {

                            Element e = dropped.EmbeddedElements.AddNew(attribute.Alias.Length != 0 ? attribute.Alias : attribute.Name, "ActivityParameter");

                            this.eaUtils.instanceUtils.makeInstance(attribute, e);

                            e.Alias = e.Name;
                            e.Name = EAUtils.StringUtils.toPascal(e.Name.Replace(".", "-")).Replace("-", " ");

                            e.Update();
                            dropped.EmbeddedElements.Refresh();

                            Alert.Success(String.Format(Properties.Resources.instanciado, e.Name));

                            doIt = false;

                        }
                        else if (method != null)
                        {
                            Element e = dropped.EmbeddedElements.AddNew(method.Alias.Length != 0 ? method.Alias : method.Name, "ActivityParameter");

                            this.eaUtils.instanceUtils.makeInstance(method, e);

                            e.Alias = e.Name;

                            e.Update();
                            dropped.EmbeddedElements.Refresh();

                            Alert.Success(String.Format(Properties.Resources.instanciado, e.Name));

                            doIt = false;

                        }

                    }
                    if (element != null && element.Stereotype == "Language" && element.ClassifierID == 0 && diagram.MetaType == "DMN::DMN")
                    {
                        Package package = this.eaUtils.repository.GetPackageByID(diagram.PackageID);
                        Element newElement = package.Elements.AddNew("Language", "Class");
                        newElement.Stereotype = "Language";
                        newElement.ClassifierID = element.ElementID;
                        newElement.ClassifierName = element.Name;

                        newElement.Update();

                        diagram.Update();

                        Dictionary<string, int> dataChildPositionInfo = new Dictionary<string, int>();
                        dataChildPositionInfo["bottom"] = -50;
                        dataChildPositionInfo["left"] = 10;
                        dataChildPositionInfo["right"] = 90;
                        dataChildPositionInfo["top"] = -10;

                        this.eaUtils.diagramUtils.addDiagramElement(diagram, newElement, dataChildPositionInfo, null);

                        doIt = false;

                        //Alert.Success("El elemento ha sido reemplazado por una instancia");
                    }
                    if (element != null && element.Type == "ParamenterActivity" && dropped != null && dropped.Type == "Enumeration")
                    {
                        element.ClassifierID = dropped.ElementID;
                        element.Update();

                        Alert.Success(String.Format(Properties.Resources.instanciado_desde, element.Name, dropped.Name));

                        doIt = false;
                    }
                    //MessageBox.Show("EA_OnPostNewDiagramObject - Conectores del elemento: " + elemento.Connectors.Count);

                    if (dropped != null && dropped.Stereotype == "table")
                    {
                        if( element != null && element.Stereotype == "BusinessKnowledge")
                        {
                            Decision decision = new Decision(element);

                            DecisionBuilder decisionBuilder = new DecisionBuilder(decision, eaUtils, framework);

                            decisionBuilder.build(true);

                            BusinessKnwoledge2Table toTable = new BusinessKnwoledge2Table(repository, dropped);
                            toTable.toTable(decision);
                            doIt = false;

                            doIt = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    eaUtils.printOut(e.ToString());
                }
            }
            return doIt;
        }

        public Boolean EA_OnPostNewDiagramObject(EA.Repository repository, EA.EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                try
                {
                    Element element = repository.GetElementByID(int.Parse((string)info.Get("ID").Value));
                    Diagram       diagram           = repository.GetDiagramByID(int.Parse((string)info.Get("DiagramID").Value));
                    DiagramObject diagramObject = diagram.GetDiagramObjectByID( element.ElementID, ((string)info.Get("DUID").Value) );

                    Element current;
                    if (element.Stereotype == "DataType" && diagram.MetaType == "DMN::Language" && element.Connectors.Count == 0)
                    {
                       foreach(DiagramObject currentDiagramObject in diagram.DiagramObjects)
                        {
                            current = this.eaUtils.repository.GetElementByID(currentDiagramObject.ElementID);
                            if( current.Stereotype == "Language")
                            {
                                this.eaUtils.connectorUtils.addConnectorDependency(current, element, null, null);
                            }
                        }

                    }
                    
                }
                catch (Exception) { }
            }
            return changed;
        }

        public Boolean EA_OnPreNewConnector(Repository repository, EventProperties info)
        {
            Boolean add = true;

            if (repository != null && info != null)
            {
                //Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").asString));
            }
            return add;
        }

        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Diagram diagrama = repository.GetCurrentDiagram();

                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                // Si el elemento es nuevo (en el evento que avisa que se crea el elemento le asignamos el nombre y acá se lo cambiamos) y 
                if (diagrama.MetaType.Contains("DMN"))
                {
                    if( connector.Type == "Dependency")
                    {
                        Element origen = repository.GetElementByID(connector.ClientID);
                        Element destino = repository.GetElementByID(connector.SupplierID);
                        if( origen.Name == "BusinessKnowledge")
                        {
                            string businessKnowledgeSufijo = "tabla";
                            if( eaUtils.repositoryConfiguration.getLanguage() != "es")
                            {
                                businessKnowledgeSufijo = this.resourceManager.GetString( Properties.Resources.business_knowledge_sufijo_key, eaUtils.repositoryConfiguration.repositoryCulltureInfo());
                            }
                            origen.Name = destino.Name + " "+ businessKnowledgeSufijo;
                            origen.Update();
                        }
                        //MessageBox.Show("DMN post connector create - origen " + origen.Name +" destino "+ destino.Name);
                    }
                }
            }
            return changed;
        }
	}
}
