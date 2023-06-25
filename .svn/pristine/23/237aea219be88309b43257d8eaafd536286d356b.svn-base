using EA;
using System;
using System.Windows.Forms;
using UIResources;

namespace TrazabilidadDetallada
{
    /// <summary>
    /// Summary description for EAUtils.
    /// </summary>
    public class Main : MainCommons
	{

		public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
		{
            if (itemName == Properties.Resources.ITEM_MENU__SINCRONIZAR)
            {
                sincronizar(repository, location);
            }
            else if (itemName == Properties.Resources.ITEM_MENU__MAPEAR)
            { 
                    mapear(repository, location);
            }
        }

        private void mapear(Repository repository, string location)
        {
            Diagram diagram = this.getMapeable(repository, location);
            EAUtils.EAUtils eaUtils = new EAUtils.EAUtils(repository);
            Mapper mapper = new Mapper(eaUtils, diagram);
            mapper.go();
        }

        private void sincronizar(Repository repository, string location)
        {
            EAUtils.EAUtils eaUtils = new EAUtils.EAUtils(repository);
            Utilities utils = new Utilities( eaUtils );

            eaUtils.createOutput();

            if ( location == "TreeView")
            {
                EA.ObjectType treeSelectedType = repository.GetTreeSelectedItemType();

                switch (treeSelectedType)
                {
                    case EA.ObjectType.otPackage:
                    {
                        Package package = repository.GetTreeSelectedPackage();

                        try
                        {
                            if (utils.existeRepositorio(package))
                            {
                                utils.go(package);
                                Alert.Success(Properties.Resources.sincronizacion_finalizada);
                            }
                            else
                            {
                                // establecer el paquete que será el repositorio
                            }
                        }
                        catch ( Exception e)
                        {
                            eaUtils.printOut(e.ToString());
                        }
                        break;
                    }
                    case EA.ObjectType.otElement:
                    {
                        object selectedObject = repository.GetTreeSelectedObject();
                        Element element = (Element)selectedObject;

                        //int elementid = eaUtils.repository.InvokeConstructPicker("IncludedTypes=Package;");
                            //Element elementOf = eaUtils.repository.GetElementByID(elementid);
                            //Package package = eaUtils.repository.GetPackageByID(elementOf.PackageID);
                            //package.IsNamespace = false;
                            //package.Update();


                            if (utils.existeRepositorio(element))
                        {
                            try
                            { 
                                utils.go(element);
                                Alert.Success(Properties.Resources.sincronizacion_finalizada);
                            }
                            catch (Exception e)
                            {
                                eaUtils.printOut(e.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.establecer_repositorio);

                            int elementid = eaUtils.repository.InvokeConstructPicker("IncludedTypes=Package;");
                            if( elementid != 0)
                            {
                                    Element elementOf = eaUtils.repository.GetElementByID(elementid);
                                    Package package = eaUtils.repository.GetPackageByGuid(elementOf.ElementGUID);
                                    package.IsNamespace = true;
                                    package.Update();
                                    
                                    if (utils.existeRepositorio(element))
                                    {
                                        try
                                        {
                                            utils.go(element);
                                            Alert.Success(Properties.Resources.sincronizacion_finalizada);
                                        }
                                        catch (Exception e)
                                        {
                                            eaUtils.printOut(e.ToString());
                                        }
                                    }
                                }
                            
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            else if( location == "Diagram")
            {
                Diagram currentDiagram = repository.GetCurrentDiagram();
                Collection selectedObjects = currentDiagram.SelectedObjects;
                if (selectedObjects.Count > 0)
                {
                    DiagramObject diagramObjectCheck = (DiagramObject)selectedObjects.GetAt(0);

                    if (utils.existeRepositorio(repository.GetElementByID(diagramObjectCheck.ElementID)))
                    { 
                        foreach (DiagramObject diagramObject in selectedObjects)
                        {
                            utils.go(repository.GetElementByID(diagramObject.ElementID));
                        }
                        Alert.Success(Properties.Resources.sincronizacion_finalizada);
                    }
                }
            }
        }

        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info)
        {
            Boolean doIt = true;

            if (repository != null && info != null)
            {
                EAUtils.EAUtils eaUtils = new EAUtils.EAUtils(repository);
                eaUtils.createOutput();

                try
                {
                    string type = info.Get("Type").Value;

                    if (type == "4" )
                    {
                        if (this.getDropped(repository, info) == null)
                        {
                            Element element = repository.GetElementByID(info.Get("ID").Value);
                            Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);

                            if (element != null && this.mapper.elegible(element) && this.getMapeable(diagram) != null )
                            {
                                Mapper mapper = new Mapper(eaUtils, diagram);
                                mapper.go(element);
                            }
                        }
                        //MessageBox.Show("EA_OnPostNewDiagramObject - Conectores del elemento: " + elemento.Connectors.Count);
                    }
                }
                catch (Exception e)
                {
                    Alert.Error(Properties.Resources.error_indeterminado);
                    eaUtils.printOut(e.ToString());
                }
            }
            return doIt;
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

        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Diagram diagrama = repository.GetCurrentDiagram();

                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));
            }
            return changed;
        }
	}
}
