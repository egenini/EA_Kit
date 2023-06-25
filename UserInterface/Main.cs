using EA;
using EAUtils;
using EAUtils.framework2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;
using UserInterface.frw;
using static EAUtils.ConnectorUtils;

namespace UserInterface
{
    public class Main : MainCommons
    {
        public void EA_MenuClick(Repository repository, string location, string menuName, string itemName)
        {
            switch (itemName)
            {
                case ITEM_MENU__GENERAR_UI:

                    try
                    {
                        generarUI(repository);
                    }
                    catch (Exception e)
                    {
                        Clipboard.SetText(e.ToString());
                    }
                    break;

                case ITEM_MENU__GENERAR_JSON:

                    try
                    {
                        generarJson(repository);
                    }
                    catch (Exception e)
                    {
                        Clipboard.SetText(e.ToString());
                    }
                    break;

                case ITEM_MENU__GENERAR_ARTEFACTO:

                    try
                    {
                        generarArtefacto(repository);
                    }
                    catch (Exception e)
                    {
                        Clipboard.SetText(e.ToString());
                    }
                    break;
            }
        }

        // *************************
        // *** Opciones de menú. ***
        // *************************
        private void generarJson(Repository repository)
        {
            this.generar(repository, true, false);
        }

        private void generarArtefacto(Repository repository)
        {
            this.generar(repository, false, true);
        }

        private void generarUI(Repository repository)
        {
            this.generar(repository, true, true);
        }

        private void generar(Repository repository, bool json, bool artefacto)
        {
            Element element = this.getElementSelected(repository);

            eaUtils.setRepositorio(repository);

            try
            {
                frw.Framework frameworkInstance = new frw.Framework(this.eaUtils);

                if (frameworkInstance.choose( element ))
                {

                    frw.Generator generator = new frw.Generator(element, frameworkInstance, this.eaUtils);

                    if (json)
                    {
                        generator.generarJSon();

                        Alert.Success("Se ha generado el JSON");
                    }

                    if (artefacto)
                    {
                        generator.generarArtefacto();

                        Alert.Success("Se ha/n generado el/los artefactos");
                    }
                }
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error pegado al portapapeles");
            }
        }

        public void EA_OnElementTagEdit(EA.Repository repository, int objectId, String TagName, String TagValue, String TagNotes)
        {
            if( repository != null)
            {
                this.buildEAUtils(repository);

                Element element = repository.GetElementByID(objectId);

                List < ElementConnectorInfo > infos = this.eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, null, null, "Screen", false, null);

                if( infos.Count != 0)
                {
                    Package package = this.eaUtils.repository.GetPackageByID(infos[0].element.PackageID);

                    try
                    {
                        Diagram diagram = package.Diagrams.GetAt(0);

                        this.eaUtils.repository.OpenDiagram(diagram.DiagramID);
                        //this.eaUtils.repository.ActivateDiagram(  diagram.DiagramID );
                    }
                    catch (Exception) { }
                }
            }
        }
        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info)
        {
            Boolean doIt = true;

            if (repository != null && info != null)
            {
                Element element = null;
                EA.Attribute attribute = null;
                Package package = null;

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
                    else if (type == "5")
                    {
                        package = repository.GetPackageByID(info.Get("ID").Value);
                        if (package.Name.ToLower().StartsWith("template") || package.Element.Stereotype != "Dialect")
                        {
                            package = null;
                        }
                    }

                    Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);
                    Element dropped = null;

                    try
                    {
                        dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                    }
                    catch (Exception) { }

                    if (dropped != null)
                    {
                        this.buildEAUtils(repository);

                        OnPreDropFromTree onpredrop = new OnPreDropFromTree(dropped, element, package);
                        Framework frameworkInstance = new Framework(this.eaUtils);

                        doIt = onpredrop.EA_OnPreDropFromTree(repository, frameworkInstance, "Screen");
                    }

                }
                catch (Exception e)
                {
                    Clipboard.SetText(e.ToString());
                }
            }

            return doIt;
        }

        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (repository != null && info != null)
            {
                Diagram diagrama = repository.GetCurrentDiagram();

                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                if (diagrama.MetaType.Contains("UI DG"))
                {
                    if (connector.Stereotype == "bind")
                    {
                        Element origen = repository.GetElementByID(connector.ClientID);
                        Element destino = repository.GetElementByID(connector.SupplierID);

                        if( origen.Stereotype == "Table" && destino.Stereotype == "custom table")
                        {
                            string data = this.eaUtils.taggedValuesUtils.get(destino, "data", "").asString();

                            if( data == "")
                            {


                            }
                        }
                    }
                }
            }
            return changed;
        }
    }
}
