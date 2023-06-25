using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIResources;

namespace RestFul.ea
{
    class ServiceModel
    {
        public const string INFO__TITLE            = "title";
        public const string INFO__DESCRIPTION      = "description";
        public const string INFO__TERMS_OF_SERVICE = "termsOfService";
        public const string INFO__VERSION          = "version";

        public const string CONTACT__NAME  = "name";
        public const string CONTACT__URL   = "url";
        public const string CONTACT__EMAIL = "email";

        public const string LICENSE__NAME = "name";
        public const string LICENSE__URL  = "url";

        public string[] SWAGGER__INFO = { INFO__TITLE, INFO__DESCRIPTION, INFO__TERMS_OF_SERVICE, INFO__VERSION };

        public const string URI__ATTRIBUTE_SCHEMA    = "schema";
        public const string URI__ATTRIBUTE_HOST      = "host";
        public const string URI__ATTRIBUTE_BASE_PATH = "basePath";

        public EAUtils.EAUtils eaUtils;

        public Package rootPackage;
        public Package definitionPackage;
        public Package summaryPackage;

        public Element swagger = null;
        public Element info    = null;
        public Element contact = null;
        public Element license = null;
        public Element service = null;

        public List<Element>        uris       = null;
        public List<ModelInterface> interfaces = null;

        public ServiceModel()
        {
            interfaces = new List<ModelInterface>();
            uris = new List<Element>();
            //eaUtils.repository.EnableUIUpdates = false;
        }

        public void findRootPackage()
        {
            rootPackage = eaUtils.repository.GetPackageByID(swagger.PackageID);
        }

        public void findDefinitionPackage()
        {
            this.definitionPackage = eaUtils.packageUtils.getChildPackageByName(this.rootPackage, "Definición");
            if (this.definitionPackage == null)
            {
                this.definitionPackage = eaUtils.packageUtils.getChildPackageByName(this.rootPackage, "Definition");
                if (this.definitionPackage == null)
                {
                    this.definitionPackage = eaUtils.packageUtils.getChildPackageByName(this.rootPackage, "Definicion");
                }
                else
                {
                    // abrir ventana para que se busque el paquete donde está el servicio.
                }
            }
        }

        public void findSummaryPackage()
        {
            foreach (Package currentPackage in rootPackage.Packages)
            {
                if (currentPackage.Name == "Resumen" || currentPackage.Name == "Summary")
                {
                    summaryPackage = currentPackage;
                    break;
                }
            }
        }

        public void findService()
        {
            this.service = eaUtils.packageUtils.getChildElementByStereotype(definitionPackage, "Servicio");
            if (this.service == null)
            {
                this.service = eaUtils.packageUtils.getChildElementByStereotype(definitionPackage, "Service");
            }
        }

        public void findUri()
        {
            findInterfaces();

            // la uri esta relacionada con todas las interfaces expuestas.
            foreach (ModelInterface modelInterface in this.interfaces)
            {
                List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, modelInterface.instance, null, null, null, "URI");
                if (elementsAndConnectors.Count != 0)
                {
                    foreach (ArrayList elementAndConnector in elementsAndConnectors)
                    {
                        this.uris.Add((Element)elementAndConnector[0]);
                    }
                    break;
                }
            }
        }

