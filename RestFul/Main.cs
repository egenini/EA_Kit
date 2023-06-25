using EA;
using RestFul.resumen;
using RestFul.ui;
using System;
using System.Windows.Forms;
using UIResources;

namespace RestFul

{
    /// <summary>
    /// Summary description for EAUtils.
    /// </summary>
    public class Main : MainCommons
    {

		public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
		{
            /*
            case ITEM_MENU__EDITOR:

                showViewEditor(Repository);

                break;
            */
            /*
            case ITEM_MENU_SWAGGER_MENU__VER:

                viewSwagger(Repository);
                break;
            */

            if (ItemName == ITEM_MENU__GENERAR_ABM)
            {

                generarABM(Repository);
            }
            else if (ItemName == ITEM_MENU__GENERAR_DESDE_RESUMEN)
            {
                generarDesdeResumen(Repository);
            }
            else if (ItemName == ITEM_MENU__MAPPING)
            {
                mapAttributes(Repository);
            }
            else if (ItemName == ITEM_MENU_SWAGGER_MENU__INVERSA)
            {
                inversaSwagger(Repository);
            }
            else if (ItemName == ITEM_MENU_SWAGGER_MENU__GENERAR)
            {
                generarSwagger(Repository);
            }
            else if (ItemName == ITEM_MENU_SWAGGER_EDITOR)
            {
                showViewEditor(Repository);
            }
            else if (ItemName == ITEM_MENU__GENERAR_TBF)
            {
                generarTbf(Repository, Location);
            }
        }

