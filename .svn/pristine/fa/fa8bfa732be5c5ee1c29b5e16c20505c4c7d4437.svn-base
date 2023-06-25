using EA;
using ServiceCommons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestFul
{
    class RestFulABM
    {
        Element sourceElement;
        EAUtils.EAUtils eaUtils;
        Diagram currentDiagram;
        Package currentPackage;

        ServiceUtils serviceUtils;

        public RestFulABM(EAUtils.EAUtils eaUtils, Element sourceElement)
        {
            this.eaUtils = eaUtils;
            this.sourceElement = sourceElement;
            this.currentDiagram = eaUtils.repository.GetCurrentDiagram();
            this.currentPackage = eaUtils.repository.GetPackageByID(this.currentDiagram.PackageID);
            this.serviceUtils = new ServiceUtils(eaUtils);
        }

        public void generar()
        {
            // crear elementos mensaje (request, response, error)
            // crear interfaz para el elemento con método get y post
            // crear interfaz para el elementoId con método get, put, delete.
            // crear el elemento servicio
            // instanciar cada interfaz en el servicio.

            Element requestResponse = serviceUtils.createMessage(sourceElement.Name, "RequestResponse", currentPackage);
            Element interfaz        = serviceUtils.createInterface(sourceElement.Name, false, requestResponse, currentPackage);

            createMethodGetCollection(interfaz, requestResponse);
            createMethodPost(interfaz, requestResponse);
            
            Element interfazId = serviceUtils.createInterface(sourceElement.Name, true, requestResponse, currentPackage);

            createMethodGet( interfazId, requestResponse );
            createMethodPut( interfazId, requestResponse );

            Element service             = serviceUtils.createService("Gestionar"+sourceElement.Name +"V1", currentPackage);
            Element providedInterface   = serviceUtils.createProvidedInterface( service, interfaz, "RestFul", this.currentDiagram);
            Element providedInterfaceId = serviceUtils.createProvidedInterface( service, interfazId, "RestFul", this.currentDiagram);

            Element mediaTypeJson = createMediaType( providedInterface, "json" );
            Element mediaTypeXml  = createMediaType( providedInterface, "xml"  );

            createMediaType( providedInterfaceId, "json", mediaTypeJson);
            createMediaType( providedInterfaceId, "xml" , mediaTypeXml);
        }



        private void createMethodGetCollection(Element element, Element requestResponse)
        {
            Method method = (Method)element.Methods.AddNew("get", "");
            method.Update();
            element.Methods.Refresh();

            EA.Parameter parameter;
            // por cada atributo
            // si es una clase usamos los atributos, si es un container entonces usamos sus elementos.
            if (sourceElement.Stereotype == "DataContainer")
            {
                foreach (Element childElement in sourceElement.Elements)
                {
                    parameter = (EA.Parameter) method.Parameters.AddNew(childElement.Name, eaUtils.taggedValuesUtils.get(childElement, "Tipo de dato", "").asString());
                    parameter.Update();
                    method.Parameters.Refresh();
                }
            }
            else if (sourceElement.Type == "Class")
            {
                foreach (EA.Attribute attribute in sourceElement.Attributes)
                {
                    parameter = (EA.Parameter)method.Parameters.AddNew(attribute.Name, attribute.Type);
                    parameter.Update();
                    method.Parameters.Refresh();
                }
            }
            // hacemos la relación de dependencia.
            Connector connector = eaUtils.connectorUtils.addConnector( element, requestResponse, "Dependency", "get", null);
            connector.SupplierEnd.Role = "response.200";
            connector.Update();
            //requestResponse.Refresh();
        }

        private void createMethodPost(Element interfaz, Element requestResponse)
        {
            Method method = (Method)interfaz.Methods.AddNew("post", "");
            method.Update();

            EA.Parameter parameter = (EA.Parameter)method.Parameters.AddNew(requestResponse.Name, requestResponse.Name);
            //parameter.ClassifierID = requestResponse.ElementID.ToString();
            parameter.Update();
            method.Parameters.Refresh();

            // hacemos la relación de dependencia.
            Connector connector = eaUtils.connectorUtils.addConnector( interfaz, requestResponse, "Dependency", "post", null);
            connector.SupplierEnd.Role = "request";
            connector.Update();

            connector = eaUtils.connectorUtils.addConnector( interfaz, requestResponse, "Dependency", "post", null);
            connector.SupplierEnd.Role = "response.200";
            connector.Update();
        }

        private void createMethodGet(Element element, Element requestResponse)
        {
            Method method = (Method)element.Methods.AddNew("get", "");
            method.Update();

            EA.Parameter parameter = (EA.Parameter) method.Parameters.AddNew(requestResponse.Name+"Id", "string");
            parameter = (EA.Parameter) method.Parameters.AddNew("fields", "string");
            parameter.ClassifierID = requestResponse.ElementID.ToString();
            parameter.Update();
            method.Parameters.Refresh();


            // hacemos la relación de dependencia.
            Connector connector = eaUtils.connectorUtils.addConnector( element, requestResponse, "Dependency", "get", null);
            connector.SupplierEnd.Role = "response.200";
            connector.Update();
            //requestResponse.Refresh();
        }

        private void createMethodPut(Element element, Element requestResponse)
        {
            Method method = (Method)element.Methods.AddNew("put", "");
            method.Update();

            EA.Parameter parameter = (EA.Parameter)method.Parameters.AddNew(requestResponse.Name + "Id", "string");
            parameter = (EA.Parameter)method.Parameters.AddNew(requestResponse.Name, requestResponse.Name);
            parameter.ClassifierID = requestResponse.ElementID.ToString();
            parameter.Update();
            method.Parameters.Refresh();

            // hacemos la relación de dependencia.
            Connector connector = eaUtils.connectorUtils.addConnector( element, requestResponse, "Dependency", "put", null);
            connector.SupplierEnd.Role = "response.200";
            connector.Update();
            //requestResponse.Connectors.Refresh();
        }



        private Element createMediaType(Element element, string mediaType)
        {
            return createMediaType(element, mediaType, null);
        }

        private Element createMediaType(Element element, string mediaType, Element media)
        {
            if (media == null)
            {
                media = (Element)currentPackage.Elements.AddNew(mediaType, "Interface");
                media.Stereotype = "mediaType";
                media.Update();
            }

            // agregar consumes y produces
            eaUtils.connectorUtils.addConnectorDependency(element, media, "consumes", null);
            eaUtils.connectorUtils.addConnectorRealization(element, media, "produces", null);

            return media;
        }
    }
}
