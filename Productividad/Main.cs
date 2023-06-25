using EA;
using EAUtils;
using Productividad.classtable;
using Productividad.framework.domain;
using Productividad.table2class;
using System;
using System.Windows.Forms;
using UIResources;

namespace Productividad

{
    /// <summary>
    /// Responsable de implementar el comportamiento particular del addin, normalmente delegándolo.
    /// </summary>
    public class Main : MainCommons
    {

        private JsonXmlViewer viewJson = null;
        private JsonXmlViewer viewXml = null;

        /// <summary>
        /// Dejamos este método acá para tener a mano la llamada que se hace.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="location"></param>
        /// <param name="menuName"></param>
        /// <param name="itemName"></param>
        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            switch (itemName)
            {
                case ITEM_MENU__VER_MENSAJE_AS_JSON:

                    showViewAs(repository, true);

                    break;
                case ITEM_MENU__VER_MENSAJE_AS_XML:

                    showViewAs(repository, false);

                    break;
                case ITEM_MENU__GENERAR_CODIGO:

                    pasteEnumCode(repository, location);

                    break;
                case ITEM_MENU__GENERAR_CODIGO_CLASS:

                    pasteClassCode(repository, location);

                    break;
                case ITEM_MENU__INSTANCIAR_DESDE_CONECTOR:

                    simpleInstantiation(repository);

                    break;
                /*
                case ITEM_MENU__GENERAR_DAO:

                    generarDao(this.getTableSelected(repository), repository);

                    break;
                case ITEM_MENU__GENERAR_POJO:

                    generarPojo(this.getTableSelected(repository), repository);

                    break;

                case ITEM_MENU__GENERAR_POJO_METADATA:

                    generarPojoMetadata(this.getTableSelected(repository), repository);

                    break;
                */
                case ITEM_MENU__ETIQUETAS:

                    generarEtiquetas(this.getClassSelected(repository), repository);

                    break;
                /*
                case ITEM_MENU__TEST_JIRA:

                    login( repository);

                    break;
                */
                case ITEM_MENU__JSON_2_CLASS:

                    json2Class(this.getClassSelected(repository), repository);

                    break;

                case ITEM_MENU__MOVE_ELEMENT_TO_ACTIVE_PACKAGE:

                    moveToActivePackage(this.getElementSelected(repository), repository);

                    break;
            }
        }

        // *************************
        // *** Opciones de menú. ***
        // *************************

        private void moveToActivePackage(Element element, Repository repository)
        {
            Diagram diagram = repository.GetCurrentDiagram();

            element.PackageID = diagram.PackageID;

            element.Update();

            Package package = repository.GetPackageByID(element.PackageID);
            package.Elements.Refresh();
        }

        private void json2Class(Element element, Repository repository)
        {
            if( element.Notes != "")
            {
                Json2EAClass toEaClass = new Json2EAClass();

                toEaClass.go( element, this.eaUtils );
            }

        }
        private void generarEtiquetas(Element element, Repository repository)
        {
            foreach( EA.Attribute attr in element.Attributes)
            {
                if( attr.Alias == "")
                {
                    attr.Alias = StringUtils.toLabel(attr.Name);
                    attr.Update();
                }
            }
            Alert.Success("Se han generado las etiquetas");
        }

        /*
        private void generarPojo(Element element, Repository repository)
        {
            // obtener los "lenguajes"
            // recorrerlos para agregarlos a la ventana.
            SelectFromListForm selectForm = new SelectFromListForm();

            framework.Framework frameworkInstance = new framework.Framework(this.eaUtils);
            frameworkInstance.build();

            foreach (Language language in frameworkInstance.languages)
            {
                selectForm.addOption(language.name);
            }

            selectForm.ShowDialog();

            if (selectForm.getSelected() != null)
            {
                frameworkInstance.loadTemplatesByName(selectForm.getSelected());
            }

            // generar el código.
            PojoGenerator generator = new PojoGenerator();

            generator.doIt(element, frameworkInstance, eaUtils);
        }

        private void generarPojoMetadata(Element element, Repository repository)
        {
            // obtener los "lenguajes"
            // recorrerlos para agregarlos a la ventana.
            SelectFromListForm selectForm = new SelectFromListForm();

            framework.Framework frameworkInstance = new framework.Framework(this.eaUtils);
            frameworkInstance.build();

            foreach (Language language in frameworkInstance.languages)
            {
                selectForm.addOption(language.name);
            }

            selectForm.ShowDialog();

            if (selectForm.getSelected() != null)
            {
                frameworkInstance.loadTemplatesByName(selectForm.getSelected());
            }

            // generar el código.
            PojoMetadataGenerator generator = new PojoMetadataGenerator();

            generator.doIt(element, frameworkInstance, eaUtils);
            //generator.generate(element, frameworkInstance, eaUtils);


        }
        private void generarDao( Element element, Repository repository)
        {
            // obtener los "lenguajes"
            // recorrerlos para agregarlos a la ventana.
            SelectFromListForm selectForm = new SelectFromListForm();

            framework.Framework frameworkInstance = new framework.Framework( this.eaUtils );
            frameworkInstance.build();

            foreach (Language language in frameworkInstance.languages)
            {
                selectForm.addOption(language.name);
            }

            selectForm.ShowDialog();

            if( selectForm.getSelected() != null)
            {
                frameworkInstance.loadTemplatesByName(selectForm.getSelected());
            }

            // buscar los datos faltantes.

            // generar el código.
            JooqDaoGenerator generator = new JooqDaoGenerator();

            generator.doIt(element, frameworkInstance, eaUtils);
            //generator.generate(element, frameworkInstance, eaUtils);


        }
        */
        private void pasteEnumCode(Repository repository, string location)
        {
            Element enumElement = this.getEnumSelected(repository, location);

            BuilderManager builder = new BuilderManager(repository, enumElement);
            builder.go();
        }