        public void findSwaggerClass()
        {
            foreach (Element e in rootPackage.Elements)
            {
                if (e.Name == "Swagger")
                {
                    swagger = e;

                    List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, swagger, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null);
                    Element elmentClass;

                    foreach (ArrayList elementAndConnector in elementsAndConnectors)
                    {
                        elmentClass = (Element)elementAndConnector[0];

                        if (elmentClass.Name == "Alert")
                        {
                            info = (Element)elementAndConnector[0];

                            List<ArrayList> elementsAndConnectorsOfInfo = eaUtils.connectorUtils.getFromConnectorFilter(false, elmentClass, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null);
                            foreach (ArrayList elementAndConnectorOfInfo in elementsAndConnectorsOfInfo)
                            {
                                elmentClass = (Element)elementAndConnectorOfInfo[0];
                                if (elmentClass.Name == "Contact")
                                {
                                    contact = (Element)elementAndConnectorOfInfo[0];
                                }
                                if (elmentClass.Name == "Licence")
                                {
                                    license = (Element)elementAndConnectorOfInfo[0];
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        public Element addClass(string className)
        {
            Element message = (Element)definitionPackage.Elements.AddNew(className, "Class");

            message.Update();

            return message;
        }

        public void findInterfaces()
        {
            if (this.interfaces.Count == 0)
            {
                foreach (Element instancia in service.EmbeddedElements)
                {
                    if (instancia.ClassifierID != 0)
                    {
                        this.interfaces.Add(new ModelInterface(eaUtils.repository.GetElementByID(instancia.ClassifierID), instancia));
                    }
                }
            }
        }

        public void updateUriName( Element uri )
        {
            string schema   = this.eaUtils.taggedValuesUtils.get(uri, URI__ATTRIBUTE_SCHEMA,"").asString();
            string host     = this.eaUtils.taggedValuesUtils.get(uri, URI__ATTRIBUTE_HOST, "").asString();
            string basePath = this.eaUtils.taggedValuesUtils.get(uri, URI__ATTRIBUTE_BASE_PATH,"").asString();
            
            /*
            string schema   = EAUtils.AttributeUtils.getValue(uri, URI__ATTRIBUTE_SCHEMA, "");
            string host     = EAUtils.AttributeUtils.getValue(uri, URI__ATTRIBUTE_HOST, "");
            string basePath = EAUtils.AttributeUtils.getValue(uri, URI__ATTRIBUTE_BASE_PATH, "");
            */

            uri.Name = composeUriName(schema, host, basePath);

            uri.Update();
        }

        public string composeUriName( string schema, string host, string basePath)
        {
            return schema + "://" + host + basePath;
        }

        public void addUri(string schema, string host, string basePath)
        {
            Element uri = definitionPackage.Elements.AddNew( this.composeUriName(schema, host, basePath ) , "Class");
            uri.Stereotype = "URI";
            uri.Update();

            this.eaUtils.taggedValuesUtils.set(uri, URI__ATTRIBUTE_SCHEMA, schema);
            this.eaUtils.taggedValuesUtils.set(uri, URI__ATTRIBUTE_HOST, host);
            this.eaUtils.taggedValuesUtils.set(uri, URI__ATTRIBUTE_BASE_PATH, basePath);

            /*
            EA.Attribute attr;
            attr = (EA.Attribute)uri.Attributes.AddNew(URI__ATTRIBUTE_SCHEMA, "String");
            attr.Default = schema;
            attr.Update();
            attr = (EA.Attribute)uri.Attributes.AddNew(URI__ATTRIBUTE_HOST, "String");
            attr.Default = host;
            attr.Update();
            attr = (EA.Attribute)uri.Attributes.AddNew(URI__ATTRIBUTE_BASE_PATH, "String");
            attr.Default = basePath;

            attr.Update();
            */

            synchronizeUriRelations(uri);

            this.uris.Add(uri);
        }

        public void synchronizeUriRelations(Element uri)
        {
            if( this.service != null )
            {
                foreach( Element providedInterface in service.EmbeddedElements)
                {
                    if(providedInterface.Type == "ProvidedInterface")
                    {
                        eaUtils.connectorUtils.addOnlyOneAssociation(providedInterface, uri,null,null, EAUtils.ConnectorUtils.DIRECTION__SOURCE_DESTINATION);
                    }
                }
            }
        }

        internal Element synchronizeInterface(string interfaceName, string path)
        {
            Element eaInterface = null;

            foreach (ModelInterface modelInterface in this.interfaces)
            {
                if (modelInterface.@interface.Alias == path)
                {
                    eaInterface = modelInterface.@interface;
                    break;
                }
            }

            if (eaInterface == null)
            {
                eaInterface = (Element)definitionPackage.Elements.AddNew(EAUtils.StringUtils.toPascal(interfaceName), "Interface");

                eaInterface.Alias = path;

                eaInterface.Update();

                this.interfaces.Add(new ModelInterface(eaInterface, null));
            }

            return eaInterface;
        }

        public void synchronizeService(string serviceName)
        {
            if (this.service == null)
            {
                this.service = eaUtils.packageUtils.getChildElementByStereotype(definitionPackage, "Service");

            }

            this.service = eaUtils.elementUtils.synchronize(definitionPackage, serviceName, "Component", "Service", "Services");
        }

        public void synchronizeSwaggerAttribute(string key, dynamic value)
        {
            EAUtils.AttributeUtils.synchronize(this.swagger, key, "string", value);
        }

        public void findInfo()
        {
            if (info == null)
            {
                foreach (Element infoCurrentElement in swagger.Elements)
                {
                    if (infoCurrentElement.Name == "Alert")
                    {
                        info = infoCurrentElement;
                        break;
                    }
                }
            }

            if (info == null)
            {
                info = (Element)swagger.Elements.AddNew("Alert", "Class");
                info.Update();

                this.eaUtils.connectorUtils.addConnectorAssociation(swagger, info, null, null, EAUtils.ConnectorUtils.DIRECTION__SOURCE_DESTINATION);
            }
        }

        public void synchronizeInfoAttribute(string key, dynamic value)
        {
            EAUtils.AttributeUtils.synchronize(this.info, key, "string", value);
        }

        internal void findContactAndLicence()
        {
            // contacto y licencia van por relación (pueden estar en cualquier parte y ser reusados)
            List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(false, info, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, "Class", null);
            Element currentElement;

            foreach (ArrayList elementAndConnector in elementsAndConnectors)
            {
                currentElement = (Element)elementAndConnector[0];

                if (currentElement.Name == "Contact")
                {
                    contact = currentElement;
                }
                if (currentElement.Name == "License")
                {
                    license = currentElement;
                }
            }

            if (contact == null)
            {
                contact = (Element)swagger.Elements.AddNew("Contact", "Class");
                contact.Update();

                eaUtils.connectorUtils.addConnectorAssociation(info, contact, null, null);
            }

            if (license == null)
            {
                license = (Element)swagger.Elements.AddNew("License", "Class");
                license.Update();

                eaUtils.connectorUtils.addConnectorAssociation(info, license, null, null);
            }
        }

        public void synchronizeContactAttribute(string attrName, dynamic value)
        {
            EAUtils.AttributeUtils.synchronize(contact, attrName, "string", value);
        }

        public void synchronizeLicenseAttribute(string attrName, dynamic value)
        {
            EAUtils.AttributeUtils.synchronize(contact, attrName, "string", value);
        }

        public string getBasePath()
        {
            string value = "";

            if (uris.Count != 0)
            {
                foreach (EA.Attribute atributo in uris[0].Attributes)
                {
                    if (atributo.Name == "basePath")
                    {
                        value = atributo.Default;
                        break;
                    }
                }
            }
            return value;
        }

        public void synchronizeHost(string value)
        {
            foreach (Element uri in uris)
            {
                EAUtils.AttributeUtils.synchronize(uri, "host", "string", value);
            }
            foreach (Element uri in uris)
            {
                this.updateUriName(uri);
            }
        }

        public void synchronizeBasePath(string value)
        {
            foreach (Element uri in uris)
            {
                EAUtils.AttributeUtils.synchronize(uri, "basePath", "string", value);
                this.updateUriName(uri);
            }
        }

        /// <summary>
        /// Un schema puede tomar sólo 4 valores (http, https, ws o wss), por cada valor se crea una URI
        /// Por lo tanto si uno de estos no es soportado hay que eliminar la URI.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="supportScheme"></param>
        public void synchronizeSchema(string schema, bool supportScheme, string host, string basePath )
        {
            Element      uri       = null;
            EA.Attribute attribute = null;

            foreach (Element currentUri in uris)
            {
                EA.Attribute attr = EAUtils.AttributeUtils.get(currentUri, URI__ATTRIBUTE_SCHEMA );

                if( attr.Default == schema)
                {
                    attribute = attr;
                    uri       = currentUri;
                    break;
                }

                //EAUtils.AttributeUtils.synchronize(currentUri, "basePath", "string", value);
            }
            if( uri == null && supportScheme )
            {
                this.addUri(schema, host, basePath);
                // todo: hacer las relaciones correspondientes.
            }
            else
            {
                if( uris.Count == 1)
                {
                    Alert.Error(Properties.Resources.error_al_menos_1_uri);
                }
                else
                {
                    if( eaUtils.elementUtils.delete(uri))
                    {
                        //Alert.Alert();
                    }
                }
            }

        }

        public string getHost()
        {
            string value = "";

            if (uris.Count != 0)
            {
                foreach (EA.Attribute atributo in uris[0].Attributes)
                {
                    if (atributo.Name == "host")
                    {
                        value = atributo.Default;
                        break;
                    }
                }
            }
            return value;
        }

        public List<string> getSchemes()
        {
            List<string> value = new List<string>();

            foreach (Element uri in uris)
            {
                foreach (EA.Attribute atributo in uri.Attributes)
                {
                    if (atributo.Name == URI__ATTRIBUTE_SCHEMA)
                    {
                        value.Add(atributo.Default);
                    }
                }
            }
            return value;
        }
    }

    public class ModelInterface
    {
        public Element @interface;
        public Element instance;

        public ModelInterface( Element @interface, Element instance)
        {
            this.@interface = @interface;
            this.instance   = instance;
        }
    }
}
