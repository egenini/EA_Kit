using EA;
using Newtonsoft.Json.Linq;
using RestFul.ea;
using RestFul.modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestFul
{

    class InversaSwagger
    {
        string[] SWAGGER__ROOT = { "swagger", "consumes", "produces" };

        EAUtils.EAUtils eaUtils;
        //Package rootPackage;
        //Package definicionPackage;
        //Package resumenPackage;

        ServiceModel serviceModel;

        dynamic json;
        //List<Element> uris = new List<Element>();
        Dictionary<string, Element> classes    = new Dictionary<string, Element>();
        Dictionary<string, Element> parameters = new Dictionary<string, Element>();
        Dictionary<string, Element> paths      = new Dictionary<string, Element>();
        List<InterfaceInfo>         interfaces = new List<InterfaceInfo>();
        Dictionary<string, Element> mediaTypes = new Dictionary<string, Element>();

        public InversaSwagger(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils      = eaUtils;
            this.serviceModel = new ServiceModel();

            this.serviceModel.eaUtils = eaUtils;
        }

        public void go(Element swaggerClass)
        {
            serviceModel.swagger = swaggerClass;

            if (swaggerClass.Notes.Length != 0)
            {
                json = Newtonsoft.Json.Linq.JObject.Parse(swaggerClass.Notes);

                serviceModel.findRootPackage();

                // establecemos los paquetes
                serviceModel.findDefinitionPackage();

                serviceModel.findSummaryPackage();

                this.generateUri();
                this.generateReferences();
                this.generateInterfaces();

                try
                {
                    this.generarServicio();
                }
                catch (Exception e)
                {
                    eaUtils.printOut(e.ToString());
                }

                try
                {
                    this.generateModel();
                }
                catch (Exception e)
                {
                    eaUtils.printOut(e.ToString());
                }

                try
                {
                    SwaggerResumen resumen = new SwaggerResumen(this.eaUtils, serviceModel.summaryPackage, serviceModel.definitionPackage);
                    resumen.makeSummary();
                }
                catch (Exception e)
                {
                    eaUtils.printOut(e.ToString());
                }
            }
        }

        private void generateUri()
        {
            foreach (dynamic scheme in json.schemes)
            {
                string schemeValue = scheme.Value;
                string hostValue = json.host;
                string basePathValue = json.basePath;

                serviceModel.addUri(schemeValue, hostValue, basePathValue);
            }
        }

        private void generateReferences()
        {
            // Esto puede contener una variedad de cosas tales como definiciones de:
            // Parámetros, la forma de reconocerlo es si existe un atributo "in"
            // Paths, la forma de reconocerlo es si el nombre (Name) comienza por una barra /
            // y Clases, la forma de reconocerla es por descarte de las anteriores.
            foreach (dynamic definition in json.definitions)
            {
                if (definition.Name.Contains("/"))
                {
                    // es path
                    eaUtils.printOut(definition.Name + " es un path");
                }
                else
                {
                    try
                    {
                        if (definition.@in == "query")
                        {
                            // es un parámetro
                            eaUtils.printOut( definition.@in +" es un (in) parámetro");
                        }
                        else
                        {
                            // es una clase
                            eaUtils.printOut(definition.@in + " es una (in) clase");
                            generateClassReference(definition.Name);
                        }
                    }
                    catch (Exception)
                    {
                        // es una clase
                        eaUtils.printOut(definition.Name + " es una clase");
                        if (json.definitions[definition.Name] != null)
                        {
                            generateClassReference(definition.Name);
                        }
                        else
                        {
                            // puede ser una clase que no esté incluida en definitions sino que se defina acá mismo.
                            eaUtils.printOut(definition.Name + " es una clase no incluida en definitions");
                        }
                    }
                }
            }
        }

        private Element generateClassReference(string className)
        {
            Element message = null;
            Dictionary<string, Element> requireds = new Dictionary<string, Element>();

            if ( ! classes.ContainsKey("#/definitions/" + className) )
            {
                message = serviceModel.addClass(className);

                classes.Add("#/definitions/" + className, message);

                try
                {
                    foreach (dynamic required in json.definitions[className].required)
                    {
                        requireds.Add(required.Value, message);
                    }
                }
                catch (Exception) { }

                try
                {
                    this.manageProperties(json.definitions[className], message, requireds);

                    message.Elements.Refresh();
                }
                catch (Exception) { this.eaUtils.printOut("No se encuentra la referencia a la clase " + className); }

                this.manageMessageExample(json.definitions[className], message);
            }
            return message;
        }

        private void manageMessageExample( dynamic classDefinition, Element message )
        {
            if (classDefinition["example"] != null)
            {
                // al ser una entidad el ejemplo viene en un string, este string tiene relación con el mimetype.

                // dado que el mime type del consumes y produces pueden ser varios intentamos 1ro con json.
                // si esto no funciona lo metemos como un string (aunque no tengo idea donde)
                try
                {
                    JObject jsonExample = Newtonsoft.Json.Linq.JObject.Parse(classDefinition["example"].Value);
                    Element classToInstance;
                    Element instance;

                    foreach( dynamic example in jsonExample.Children())
                    {
                        this.eaUtils.printOut("Ejemplo crear para " + example.Name);

                        classToInstance = this.eaUtils.elementUtils.search( example.Name, message.Elements, true);

                        if(classToInstance != null)
                        {
                            instance = this.eaUtils.elementUtils.createInstance(example.Name, classToInstance, this.eaUtils.repository.GetPackageByID(message.PackageID), null,null);

                            foreach (dynamic propertyL1 in example.Children())
                            {
                                foreach (dynamic property in propertyL1.Children())
                                {
                                    // si estas son propiedades de una clase es simple, pero si hay propiedades que son de otras clases...

                                    foreach (EA.Attribute attr in instance.Attributes)
                                    {
                                        if (attr.Name == property.Name)
                                        {
                                            attr.Default = property.Value;
                                            attr.Update();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e) { eaUtils.printOut(e.ToString()); }
            }
        }

        private void manageProperties( dynamic classDefinition, Element message, Dictionary<string, Element> requireds)
        {
            string classNameFind;

            if(classDefinition["properties"] != null)
            {

                foreach (dynamic property in classDefinition.properties)
                {
                    if (classDefinition.properties[property.Name]["$ref"] != null)
                    {
                        classNameFind = getClassNameFromRef(classDefinition.properties[property.Name]);
                        generateAssociation(message, property.Name, classNameFind, requireds, false);
                    }
                    else if (classDefinition.properties[property.Name].type == "array")
                    {
                        if (classDefinition.properties[property.Name].items != null) // esto deberia ser una asociacion
                        {
                            if (classDefinition.properties[property.Name].items.type != null)
                            {
                                // esto deberia ser un atributo con una cardinalidad  de 0 a N
                                if (classDefinition.properties[property.Name].items.type == "object")
                                {
                                    // en este caso las propiedades son objetos.
                                    if (classDefinition.properties[property.Name].items.properties.Count == 1)
                                    {
                                        // cuando hay 1 sólo elemento se genera la clase y se le agrega la cardinalidad y el rol
                                        dynamic propertyObj = classDefinition.properties[property.Name].items.properties[0];

                                        ArrayList elementAndConnector = generateInner(message, classDefinition.properties[property.Name].items.properties[propertyObj.Name], propertyObj, requireds);

                                        Connector connector = ((Connector)elementAndConnector[1]);

                                        connector.ClientEnd.Cardinality = "0..*";
                                        connector.ClientEnd.Role = property.Name;
                                        connector.Update();
                                    }
                                    else
                                    {
                                        // Al ser varios los objetos hay que agregar una clase que los contenga.

                                        ArrayList elementAndConnector = generateInner(message, classDefinition.properties[property.Name], property, requireds);

                                        Connector connector = ((Connector)elementAndConnector[1]);

                                        connector.ClientEnd.Cardinality = "0..*";
                                        connector.ClientEnd.Role = property.Name;
                                        connector.Update();

                                        Element itemClassMessage = ((Element)elementAndConnector[0]);

                                        foreach (dynamic propertyObj in classDefinition.properties[property.Name].items.properties)
                                        {
                                            // estos pueden ser atributos u objetos.

                                            if (classDefinition.properties[property.Name].items.properties[propertyObj.Name].type == "object")
                                            {
                                                generateInner(itemClassMessage, classDefinition.properties[property.Name].items.properties[propertyObj.Name], propertyObj, requireds);
                                            }
                                            else
                                            {
                                                this.createAttribute(itemClassMessage, classDefinition, propertyObj, requireds);
                                                //this.manageProperties(classDefinition.properties[property.Name].items.properties[propertyObj.Name], message, requireds);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (classDefinition.properties[property.Name].items["$ref"] != null)
                            {
                                classNameFind = getClassNameFromRef(classDefinition.properties[property.Name].items);

                                generateAssociation(message, property.Name, classNameFind, requireds, true);
                            }
                        }
                    }

                    else if (classDefinition.properties[property.Name].type == "object")
                    {
                        // al ser un objeto vuelve a entrar
                        eaUtils.printOut(property.Name + " es una clase");
                        if (json.definitions[property.Name] != null)
                        {
                            generateClassReference(property.Name);
                        }
                        else
                        {
                            // puede ser una clase que no esté incluida en definitions sino que se defina acá mismo.
                            eaUtils.printOut(property.Name + " es una clase no incluida en definitions");
                            generateInner(message, classDefinition.properties[property.Name], property, requireds);
                        }
                    }
                    else
                    {
                        this.createAttribute(message, classDefinition, property, requireds);
                    }
                }
            }

            if (classDefinition.type == "string" && classDefinition["enum"] != null)
            {
                message.Stereotype = "enumeration";
                message.Update();
                EA.Attribute attr;

                foreach (var e in classDefinition["enum"])
                {
                    attr = message.Attributes.AddNew(e.Value, "");
                    attr.Stereotype = "enum";
                    attr.Update();
                }
            }
        }

        /// <summary>
        /// Se crea un elemento dentro del parent dado que no viene dfinitions.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="classDefinition"></param>
        /// <param name="property"></param>
        /// <param name="requireds"></param>
        private ArrayList generateInner(Element parent, dynamic classDefinition, dynamic property, Dictionary<string, Element> requireds)
        {
            Element message = (Element)parent.Elements.AddNew(property.Name, "Class");

            message.Update();
            parent.Refresh();

            this.eaUtils.printOut("Mensaje creado "+ property.Name);

            if (classDefinition["properties"] != null)
            {
                this.manageProperties(classDefinition, message, requireds);
            }

            Connector connector = this.eaUtils.connectorUtils.addConnectorNesting(message, parent, null, null);

            ArrayList elementAndConnector = new ArrayList();
            elementAndConnector.Add(message);
            elementAndConnector.Add(connector);

            return elementAndConnector;
        }

        private void createAttribute(Element message, dynamic classDefinition, dynamic property, Dictionary<string, Element> requireds)
        {
            EA.Attribute attr = message.Attributes.AddNew(property.Name, "");
            attr.Stereotype = "ExtendedAttribute";

            eaUtils.printOut("property name " + property.Name + " value " + property.Value);
            if(property.Value["enum"] != null)
            {
                EA.Element enumReference = generateEnum(message, attr, property.Value["enum"]);
                attr.Type = enumReference.Name;
                attr.ClassifierID = enumReference.ElementID;
            }
            else
            {
                attr.Type = property.Value.type;
            }

            attr.Update();

            if (requireds.ContainsKey(property.Name))
            {
                eaUtils.taggedValuesUtils.setRequired(attr, "true");
            }
            else
            {
                eaUtils.taggedValuesUtils.setRequired(attr, "false");
            }

            if (property.Value["format"] != null)
            {
                eaUtils.taggedValuesUtils.setFormat(attr, property.Value["format"].Value);
            }
        }

        private void generateAssociation(Element element, string propKey, string classNameFind, Dictionary<string, Element> requireds, Boolean asArray)
        {

            Element referencedElement = getReferenced(classNameFind);
            Connector connection;

            connection = eaUtils.connectorUtils.addConnectorAssociation(element, referencedElement, null, null);

            connection.SupplierEnd.Role = propKey;
            connection.Update();

            if (requireds.ContainsKey(propKey))
            {
                connection.SupplierEnd.Cardinality = (asArray ? "1..*" : "1");
                eaUtils.taggedValuesUtils.setRequired(connection, "true");
            }
            else
            {
                connection.SupplierEnd.Cardinality = (asArray ? "0..*" : "0..1");
                eaUtils.taggedValuesUtils.setRequired(connection, "false");
            }

            connection.Update();
        }

        private string getClassNameFromRef(dynamic reference)
        {
            string[] referenced = reference["$ref"].Value.Split('/');

            return referenced[referenced.Length - 1];
        }

        private Element getReferenced(string classNameFind)
        {
            Element referencedElement = getMessage(classNameFind);

            if (referencedElement == null)
            {
                referencedElement = generateClassReference(classNameFind);
            }
            return referencedElement;
        }

        private Element getMessage(string classNameFind)
        {
            Element message = null;
            return classes.TryGetValue("#/definitions/" + classNameFind, out message) ? message : null;
        }

        private Element generateEnum(Element parent, EA.Attribute attr, dynamic enumeration)
        {
            EA.Element element;

            element = (Element)parent.Elements.AddNew(attr.Name, "enumeration");
            element.Update();

            foreach (var e in enumeration)
            {
                attr = element.Attributes.AddNew(e.Value, "");
                attr.Stereotype = "enum";
                attr.Update();
            }
            element.Attributes.Refresh();

            return element;
        }

        private void generateInterfaces()
        {
            foreach (dynamic path in json.paths)
            {
                generateInterface(path.Name);
            }
        }

        private void generateInterface(string path)
        {
            EA.Element eaInterface;
            EA.Method operation;
            InterfaceInfo interfaceInfo = new InterfaceInfo(path);
            EA.Parameter parameter;

            // esto objeto se tiene que reemplazar por el path completo desde el root json.
            dynamic operationInfo;
            dynamic parameterInfo;
            string interfaceName;
            dynamic responseInfo;

            interfaceName = interfaceInfo.name.Substring(1, (interfaceInfo.name.Length - 2));

            eaInterface = serviceModel.synchronizeInterface(interfaceName, path );

            // ojo con esto porque no es algo generico sino que aplica solo para el caso de las telcos.
            //setValueToTaggedValue(eaInterface, "TMFORUM endpoint", "" );

            foreach (var operationKey in json.paths[path])
            {
                operationInfo = json.paths[path][operationKey.Name];
                responseInfo = operationInfo.responses;

                //interfaceInfo.operationInfo = {};
                interfaceInfo.operationData.Add( new OperationData( path, operationKey.Name) );

                operation = eaInterface.Methods.AddNew(operationKey.Name, "");

                if ( operationInfo.operationId != null)
                {
                    operation.Alias = operationInfo.operationId.Value;
                }

                if (operationInfo.summary != null)
                {
                    operation.Notes = operationInfo.summary.Value;
                }

                operation.Update();

                // security
                MethodConstraint precondition;
                if (json.paths[path][operationKey.Name]["security"] != null)
                {
                    // esto es una lista de objetos
                    foreach (var skey in json.paths[path][operationKey.Name]["security"])
                    {
                        // lista de string con la forma "key:value"
                        foreach (var si in json.paths[path][operationKey.Name]["security"][skey])
                        {
                            precondition = operation.PreConditions.AddNew(json.paths[path][operationKey.Name]["security"][skey][si], "");
                            // la precondicion es la key:value y las notas es la skey en el ejemplo de swagger es petstore_auth
                            precondition.Notes = skey;
                            precondition.Update();
                        }
                    }
                }

                if (json.paths[path][operationKey.Name]["responses"] != null)
                {
                    Boolean addMultiplicity = false;

                    foreach (var responseKey in json.paths[path][operationKey.Name]["responses"])
                    {
                        addMultiplicity = false;
                        // el schema puede contener "$ref", type=array con items  o type=string
                        if (json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema != null)
                        {
                            EA.Connector connector;
                            Element message = null;
                            if (json.paths[path][operationKey.Name]["responses"][responseKey.Name]["$ref"] != null)
                            {
                                try
                                {
                                    message = getMessage(getClassNameFromRef(json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema));
                                }
                                catch (Exception) { }

                            }
                            else if (json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema["$ref"] != null)
                            {
                                try
                                {
                                    message = getMessage( getClassNameFromRef(json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema) );
                                }
                                catch (Exception) { }
                            }
                            else if (json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema.type.Value == "array")
                            {
                                try
                                {
                                    addMultiplicity = true;
                                    if (json.paths[path][operationKey.Name]["responses"][responseKey.Name].schema.items["$ref"] != null)
                                    {
                                        message = getMessage( getClassNameFromRef(responseInfo[responseKey.Name].schema.items) );
                                    }
                                }
                                catch (Exception) { }
                            }
                            if (message != null)
                            {
                                connector = eaUtils.connectorUtils.addConnectorDependency(eaInterface, message, null, null);
                                connector.Name = operationKey.Name;
                                connector.SupplierEnd.Role = "response." + responseKey.Name;

                                if( addMultiplicity )
                                {
                                    connector.SupplierEnd.Cardinality = "1..*";
                                }

                                connector.Update();

                                eaUtils.taggedValuesUtils.set(connector, "description", json.paths[path][operationKey.Name]["responses"][responseKey.Name].description.Value, true);

                                if (json.paths[path][operationKey.Name]["responses"][responseKey.Name].headers != null)
                                {
                                    header2taggedValues(connector, path, operationKey.Name, responseKey.Name);
                                }
                            }
                        }
                        else
                        {
                            // responseKey 
                            /*
                                responses: {
                                          400: {
                                            description: "Invalid ID supplied"
                                          },
                                          404: {
                                            description: "Pet not found"
                                          },
                                          405: {
                                            description: "Validation exception"
                                          }
                                        }					
                            */
                            // cada codigo es una clase, el nombre de la clase es la descripcion
                            // se hace una relacion de asociacion el nombre es la operacion y el rol es en este caso
                            // reponse +"."+ codigo (400)

                            try
                            {
                                string responseCode = responseKey.Name;

                                Element responseCodeElement = getMessage4xx(responseCode);
                                if (responseCodeElement == null)
                                {
                                    responseCodeElement = serviceModel.definitionPackage.Elements.AddNew(responseCode, "Class");
                                    responseCodeElement.Stereotype = "ErrorMessage";
                                    responseCodeElement.Update();
                                    EA.Attribute responseDescription = responseCodeElement.Attributes.AddNew("description", "string");

                                    responseDescription.Default = responseInfo[responseKey.Name].description.Value;
                                    responseDescription.Update();

                                    this.classes["#/4xx/" + responseCode] = responseCodeElement;
                                }
                                Connector connector = eaUtils.connectorUtils.addConnectorDependency(eaInterface, responseCodeElement, null, null);
                                connector.Name = operationKey.Name;
                                connector.SupplierEnd.Role = "response." + responseCode;
                                connector.Update();
                            }
                            catch (Exception e)
                            {
                                this.eaUtils.printOut(e.ToString());
                            }
                        }
                    }
                }

                int paramIndex = 0;
                if(operationInfo != null && operationInfo.parameters != null)
                {
                    foreach (var i in operationInfo.parameters)
                    {
                        parameterInfo = i;

                        eaUtils.printOut(parameterInfo.name.Value);

                        parameter = operation.Parameters.AddNew(parameterInfo.name.Value, (parameterInfo.type == null ? parameterInfo.name.Value : parameterInfo.type.Value));
                        parameter.Notes = parameterInfo.description != null ? parameterInfo.description.Value : "";
                        parameter.Alias = parameterInfo.operationId != null ? parameterInfo.operationId.Value : "";
                        parameter.Position = paramIndex++;

                        parameter.Update();

                        eaUtils.taggedValuesUtils.set(parameter, "in", parameterInfo["in"].Value);

                        eaUtils.taggedValuesUtils.set(parameter, "required", parameterInfo["required"].Value ? "true" : "false");

                        if (parameterInfo["enum"] != null)
                        {
                            eaUtils.taggedValuesUtils.set(parameter, "enum", Newtonsoft.Json.JsonConvert.SerializeObject(parameterInfo["enum"]));
                        }

                        if (parameterInfo.schema != null)
                        {
                            if (parameterInfo.schema["$ref"] != null)
                            {
                                Element message = getMessage(getClassNameFromRef(parameterInfo.schema));
                                Connector connector = eaUtils.connectorUtils.addConnectorDependency(eaInterface, message, null, null);
                                connector.Name = operation.Name;
                                connector.SupplierEnd.Role = "request";
                                connector.Update();

                                parameter.ClassifierID = message.ElementID.ToString();

                                parameter.Update();

                            }
                            if (parameterInfo.schema["type"] != null && parameterInfo.schema["type"].Value == "array")
                            {
                                Element message = getMessage(getClassNameFromRef(parameterInfo.schema.items));
                                Connector connector = eaUtils.connectorUtils.addConnectorDependency(eaInterface, message, null, null);
                                connector.Name = operation.Name;
                                connector.SupplierEnd.Role = "request";
                                connector.SupplierEnd.Cardinality = (parameterInfo["required"].Value ? "1" : "0") + "..*";
                                connector.Update();
                            }
                        }
                    }
                    operation.Parameters.Refresh();
                }
            }

            eaInterface.Methods.Refresh();

            serviceModel.definitionPackage.Elements.Refresh();

            interfaceInfo.EAInterface = eaInterface;

            interfaces.Add(interfaceInfo);
        }

        private void generarServicio()
        {
            
            string serviceName = json.basePath.Value;

            try
            {
                serviceName = EAUtils.StringUtils.toPascal(json.basePath.Value.Replace("/", "_").Substring(1));
            }
            catch(Exception) {}

            serviceModel.synchronizeService(serviceName);
            
	        foreach(var key in SWAGGER__ROOT)
            {
                if( key != "swagger")
                {
                    foreach (var value in json[key])
                    {
                        // todo ver donde poner los produces y consumes genéricos.
                        //serviceModel.synchronizeInfoAttribute(key, json.info[key].Value);
                    }
                }else
                {
                    serviceModel.synchronizeSwaggerAttribute(key, json[key].Value);
                }
            }

            serviceModel.findInfo();

	        foreach(var key in serviceModel.SWAGGER__INFO)
            {
                if(json.info[key] != null)
                {
                    serviceModel.synchronizeInfoAttribute(key, json.info[key].Value);
                }
            }

            serviceModel.findContactAndLicence();


            if( json["contact"] != null )
            {
                string attrName = "name";
                if( json.contact[attrName] != null )
                {
                    serviceModel.synchronizeContactAttribute(attrName, json.contact[attrName]);

                }
                attrName = "url";
                if (json.contact[attrName] != null)
                {
                    serviceModel.synchronizeContactAttribute(attrName, json.contact[attrName]);
                }
                attrName = "email";
                if (json.contact[attrName] != null)
                {
                    serviceModel.synchronizeContactAttribute(attrName, json.contact[attrName]);
                }
            }
            if (json["license"] != null)
            {
                string attrName = "name";
                if (json.license[attrName] != null)
                {
                    serviceModel.synchronizeLicenseAttribute(attrName, json.license[attrName]);
                }
                attrName = "url";
                if (json.license[attrName] != null)
                {
                    serviceModel.synchronizeLicenseAttribute(attrName, json.license[attrName]);
                }
            }
            serviceModel.swagger.Elements.Refresh();
            serviceModel.definitionPackage.Elements.Refresh();
        }

        private void header2taggedValues(Connector connector, string path, string operationKey, string responseKey)
        {
	        foreach(var headerKey in json.paths[path][operationKey]["responses"][responseKey].headers)
            {
		        foreach(var headerDataKey in json.paths[path][operationKey]["responses"][responseKey].headers[headerKey.Name])
                {
                    eaUtils.taggedValuesUtils.set(connector, "header." + headerKey.Name + "." + headerDataKey.Name, json.paths[path][operationKey]["responses"][responseKey].headers[headerKey.Name][headerDataKey.Name].Value );
                }
            }
        }

        private Element getMessage4xx(string classNameFind)
        {
            Element message;
            return (this.classes.TryGetValue("#/4xx/" + classNameFind, out message) ? message: null);
        }

        private void generateModel()
        {
            Element providedInterface;
	
	        foreach(var i in interfaces)
            {
                // exponer la interfaz en el servicio
                providedInterface = exposeInterface(i.EAInterface);
                producesConsumes(providedInterface, i);
            }
        }

        private EA.Element exposeInterface(Element eaInterface)
        {
	        EA.Element providedInterface;
            providedInterface = (EA.Element)serviceModel.service.EmbeddedElements.AddNew(eaInterface.Name, "ProvidedInterface");
	        providedInterface.ClassifierID = eaInterface.ElementID;
	        providedInterface.Stereotype = "RESTFul";
	        providedInterface.Update();

            eaUtils.connectorUtils.addConnectorInstantiation(providedInterface, eaInterface, null);
	

	        foreach( var uri in serviceModel.uris )
	        {
                eaUtils.connectorUtils.addConnectorAssociation(providedInterface, uri, null, null, EAUtils.ConnectorUtils.DIRECTION__SOURCE_DESTINATION);
            }
	        return providedInterface;
        }

        private void producesConsumes(Element providedInterface, InterfaceInfo interfazInfo)
        {
            string mediaTypeName;
            Element mediaType;
            
            foreach(OperationData operationData in interfazInfo.operationData)
            {
                if (operationData.operationInfo(json)["consumes"] != null)
                {
                    // crear interfaz con el media type y hacer una relacion de dependencia
                    foreach (var c in operationData.operationInfo(json)["consumes"])
                    {
                        mediaTypeName = c.Value;
                        mediaType = getMediaType(mediaTypeName);
                        Connector connector = eaUtils.connectorUtils.addConnectorDependency(providedInterface, mediaType, null, null);
                        connector.SupplierEnd.Role = operationData.operation;
                        connector.Update();
                    }
                }
                if (operationData.operationInfo(json)["produces"] != null)
                {
                    // crear interfaz con el media type y hacer una relacion de realizacion
                    foreach (var c in operationData.operationInfo(json)["produces"])
                    {
                        mediaTypeName = c.Value;
                        mediaType = getMediaType(mediaTypeName);
                        Connector connector = eaUtils.connectorUtils.addConnectorRealization(providedInterface, mediaType, null, null);
                        connector.SupplierEnd.Role = operationData.operation;
                        connector.Update();
                    }
                }
            }
        }

        private Element getMediaType( string mediaTypeName )
        {
            Element mediaType;

            if (! mediaTypes.TryGetValue(mediaTypeName, out mediaType))
            {
                mediaType = (Element)serviceModel.definitionPackage.Elements.AddNew("Interface", "Interface");
                mediaType.Name = mediaTypeName;
                mediaType.Stereotype = "mediaType";
                mediaType.Update();
                mediaTypes.Add(mediaTypeName, mediaType);
            }
            return mediaType;
        }
        
    }

    class InterfaceInfo
    {
        public List<string> parameters = new List<string>();
        public string name;
        public string path;
        public Element EAInterface;
        public List<OperationData> operationData = new List<OperationData>();

        public InterfaceInfo(string path)
        {
            string[] pathSplitted = path.Split('/');
            this.name = "";

            this.path = path;

            foreach (string pathPart in pathSplitted)
            {
                if (pathPart.IndexOf("{") != -1)
                {
                    this.name += pathPart.Replace("{", "").Replace("}", "") + "_";
                }
                else
                {
                    this.name += pathPart + "_";
                }
            }
        }
    }

    public class OperationData
    {
        public string path;
        public string operation;

        public OperationData(string path, string operation)
        {
            this.path = path;
            this.operation = operation;
        }

        public dynamic operationInfo(dynamic json )
        {
            // json.paths["/products"].post

            return json.paths[path][operation];
        }
    }
}
