using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table
{
    public class Main : MainCommons
    {
        private const string MODEL_PROP_NAME = "Model => Table";
        
        private string lastNameModel = null;

        private EA.PropertiesTab modelPropertiesTab = null;

        private void showProps()
        {

        }
        /*
        public bool EA_OnPostNewElement(Repository repository, EventProperties info)
        {
            if (leDoyBola(repository))
            {
                this.sincro.enModelCreado(repository.GetElementByID(int.Parse((string)info.Get("ElementID").Value)));
            }

            return true;
        }

        public bool EA_OnPostNewAttribute(Repository repository, EventProperties info)
        {
            if (leDoyBola(repository))
            {
                EA.Attribute a = repository.GetAttributeByID( int.Parse( (string)info.Get( "AttributeID" ).Value ) );

                new Sincro(eaUtils, this.framework).enNuevoAtributo(a);
            }

            return true;
        }
        */
        public bool EA_OnPreDeleteElement(Repository repository, EventProperties info)
        {
            if (leDoyBola(repository))
            {
                EA.Element e = repository.GetElementByID(int.Parse((string)info.Get("ElementID").Value));

                this.sincro.framework = this.frameworkManager.diagramaFramework[repository.GetCurrentDiagram().DiagramID];
                this.sincro.enModelBorrado(e);
            }

            return true;
        }
        public bool EA_OnPreDeleteAttribute(Repository repository, EventProperties info)
        {

            if (leDoyBola(repository))
            {
                EA.Attribute a = repository.GetAttributeByID(int.Parse((string)info.Get("AttributeID").Value));

                this.sincro.framework = this.frameworkManager.diagramaFramework[repository.GetCurrentDiagram().DiagramID];
                this.sincro.enBorrarAtributo(a);
            }

            return true;
        }
        public void EA_OnContextItemChanged(Repository repository, string guid, EA.ObjectType o)
        {            
            // Llamado cuando un elemento se ha modificado.
            if (leDoyBola(repository))
            {
                switch (o)
                {
                    case ObjectType.otAttribute:
                        EA.Attribute a = repository.GetAttributeByGuid(guid);
                        // presentar ventana de propiedaes.
                        if( a.Type != "")
                        {
                            this.sincro.framework = this.frameworkManager.diagramaFramework[repository.GetCurrentDiagram().DiagramID];
                            this.sincro.enNuevoAtributo(a);
                        }
                        break;
                    case ObjectType.otElement:
                        EA.Element e = repository.GetElementByGuid(guid);
                        
                        lastNameModel = e.Name;

                        // presentar ventana de propiedades.

                        break;
                }
            }
        }
        public void EA_OnNotifyContextItemModified(Repository repository, string guid, EA.ObjectType o)
        {
            if (leDoyBola(repository))
            {
                switch(o)
                {
                    case ObjectType.otAttribute:
                        EA.Attribute a = repository.GetAttributeByGuid(guid);
                        // presentar ventana de propiedaes.
                        this.sincro.framework = this.frameworkManager.diagramaFramework[repository.GetCurrentDiagram().DiagramID];
                        sincro.enAtributoModificado(a);
                        break;
                    case ObjectType.otElement:
                        EA.Element e = repository.GetElementByGuid(guid);
                        this.sincro.framework = this.frameworkManager.diagramaFramework[repository.GetCurrentDiagram().DiagramID];
                        sincro.enModelModificado(e, lastNameModel);
                        lastNameModel= e.Name;
                        break;
                }
            }
        }
        public Boolean EA_OnPostNewConnector(Repository repository, EventProperties info)
        {
            Boolean changed = false;

            if (leDoyBola(repository))
            {
                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION)
                {
                    // crear fk
                }
            }
            return changed;
        }
        public bool EA_OnPreDeleteConnector(EA.Repository repository, EventProperties info)
        {
            Boolean canDelete = true;

            EA.Diagram d = repository.GetCurrentDiagram();

            if (leDoyBola(repository))
            {
                Connector connector = repository.GetConnectorByID(int.Parse((string)info.Get("ConnectorID").Value));

                if (connector.Type == EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION)
                {
                    // eliminar fk
                }
            }

            return canDelete;
        }

        private bool leDoyBola(Repository repository)
        {
            bool doyBola = false;

            try
            {
                EA.Diagram d = repository.GetCurrentDiagram();

                if( this.eaUtils == null)
                {
                    buildEAUtils(repository);

                    this.eaUtils.printOut("Nueva instancia eautils");
                }

                doyBola = frameworkManager.isSourceDiagram(d);
            }
            catch(Exception) { }

            return doyBola;
        }
    }
}