        private void pasteClassCode(Repository repository, string location)
        {
            Element enumElement = this.getClassSelected(repository);

            BuilderManager builder = new BuilderManager(repository, enumElement);
            builder.go();
        }

        private void showViewAs(Repository repository, Boolean viewAsJson)
        {
            ObjectType ot = repository.GetContextItemType();
            Element rootElement = null;
            switch (ot)
            {
                case ObjectType.otElement:
                    rootElement = (Element)repository.GetContextObject();
                    break;
                case EA.ObjectType.otPackage:

                    break;

                case EA.ObjectType.otDiagram:
                    break;
                default:
                    break;
            }

            if (rootElement != null && rootElement.Type == "Class")
            {
                string json = jutils.toString(rootElement, StringUtils.NONE, true);

                if (viewAsJson)
                {
                    if (viewJson == null)
                    {
                        viewJson = (JsonXmlViewer)repository.AddWindow(JsonXmlViewer.JSON_WINDOW_NAME, "Productividad.JsonXmlViewer");
                        viewJson.Show();
                        repository.ShowAddinWindow(JsonXmlViewer.JSON_WINDOW_NAME);
                    }

                    viewJson.setJson(json, this);
                }
                else
                {
                    if (viewXml == null)
                    {
                        viewXml = (JsonXmlViewer)repository.AddWindow(JsonXmlViewer.XML_WINDOW_NAME, "Productividad.JsonXmlViewer");
                        viewXml.Show();
                        repository.ShowAddinWindow(JsonXmlViewer.XML_WINDOW_NAME);
                    }
                    try
                    {
                        viewXml.setXml(json, this);
                    }
                    catch (Exception e)
                    {
                        eaUtils.printOut(e.ToString());
                    }
                }
            }
        }

        public void simpleInstantiation(Repository repository)
        {
            Connector connector = this.getConnectorInstantiationSelected(repository);
            Element origen = repository.GetElementByID(connector.ClientID);
            Element destino = repository.GetElementByID(connector.SupplierID);
            origen.ClassifierID = destino.ElementID;
            origen.Update();
        }