        public void generarTbf(Repository repository, String location)
        {
            Element element = this.resourceSelected(repository, location);

            try
            {
                tbf.Framework frameworkInstance = new tbf.Framework(this.eaUtils);

                if (frameworkInstance.choose())
                {

                    tbf.Generator generator = new tbf.Generator();

                    generator.build(element, frameworkInstance, this.eaUtils);
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error pegado al portapapeles");
            }
        }

        public void generarDesdeResumen(Repository repository)
        {
            Package package = repository.GetTreeSelectedPackage();
            GenerarDesdeResumen generador = new GenerarDesdeResumen(repository);
            generador.generar(package);
        }

        /// <summary>
        /// Abre la ventana de edición con los datos básicos del servicio.
        /// </summary>
        /// <param name="element">El componente que representa al servicio</param>
        public void showServiceEditor(Element element)
        {
            if (element != null)
            {
                if (this.swaggerPrincipal == null)
                {
                    this.swaggerPrincipal = (SwaggerPrincipal)eaUtils.repository.AddWindow(SwaggerPrincipal.NAME, "RestFul.ui.SwaggerPrincipal");
                }

                this.swaggerPrincipal.show(element, this.eaUtils, this);
                eaUtils.repository.ShowAddinWindow(SwaggerPrincipal.NAME);
            }

        }
        public void showViewEditor(Repository repository)
        {
            SwaggerMaster swaggerMaster = new SwaggerMaster(eaUtils);
            modelo.Swagger swagger = swaggerMaster.readFromEA(repository.GetTreeSelectedPackage());

            if (eaUtils.isTabClose(ViewSwagger.NAME))
            {

                editor = (SwaggerEditor)this.eaUtils.addTab(SwaggerEditor.NAME, "RestFul.SwaggerEditor");

                editor.setSwagger(swagger);
            }
            else
            {
                this.eaUtils.activateTab(SwaggerEditor.NAME);
            }
            

            //editor.show();
        }

        // ******************************
        // *** Suscripcion a eventos. ***
        // ******************************
        public void EA_OnContextItemChanged( EA.Repository repository , String GUID, EA.ObjectType ot )
        {
            this.eaUtils.setRepositorio(repository);

            switch (ot)
            {
                case EA.ObjectType.otElement:
                    Element element = repository.GetContextObject();

                    try
                    {
                        if (element.Type == "Component" && ( element.Stereotype == "Servicio" || element.Stereotype == "Service") )
                        {
                            showServiceEditor(element);
                        }
                        else
                        {
                            if (this.swaggerPrincipal != null)
                            {
                                this.swaggerPrincipal.disable();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.eaUtils.printOut(e.ToString());
                        //Alert.Error(Properties.Resources.error_indeterminado);
                    }

                    break;

                case EA.ObjectType.otPackage:
                    break;

                case EA.ObjectType.otDiagram:
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

        public void viewSwagger(Repository repository)
        {
            /*
            EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();
            eaUtils.setRepositorio(repository);
            eaUtils.createOutput();
            */

            SwaggerMaster swaggerMaster = new SwaggerMaster(eaUtils);
            modelo.Swagger swagger = swaggerMaster.readFromEA(repository.GetTreeSelectedPackage());

            FormSwagger form = new FormSwagger();
            form.Show();
            form.setSwagger(swagger);
            /*
            ViewSwagger viewSwagger;

            if (eaUtils.isTabClose(ViewSwagger.NAME))
            {
                viewSwagger = (ViewSwagger)eaUtils.addTab(ViewSwagger.NAME, "RestFul.ViewSwagger");
                viewSwagger.Show();
                viewSwagger.setSwagger(swaggerMaster.swagger);
            }
            else
            {
                eaUtils.activateTab(ViewJSON.NAME);
            }
            */

        }
        public void mapAttributes(Repository repository )
        {
            EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();
            eaUtils.setRepositorio(repository);
            eaUtils.createOutput();
            Diagram current = repository.GetCurrentDiagram();

            eaUtils.mappingElements(current);

            repository.ReloadDiagram(current.DiagramID);

        }

        public void generarABM(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element rootElement = null;
            switch (ot)
            {
                case ObjectType.otElement:
                    rootElement = (Element)repository.GetContextObject();
                    break;
                default:
                    break;
            }

            if (rootElement != null && rootElement.Type == "Class")
            {
                EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();
                eaUtils.setRepositorio(repository);
                eaUtils.createOutput();

                RestFulABM abm = new RestFulABM(eaUtils, rootElement);
                abm.generar();
            }
        }

        public void generarSwagger(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element swaggerClass = null;
            switch (ot)
            {
                case ObjectType.otElement:
                    swaggerClass = (Element)repository.GetContextObject();
                    if (swaggerClass.Name == "Swagger")
                    {
                        EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();
                        eaUtils.setRepositorio(repository);
                        eaUtils.createOutput();
                        SwaggerMaster swaggerMaster = new SwaggerMaster(eaUtils);

                        modelo.Swagger swagger = swaggerMaster.readFromEA(repository.GetPackageByID(swaggerClass.PackageID ));
                        swaggerMaster.build();
                    }
                    else
                    {
                        Alert.Error(Properties.Resources.error_solo_desde_clase_swagger);
                    }
                    break;
                default:
                    Alert.Error(Properties.Resources.error_solo_desde_clase_swagger);
                    break;
            }

        }
        public void inversaSwagger(Repository repository)
        {

            ObjectType ot = repository.GetContextItemType();
            Element swaggerClass = null;
            switch (ot)
            {
                case ObjectType.otElement:
                    swaggerClass = (Element)repository.GetContextObject();
                    if (swaggerClass.Name == "Swagger")
                    {
                        EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();
                        eaUtils.setRepositorio(repository);
                        eaUtils.createOutput();
                        InversaSwagger inversaSwagger = new InversaSwagger(eaUtils);

                        try
                        {
                            inversaSwagger.go(swaggerClass);

                        }
                        catch ( Exception e)
                        {
                            eaUtils.printOut(e.ToString());
                            Alert.Error(e.ToString());
                        }
                    }
                    else
                    {
                        Alert.Error(Properties.Resources.error_solo_desde_clase_swagger);
                    }
                    break;
                default:
                    Alert.Error(Properties.Resources.error_solo_desde_clase_swagger);
                    break;
            }
        }
        
        private void makeResource( Element element, Diagram diagram)
        {
            Builder builder = new Builder(this.eaUtils);

            builder.makeResource(element, diagram);

        }

        /*
         * http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_getribboncategory.html
         */
        /*
        public string EA_GetRibbonCategory(Repository repository)
        {
            return "DISEÑAR";
        }
        */

        public Boolean EA_OnPostNewElement(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Element elemento = repository.GetElementByID(int.Parse((string)info.Get("ElementID").Value));

                //MessageBox.Show("EA_OnPostNewElement - Conectores del elemento: " + elemento.Connectors.Count);

            }
            return changed;
        }

        public Boolean EA_OnPostNewDiagramObject(EA.Repository repository, EA.EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                try
                {
                    Element       elemento           = repository.GetElementByID(int.Parse((string)info.Get("ID").Value));
                    Diagram       diagrama           = repository.GetDiagramByID(int.Parse((string)info.Get("DiagramID").Value));
                    DiagramObject objectoDelDiagrama = diagrama.GetDiagramObjectByID(elemento.ElementID, (string)info.Get("DUID").Value);

                    RestFulController controller = new RestFulController(repository);
                    changed = controller.OnPostNewDiagramObject(repository, objectoDelDiagrama, diagrama, elemento);
                    
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

                if(((string)info.Get("Stereotype").Value) == "APIRestConnector")
                {
                    ResumenValidador validador = new ResumenValidador(repository);
                    // este conector solo se puede usar desde uri a recursos o desde recursos a recurso o método.
                    add = validador.validarConector(int.Parse((string)info.Get("ClientID").Value), int.Parse((string)info.Get("SupplierID").Value));
                    // No se puede repetir el mismo método bajo el mismo recuros
                    
                    //validador.validarMetodo(elemento);
                }
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

                if (diagrama.MetaType.Contains("Definition") || diagrama.MetaType.Contains("Definicion") || diagrama.MetaType.Contains("Definición"))
                {
                    if( connector.Type == "Dependency")
                    {
                        Element origen = repository.GetElementByID(connector.ClientID);
                        Element destino = repository.GetElementByID(connector.SupplierID);

                        if( origen.Type == "Interface" )
                        {
                            if (destino.Stereotype == "ErrorMessage")
                            {

                            }
                        }
                        //MessageBox.Show("RestFul post connector create - origen " + origen.Name +" destino "+ destino.Name);
                    }
                }
                else if (diagrama.MetaType.Contains("Summary") || diagrama.MetaType.Contains("Resumen") )
                {
                    if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__REALIZATION )
                    {
                        Element origen  = repository.GetElementByID( connector.ClientID   );
                        Element destino = repository.GetElementByID( connector.SupplierID );

                        if (origen.Stereotype.Contains( "Resource" ) && origen.Name == "/{recurso...Id}")
                        {
                            if (destino.Stereotype == "DomainClass" || destino.Stereotype == "LogicalClass")
                            {
                                string pluralName = this.eaUtils.taggedValuesUtils.get(destino, "plural_name", "").asString();
                                if (pluralName == "")
                                {
                                    pluralName = destino.Name + "s";
                                }
                                string resourceName = EAUtils.StringUtils.toCamel(pluralName).Replace("_", "-");

                                origen.Name = resourceName;
                                origen.Update();
                            }
                        }
                    }
                }
            }
            return changed;
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

                    else if (type == "4")
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

                    if (dropped != null)
                    {
                        if (dropped.Stereotype == "DomainClass" || dropped.Stereotype == "LogicClass")
                        {
                            makeResource(dropped, diagram);
                            doIt = false;
                        }
                    }
                }
                catch (Exception e) { Clipboard.SetText(e.ToString()); }
            }
            return doIt;
        }

        // http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_onretrievemodeltemplate.html

        public string EA_OnRetrieveModelTemplate(Repository repository, string location)
        {
            string template = "";

            MessageBox.Show("Plantilla solicitada " + location );

            return template;
        }

        public void EA_Disconnect()  
		{
			GC.Collect();  
			GC.WaitForPendingFinalizers();   
		}

	}
}
