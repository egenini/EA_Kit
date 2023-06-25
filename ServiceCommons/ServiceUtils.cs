using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCommons
{
    public class ServiceUtils
    {
        private EAUtils.EAUtils eaUtils;

        public ServiceUtils(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public Element addProvidedInterface( Element interfaz, string providedInterfaceStereotype, Diagram diagrama )
        {
            Element providedInterface = null;

            // busco el servicio en el mismo paquete donde se crea la interfaz.
            Package package = eaUtils.repository.GetPackageByID(interfaz.PackageID);
            Element servicio = null;

            foreach(Element child in package.Elements)
            {
                if( child.Stereotype == "Service" || child.Stereotype == "Servicio")
                {
                    servicio = child;
                    break;
                }
            }

            if( servicio != null )
            {
                // agregamos interfaz
                providedInterface = createProvidedInterface(servicio, interfaz, providedInterfaceStereotype, diagrama);
            }

            return providedInterface;
        }

        public Element createService(string name, Package package)
        {
            Element element = eaUtils.elementUtils.add(package, name, "Component", "Service", "Services");

            return element;
        }
        public Element createMessage(string name, string stereotype, Package package)
        {
            Element element = eaUtils.elementUtils.add(package, name, "Class", "RequestResponseMessage", "Services");

            return element;
        }

        public Element createInterface(string name, Boolean isIDResource, Element message, Package package)
        {
            name = name + (isIDResource ? "Id" : "");

            Element element = eaUtils.elementUtils.add( package, name, "Interface", null, null);

            return element;
        }

        public Element createProvidedInterface(Element service, Element interfaz, string stereotype, Diagram diagrama)
        {
            Element providedInterface = null;
            Boolean doCreate = true;

            foreach (Element currentElement in service.EmbeddedElements)
            {
                if( currentElement.ClassifierID == interfaz.ElementID )
                {
                    doCreate = false;
                    break;
                }
            }
            if (doCreate)
            {
                providedInterface = (Element)service.EmbeddedElements.AddNew(interfaz.Name, "ProvidedInterface");
                providedInterface.Stereotype = stereotype;
                providedInterface.ClassifierID = interfaz.ElementID;
                providedInterface.Update();

                eaUtils.connectorUtils.settingConnectorsForInstanceElement(providedInterface, interfaz, diagrama);
            }
            return providedInterface;
        }

        /// <summary>
        /// Fijate que onda: Determina si debe realizar cambios al elemento.
        /// </summary>
        /// <param name="eaUtils"></param>
        /// <param name="element"></param>
        /// <param name="diagram"></param>
        /// <returns></returns>
        public Boolean watchWhatWave(EAUtils.EAUtils eaUtils, Element element, Diagram diagram)
        {
            AttributeMessage attributeMessage = new AttributeMessage(eaUtils, element, diagram);
            return attributeMessage.go();
        }
    }
}