        public string jsonViewerRefreshCallback(bool forceReload, short stringCase)
        {
            string json = "";
            try
            {
                Element rootElement = (Element)this.eaUtils.repository.GetContextObject();

                if (rootElement != null && rootElement.Type == "Class")
                {
                    json = jutils.toString(rootElement, stringCase, forceReload);
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
            return json;
        }

        // Suscripcion a eventos 

        public Boolean EA_OnPostNewDiagramObject(EA.Repository repository, EA.EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                try
                {
                    Element elemento = repository.GetElementByID(int.Parse((string)info.Get("ID").Value));
                    Diagram diagrama = repository.GetDiagramByID(int.Parse((string)info.Get("DiagramID").Value));
                    DiagramObject objetoDelDiagrama = diagrama.GetDiagramObjectByID(elemento.ElementID, (string)info.Get("DUID").Value);

                    if( elemento.Stereotype == "Module")
                    {
                        eaUtils = new EAUtils.EAUtils();
                        eaUtils.setRepositorio(repository);

                        Element systemElement;
                        Connector connector;
                        foreach( DiagramObject diagramObjet in diagrama.DiagramObjects)
                        {
                            systemElement = eaUtils.repository.GetElementByID(diagramObjet.ElementID);

                            if( systemElement.Stereotype == "System")
                            {
                                connector = eaUtils.connectorUtils.addConnector(elemento, systemElement, EAUtils.ConnectorUtils.CONNECTOR__AGGREGATION, null, null);
                                connector.Update();
                                break;
                            }
                        }
                    }
                    //MessageBox.Show("EA_OnPostNewDiagramObject - Conectores del elemento: " + elemento.Connectors.Count);
                }
                catch (Exception e) { Clipboard.SetText(e.ToString()); }
            }
            return changed;
        }

        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Diagram diagrama = repository.GetCurrentDiagram();

                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                if (diagrama.MetaType.Contains("APIRestResumen"))
                {
                    if (connector.Type == "Dependency")
                    {
                        Element origen = repository.GetElementByID(connector.ClientID);
                        Element destino = repository.GetElementByID(connector.SupplierID);

                        //MessageBox.Show("Productividad post connector create - origen " + origen.Name +" destino "+ destino.Name);
                    }
                }

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__INSTANTIATION)
                {
                    Element origen = repository.GetElementByID(connector.ClientID);
                    Element destino = repository.GetElementByID(connector.SupplierID);
                    origen.ClassifierID = destino.ElementID;
                    if (origen.Type.Contains("Interface"))
                    {
                        origen.Name = destino.Name;
                    }
                    origen.Update();
                }

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__REALISATION)
                {
                    Element origen = repository.GetElementByID(connector.ClientID);
                    Element destino = repository.GetElementByID(connector.SupplierID);

                    if (origen.Type == "Component")
                    {
                        if (origen.Stereotype == "Functional Unit" && (destino.Type == "UseCase" || destino.Type == "Activity"))
                        {
                            origen.Name = destino.Name;
                            origen.Update();
                        }
                    }
                }
            }
            return changed;
        }

        public bool EA_OnPreDeleteConnector(EA.Repository repository, EventProperties info)
        {
            Boolean canDelete = true;

            Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

            if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__INSTANTIATION)
            {
                Element origen = repository.GetElementByID(connector.ClientID);
                origen.ClassifierID = 0;
                origen.Update();
                if (origen.ParentID != 0)
                {
                    repository.GetElementByID(origen.ParentID).Elements.Refresh();
                }
                if (origen.PackageID != 0 && origen.PackageID != origen.ParentID)
                {
                    repository.GetElementByID(origen.PackageID).Elements.Refresh();
                }
            }

            return canDelete;
        }
        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info)
        {
            Boolean doIt = true;

            if (repository != null && info != null)
            {
                Element element = null;
                EA.Attribute attribute = null;

                try
                {
                    string type = info.Get("Type").Value;

                    if (type == "23")
                    {
                        attribute = repository.GetAttributeByID(info.Get("ID").Value);
                    }

                    else if (type == "4" )
                    {
                        element = repository.GetElementByID(info.Get("ID").Value);

                    }
                    Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);
                    Element dropped = null;

                    try
                    {
                        // lo que arrastro y suelto desde el epxplorador, pueder elemento, atributo...
                        dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                    }
                    catch (Exception) { }

                    if ( dropped != null )
                    {
                        if( dropped.Type == "Class" )
                        {
                            if( attribute != null)
                            {
                                ColumnOrAttribute2AttributeOrColumn x2x = new ColumnOrAttribute2AttributeOrColumn(repository, dropped);

                                x2x.toColumnOrAttribute(attribute);

                                doIt = false;
                            }
                            else if( element != null )
                            {
                                if( ( element.Type == "Class" && element.Stereotype == "DomainClass" ) || (element.Type == "Interface" && element.Stereotype == "DomainInterface"))
                                {
                                    TableOrClass2ClassOrTable x2x = new TableOrClass2ClassOrTable(repository, dropped);
                                    x2x.toTableOrClass(element);

                                    /*
                                    Class2Table class2Table = new Class2Table(repository, dropped);
                                    class2Table.toTable(element);
                                    */

                                    doIt = false;
                                }
                                if (element.Type == "Class" && element.Stereotype == "table")
                                {
                                    TableOrClass2ClassOrTable x2x = new TableOrClass2ClassOrTable(repository, dropped);
                                    x2x.toTableOrClass(element);

                                    /*
                                    Class2Table class2Table = new Class2Table(repository, dropped);
                                    class2Table.toTable(element);
                                    */

                                    doIt = false;
                                }
                                else if ( element.Type == "Enumeration")
                                {
                                    Enumeration2Table enum2Table = new Enumeration2Table(repository, dropped);
                                    enum2Table.toTable(element);
                                    doIt = false;
                                }
                            }
                            else
                            {
                                // suelto un elemento en un diagrama
                            }
                        }
                    }
                }
                catch (Exception e) { Clipboard.SetText(e.ToString()); }
            }
                return doIt;
        }
    }
}