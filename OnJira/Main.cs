using EA;
using EAUtils;
using RestSharp;
using System;
using System.Windows.Forms;
using UIResources;

namespace OnJira

{
    /// <summary>
    /// Responsable de implementar el comportamiento particular del addin, normalmente delegándolo.
    /// </summary>
    public class Main : MainCommons
    {

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
                case TEST_LOGIN:

                    login(repository);

                    break;
                /*
                case ITEM_MENU__ETIQUETAS:

                    generarEtiquetas(this.getClassSelected(repository), repository);

                    break;
                */
            }
        }

        // *************************
        // *** Opciones de menú. ***
        // *************************
        
        private void login(Repository repository)
        {
            // obtener los "lenguajes"

            // en la cabecera debe ir "Authorization : basic (username:password) codificado en base64

            string userPass = "egenini:egenini";
            var encodedUserPass = Base64.encodeBase64(System.Text.Encoding.UTF8, userPass);

            var client = new RestClient("http://192.168.2.25:9080/");

            var request = new RestRequest("proyectos/", RestSharp.Method.POST);

            // easily add HTTP Headers
            request.AddHeader("Authorization", encodedUserPass);


            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content;


        }

        
        // Suscripcion a eventos 
        public void EA_OnContextItemChanged(EA.Repository repository, String GUID, EA.ObjectType ot)
        {
            switch (ot)
            {
                case EA.ObjectType.otElement:
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
                    Element elemento = repository.GetElementByID(int.Parse((string)info.Get("ID").Value));
                    Diagram diagrama = repository.GetDiagramByID(int.Parse((string)info.Get("DiagramID").Value));
                    DiagramObject objetoDelDiagrama = diagrama.GetDiagramObjectByID(elemento.ElementID, (string)info.Get("DUID").Value);

                    //MessageBox.Show("EA_OnPostNewDiagramObject - Conectores del elemento: " + elemento.Connectors.Count);
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
                //Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

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

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION)
                {
                    Element origen = repository.GetElementByID(connector.ClientID);
                    Element destino = repository.GetElementByID(connector.SupplierID);

                    if (origen.Stereotype == "table" && destino.Stereotype == "table")
                    {
                        // si el origen no tiene una columna con el nombre de la tabla más el _id se lo agregamos
                        bool addColumn = true;
                        string fkColumnName = destino.Name + "_id";

                        foreach (EA.Attribute columna in origen.Attributes)
                        {
                            if (columna.Name == fkColumnName)
                            {
                                addColumn = false;
                                break;
                            }
                        }

                        if (addColumn)
                        {
                            EA.Attribute fkColumn = origen.Attributes.AddNew(fkColumnName, "integer");
                            fkColumn.Stereotype = "column";
                            fkColumn.Update();

                        }
                    }
                    //MessageBox.Show("Productividad post connector create - origen " + origen.Name +" destino "+ destino.Name);
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
                        dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                    }
                    catch (Exception) { }

                    if ( dropped != null )
                    {
                        
                        
                    }

                }
                catch (Exception e) { Clipboard.SetText(e.ToString()); }

            }

                return doIt;
        }
    }
}