using EA;
using EAUtils;
using EAUtils.model;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenAPI.generar
{
    public class Generar
    {
        EAUtils.EAUtils eaUtils;
        private EstructuraSolucion estructuraSolucion;
        private OpenApiDocument openAPIDocument = new OpenApiDocument();
        public Generar(EAUtils.EAUtils eAUtils) { 
            
            this.eaUtils= eAUtils;
        }
        private bool establecerPaquete()
        {
            this.estructuraSolucion = new EstructuraSolucion();

            return this.estructuraSolucion.establecer(this.eaUtils);
        }
        public void generar()
        {
            if( this.establecerPaquete())
            {
                this.estructuraSolucion.establecerModels();
                this.estructuraSolucion.establecerResponses();

                this.openAPIDocument.Paths = new OpenApiPaths();
                this.openAPIDocument.Components = new OpenApiComponents();

                this.generarInfo();
                this.generarServers();
                this.generarPaths();

                DirectorioArchivoUsuarioHelper directorioUsuario = new DirectorioArchivoUsuarioHelper(this.eaUtils)
                    .withPaquete(this.estructuraSolucion.Principal);

                directorioUsuario.initSaveFileInfo().fileName(this.estructuraSolucion.Principal.Name + ".json");

                string nombreCompletoArchivo = directorioUsuario.nombreCompletoArchivo();

                TextWriter textWriter = new StreamWriter(nombreCompletoArchivo);

                OpenApiJsonWriter jsonWriter = new OpenApiJsonWriter(textWriter);

                this.openAPIDocument.SerializeAsV3(jsonWriter);

                jsonWriter.Flush();

                textWriter.Close();
                textWriter.Dispose();
            }

            this.eaUtils.printOut("Fin");
        }

        private void generarInfo()
        {        
            OpenApiInfo info = new OpenApiInfo();

            info.Title = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Info, "title", "").asString();
            info.Description = this.eaUtils.notes2Txt(this.estructuraSolucion.Info.Notes);

            string termsOfService = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Info, "termsOfService", "").asString();

            if(termsOfService.Length> 0)
            {
                info.TermsOfService = new Uri(termsOfService);
            }

            info.Version = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Info, "openapi", "3.0.0").asString();

            /*
            Dictionary<string, string> extensions = this.eaUtils.taggedValuesUtils.getByPrefix(this.estructuraSolucion.Info, "x-", null);

            info.Extensions = extensions;
            */

            info.Contact = new OpenApiContact();

            info.Contact.Name = this.estructuraSolucion.Contact.Name;
            info.Contact.Email = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Contact, "email", "").asString();

            string url = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Contact, "url", "").asString();
            if (url.Length > 0)
            {
                info.Contact.Url = new Uri(url);
            }

            info.License = new OpenApiLicense();

            info.License.Name = this.estructuraSolucion.Licence.Name;
            url = this.eaUtils.taggedValuesUtils.get(this.estructuraSolucion.Licence, "url", "").asString();
            if(url.Length > 0)
            {
                info.License.Url = new Uri(url);
            }

            this.openAPIDocument.Info = info;
        }

        private void generarServers()
        {
            foreach( EA.Element element in this.estructuraSolucion.Servers)
            {
                OpenApiServer server = new OpenApiServer();

                this.openAPIDocument.Servers.Add(server);

                server.Url = element.Name;
                server.Description = this.eaUtils.notes2Txt(element.Notes);

                foreach( EA.Attribute attribute in element.Attributes)
                {
                    OpenApiServerVariable variable = new OpenApiServerVariable();

                    variable.Description = this.eaUtils.notes2Txt(attribute.Notes);
                    variable.Default = attribute.Default;

                    if( attribute.ClassifierID > 0)
                    {
                        EA.Element e = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);

                        if( e.Type == "Enumeration")
                        {
                            variable.Enum = new List<string>();

                            foreach( EA.Attribute a in e.Attributes)
                            {
                                variable.Enum.Add(a.Name);
                            }
                        }
                    }
                    server.Variables.Add(attribute.Name, variable);
                }
            }
        }
        private void generarPaths()
        {
            List<object> interfaces = this.eaUtils.packageUtils.getElementFromFilter(this.estructuraSolucion.Definicion, "Interface", null, null, false);
            EA.Element interfaz = null;            

            foreach(object obj in interfaces)
            {
                interfaz = (EA.Element)obj;

                this.eaUtils.printOut("Analizando interfaz: " + interfaz.Name);

                foreach(EA.Method method in interfaz.Methods)
                {
                    OpenApiOperation operation = null;

                    this.eaUtils.printOut("--------");

                    string pathName = method.Alias.Length > 0 ? interfaz.Alias + method.Alias: interfaz.Alias;
                    OpenApiPathItem path;

                    if (method.Stereotype == "openapi.operationref")
                    {
                        path = new OpenApiPathItem();

                        openAPIDocument.Paths.Add(pathName, path);
                    }
                    
                    if (this.openAPIDocument.Paths.ContainsKey(pathName))
                    {
                        path = this.openAPIDocument.Paths[pathName];

                        this.eaUtils.printOut("Obteniendo path: " + pathName);
                    }
                    else
                    {
                        path = new OpenApiPathItem();

                        openAPIDocument.Paths.Add(pathName, path);

                        this.eaUtils.printOut("Creando path: " + pathName);
                    }

                    this.generarPath(method, path, null);

                }
            }
        }

        private void generarPath(EA.Method method, OpenApiPathItem path, OpenApiOperation operation)
        {
            if (method.Stereotype == "openapi.operationref")
            {
                path.Reference = new OpenApiReference();

                path.Reference.ExternalResource = method.Name;
            }
            else
            {
                string httpMethod = this.eaUtils.taggedValuesUtils.get(method, Constantes.HTTP_METHOD, "").asString();

                OperationType operationType = OperationType.Get;

                switch (httpMethod)
                {
                    case "post":
                        operationType = OperationType.Post;
                        break;
                    case "delete":
                        operationType = OperationType.Delete;
                        break;
                    case "put":
                        operationType = OperationType.Put;
                        break;
                    case "patch":
                        operationType = OperationType.Patch;
                        break;
                    case "head":
                        operationType = OperationType.Head;
                        break;
                    case "options":
                        operationType = OperationType.Options;
                        break;
                }

                
                if (path.Operations.ContainsKey(operationType))
                {
                    operation = path.Operations[operationType];

                    this.eaUtils.printOut("Recuperando operación: " + operationType.ToString());
                }
                
                if (operation == null)
                {
                    operation = new OpenApiOperation();

                    path.Operations.Add(operationType, operation);

                    if (method.Notes.Length > 0)
                    {
                        operation.Description = this.eaUtils.notes2Txt(method.Notes);
                    }

                    TaggedValueWrapper w = this.eaUtils.taggedValuesUtils.get(method, Constantes.OPERATION_ID, "");

                    if (w.value.Length > 0)
                    {
                        operation.OperationId = w.asString();
                    }

                    w = this.eaUtils.taggedValuesUtils.get(method, Constantes.SUMMARY, "");
                    if (w.value.Length > 0)
                    {
                        operation.Summary = w.asString();
                    }

                    operation.Deprecated = (bool)this.eaUtils.taggedValuesUtils.get(method, Constantes.DEPRECATED, "false").asBoolean();

                    string ed = this.eaUtils.taggedValuesUtils.get(method, Constantes.EXTERNAL_DOCS, "").asString();

                    // @TODO 
                    if (ed.Length > 0)
                    {
                        operation.ExternalDocs.Url = new Uri(ed);
                        operation.ExternalDocs.Description = this.eaUtils.taggedValuesUtils.get(method, "externalDoc.description", "").asString();
                    }

                    string tags = this.eaUtils.taggedValuesUtils.get(method, Constantes.TAGS, "").asString();
                    if (tags.Length > 0)
                    {
                        string[] tagsSplited = tags.Split(new char[] { ',' });

                        foreach (string tag in tagsSplited)
                        {
                            if (tag.Length > 0)
                            {
                                OpenApiTag tagItem = new OpenApiTag();
                                tagItem.Name = tag;
                                operation.Tags.Add(tagItem);
                            }
                        }
                    }
                }

                this.generarParametersORequestBody(method, operation);

                if (operation.Responses.Count == 0 && this.estructuraSolucion.reponses.ContainsKey(method.ReturnType))
                {
                    this.generarResponse(operation, this.estructuraSolucion.reponses[method.ReturnType]);
                }
            }
        }
        private void generarParametersORequestBody(Method method, OpenApiOperation operation)
        {
            EA.Element parameterContainer;

            // estos son los parámetros del método
            foreach (EA.Parameter eaParameter in method.Parameters)
            {
                // lo normal es que exista sólo 1 parámetro en el método, porque los parámetros del método no permiten configurar lo necesario.
                // por eso se usa un contenedor para los parámetros y en el van los verdaderos parámetros.
                if (eaParameter.ClassifierID.Length > 0)
                {
                    int classifierId = int.Parse(eaParameter.ClassifierID);

                    parameterContainer = this.eaUtils.repository.GetElementByID(classifierId);

                    foreach (EA.Attribute attr in parameterContainer.Attributes)
                    {
                        if(attr.Stereotype == "openapi.parameter")
                        {
                            this.generarParameters(operation, attr);
                        }
                        else if (attr.Stereotype == "openapi.callback")
                        {
                            this.generarCallback(operation, attr);
                        }
                        else if (attr.Stereotype == "contentBody")
                        {
                            this.generarRequestBody(operation, attr);
                        }
                    }
                }
            }
        }

        private void generarCallback(OpenApiOperation operationParent, EA.Attribute eventAttribute)
        {
            OpenApiCallback callback = new OpenApiCallback();
            
            Element callbackDefinition = this.eaUtils.repository.GetElementByID(eventAttribute.ClassifierID);

            OpenApiPathItem path;
            RuntimeExpression runtimeExpression;
            string propertyReferenced = "";

            foreach(EA.Method method in callbackDefinition.Methods)
            {
                path = new OpenApiPathItem();
                
                if( operationParent.RequestBody.Content.Values.First().Schema.Reference != null)
                {
                    OpenApiSchema openApiSchema = openAPIDocument.Components.Schemas[operationParent.RequestBody.Content.Values.First().Schema.Reference.Id];

                    propertyReferenced = openApiSchema.Properties.First().Key;
                }
                else
                {
                    propertyReferenced = operationParent.RequestBody.Content.Values.First().Schema.Properties.First().Key;
                }

                runtimeExpression = RuntimeExpression.Build("$request.body#/"+ propertyReferenced);

                this.generarPath(method, path, null);

                callback.AddPathItem( runtimeExpression, path);

                operationParent.Callbacks.Add(method.Name, callback);
            }
        }

        private void generarResponse(OpenApiOperation operation, Element element)
        {
            OpenApiResponse response = null;
            OpenApiMediaType mediaType;
            EA.Element attrClassifierd;
            OpenApiSchema reference;
            string httpCode = "";
            string httpCurrentCode = "";

            // el elemento es un contenedor para distintos tipos de respuesta.
            foreach ( EA.Attribute attribute in element.Attributes)
            {
                httpCode = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.HTTP_CODE, "").asString();

                if( httpCode != httpCurrentCode)
                {
                    response = new OpenApiResponse();

                    operation.Responses.Add(httpCode, response);

                    httpCurrentCode = httpCode;

                    this.eaUtils.printOut("Creando response para: " + httpCode);
                }

                string responseIn = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.IN, "").asString();

                // tengo que ver bien cual es el caso
                if ( attribute.Type == "string" && responseIn != "header")
                {
                    response.Description = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.HTTP_CODE_DESCRPTION, "").asString();

                    continue;
                }

                mediaType = new OpenApiMediaType();


                if ( responseIn == "body")
                {
                    response.Description = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.HTTP_CODE_DESCRPTION, "").asString();

                    if (attribute.ClassifierID > 0)
                    {
                        // esto es el type, en este caso el type es objeto, este puede ser un array de objetos 
                        attrClassifierd = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);
                        
                        if(attrClassifierd.HasStereotype("SchemaObject"))
                        {
                            if( this.openAPIDocument.Components.Schemas.ContainsKey(attrClassifierd.Name) )
                            {
                                reference = this.openAPIDocument.Components.Schemas[attrClassifierd.Name];
                            }
                            else
                            {
                                reference = this.buildSchema(attrClassifierd);
                                reference.Type = "object";

                                this.openAPIDocument.Components.Schemas.Add(attrClassifierd.Name, reference);
                            }

                            mediaType.Schema = new OpenApiSchema();
                            mediaType.Schema.Type = "object";
                            mediaType.Schema.Reference = new OpenApiReference();

                            mediaType.Schema.Reference.Id = attrClassifierd.Name;
                            mediaType.Schema.Reference.Type = ReferenceType.Schema;
                            
                            this.eaUtils.printOut("Agregando respuesta como referencia: "+ attrClassifierd.Name);

                            response.Content.Add(this.eaUtils.taggedValuesUtils.get(attribute, Constantes.CONTENT_TYPE, "").asString(), mediaType);
                        }
                        else if(attrClassifierd.HasStereotype("Response"))
                        {
                            // un response que hay que construir, quizás llamando a este método.
                            //this.openAPIDocument.Components.Responses
                        }
                    }
                    else
                    {
                        string contentType = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.CONTENT_TYPE, "").asString();

                        if( contentType != "application/octet-stream" )
                        {
                            mediaType.Schema = this.buildSchema(attribute);
                            mediaType.Schema.Type = "object";

                        }

                        this.eaUtils.printOut("Agregando respuesta: " + attribute.Name);

                        response.Content.Add(contentType, mediaType);
                    }
                }
                else if(responseIn == "header")
                {
                    if (attribute.ClassifierID > 0)
                    {
                        // los headers en la respuesta no pueden ser referencias.
                        attrClassifierd = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);

                        foreach( EA.Attribute a in attrClassifierd.Attributes)
                        {
                            response.Headers.Add(attribute.Name, this.generarHeader(a));

                            this.eaUtils.printOut("Agregando header: " + a.Name);
                        }
                    }
                    else
                    {
                        response.Headers.Add(attribute.Name, this.generarHeader(attribute));
                        this.eaUtils.printOut("Agregando header: " + attribute.Name);
                    }
                }
                //response.Extensions
                //response.Links
                //response.UnresolvedReference
            }
        }

        private void generarRequestBody(OpenApiOperation operation, EA.Attribute attribute)
        {
            EA.Element attrClassifierd;

            string mType = attribute.Name;

            OpenApiMediaType mediaType = new OpenApiMediaType();

            if (attribute.ClassifierID > 0)
            {
                // esto es el type, en este caso el type es objeto, este puede ser un array de objetos 
                attrClassifierd = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);
                OpenApiSchema reference;

                if( ! this.openAPIDocument.Components.Schemas.ContainsKey(attrClassifierd.Name))
                {
                    reference = this.buildSchema(attrClassifierd);
                    reference.Type = "object";

                    this.openAPIDocument.Components.Schemas.Add(attrClassifierd.Name, reference);
                }
                else
                {
                    reference = this.openAPIDocument.Components.Schemas[attrClassifierd.Name];
                }

                mediaType.Schema = new OpenApiSchema();
                mediaType.Schema.Type = "object";
                mediaType.Schema.Reference = new OpenApiReference();

                mediaType.Schema.Reference.Id = attrClassifierd.Name;
                mediaType.Schema.Reference.Type = ReferenceType.Schema;
            }
            else
            {
                mediaType.Schema = this.buildSchema(attribute);
                mediaType.Schema.Type = "object";
            }

            if(operation.RequestBody == null)
            {
                operation.RequestBody = new OpenApiRequestBody();
            }

            operation.RequestBody.Content.Add(mType, mediaType);
        }

        private OpenApiHeader generarHeader(EA.Attribute attribute)
        {
            OpenApiHeader h = new OpenApiHeader();

            //h.AllowEmptyValue No se recomienda su uso
            //h.AllowReserved
            //h.Content
            h.Deprecated = (bool)this.eaUtils.taggedValuesUtils.get(attribute, "deprecated", "false").asBoolean();
            h.Description = this.eaUtils.notes2Txt(attribute.Notes);
            //h.Example = this.eaUtils.taggedValuesUtils.get(attribute, "example", "").asString();
            //h.Examples
            h.Explode = (bool)this.eaUtils.taggedValuesUtils.get(attribute, Constantes.EXPLODE, "false").asBoolean();
            //h.Extensions
            //h.Reference
            h.Required = (bool) this.eaUtils.taggedValuesUtils.get(attribute, Constantes.PARAMETER_REQUIRED, "true").asBoolean();
            h.Schema = this.buildSchema(attribute);
            //h.Style = new ParameterStyle();
            
            //h.UnresolvedReference

            return h;
        }
        private void generarParameters(OpenApiOperation operation, EA.Attribute attribute)
        {
            EA.Element attrClassifierd;

            this.eaUtils.printOut("Analizando parametros en "+ attribute.Name);

            if (attribute.ClassifierID > 0)
            {
                // esto es el type, en este caso el type es objeto que contiene parámetros, serán referencias sino están en el modelo 
                try
                {
                    attrClassifierd = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);

                    bool vaComoReferencia = this.estructuraSolucion.models.ContainsKey(attrClassifierd.Name);

                    this.eaUtils.printOut("¿Va como referencia? " + (vaComoReferencia ? "Sí" : "No"));

                    if (attrClassifierd.Stereotype == "enum")
                    {
                        this.eaUtils.printOut("Es un enum");

                        if (vaComoReferencia)
                        {
                            // va como referencia.
                            OpenApiParameter referencia = this.generarParametro(attribute, null);

                            referencia.Schema = new OpenApiSchema();

                            List<IOpenApiAny> enumItems = new List<IOpenApiAny>();

                            string type = attrClassifierd.Attributes.GetAt(0).Type;

                            foreach (EA.Attribute a in attrClassifierd.Attributes)
                            {
                                enumItems.Add(this.getOpenApiAny(type, a.Name, false));
                            }

                            referencia.Schema.Type = type;
                            referencia.Schema.Enum = enumItems;

                            if (attribute.Default.Length > 0)
                            {
                                referencia.Schema.Default = this.getOpenApiAny(type, attribute.Default, false);
                            }

                            OpenApiParameter param = this.generarParametro(attribute, null);

                            param.Reference = new OpenApiReference();

                            param.Reference.Id = attribute.Name;
                            param.Reference.Type = ReferenceType.Parameter;

                            operation.Parameters.Add(param);
                        }
                        else
                        {
                            OpenApiParameter param = this.generarParametro(attribute, null);

                            param.Schema = new OpenApiSchema();

                            List<IOpenApiAny> enumItems = new List<IOpenApiAny>();

                            string type = attrClassifierd.Attributes.GetAt(0).Type;

                            foreach (EA.Attribute a in attrClassifierd.Attributes)
                            {
                                enumItems.Add(this.getOpenApiAny(type, a.Name, false));
                            }

                            param.Schema.Type = type;
                            param.Schema.Enum = enumItems;

                            if (attribute.Default.Length > 0)
                            {
                                param.Schema.Default = this.getOpenApiAny(type, attribute.Default, false);
                            }

                            operation.Parameters.Add(param);
                        }
                    }
                    else
                    {
                        this.eaUtils.printOut("No es un enum");

                        foreach (EA.Attribute attr in attrClassifierd.Attributes)
                        {
                            if (vaComoReferencia)
                            {
                                OpenApiParameter referencia;

                                if (this.openAPIDocument.Components.Parameters.ContainsKey(attrClassifierd.Name))
                                {
                                    referencia = this.openAPIDocument.Components.Parameters[attrClassifierd.Name];
                                }
                                else
                                {
                                    referencia = this.generarParametro(attr, attrClassifierd.Name);

                                    referencia.Schema = this.buildSchema(attr);

                                    if (attribute.Default.Length > 0)
                                    {
                                        referencia.Schema.Default = this.getOpenApiAny(referencia.Schema.Type, attribute.Default, false);
                                    }

                                    this.openAPIDocument.Components.Parameters.Add(attr.Name, referencia);
                                }

                                OpenApiParameter param = this.generarParametro(attr, attrClassifierd.Name);

                                param.Reference = new OpenApiReference();

                                param.Reference.Id = attrClassifierd.Name;
                                param.Reference.Type = ReferenceType.Parameter;

                                this.eaUtils.printOut("Agregando parámetro como referencia: " + attrClassifierd.Name);

                                operation.Parameters.Add(param);
                            }
                            else
                            {
                                // va como parámetro.

                                OpenApiParameter param = this.generarParametro(attr, attrClassifierd.Name);

                                if (attr.Name != "schema")
                                {
                                    param.Name = attr.Name;
                                }

                                param.Schema = this.buildSchema(attr);

                                if (attribute.Default.Length > 0)
                                {
                                    param.Schema.Default = this.getOpenApiAny(param.Schema.Type, attribute.Default, false);
                                }

                                if (param.Schema.Description != null && param.Schema.Description.Length > 0 && param.Description != null
                                    && param.Schema.Description == param.Description)
                                {
                                    param.Description = null;
                                }

                                operation.Parameters.Add(param);

                                this.eaUtils.printOut("Agregando parámetro: " + param.Name);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    this.eaUtils.printOut("Error al intentar analizar el parámetro "+ attribute.Name);
                    this.eaUtils.printOut(e.ToString());
                }
            }
            else
            {
                OpenApiParameter param = this.generarParametro(attribute, null );

                param.Schema = this.buildSchema(attribute);

                if (attribute.Default.Length > 0)
                {
                    param.Schema.Default = this.getOpenApiAny(param.Schema.Type, attribute.Default, false);
                }

                operation.Parameters.Add(param);

                this.eaUtils.printOut("Agregando parámetro: " + attribute.Name);

            }
        }
        private OpenApiParameter generarParametro(EA.Attribute attribute, string paramName)
        {
            OpenApiParameter param = new OpenApiParameter();

            bool isArray = ! attribute.UpperBound.Trim().Equals("1");

            param.Name = paramName == null ? attribute.Name : paramName;

            string d = this.eaUtils.notes2Txt(attribute.Notes);

            if( d.Length > 0)
            {
                param.Description = d;
            }

            switch (this.eaUtils.taggedValuesUtils.get(attribute, Constantes.IN, "query").asString())
            {
                case "query":
                    param.In = ParameterLocation.Query;
                    break;
                case "path":
                    param.In = ParameterLocation.Path;
                    break;
                case "header":
                    param.In = ParameterLocation.Header;
                    break;
                case "cookie":
                    param.In = ParameterLocation.Cookie;
                    break;
            }

            TaggedValueWrapper w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.PARAMETER_REQUIRED, "true");

            param.Required = param.In == ParameterLocation.Path ? true : (bool)w.asBoolean();

            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.DEPRECATED, "false");
            if( w.value.Length > 0)
            {
                param.Deprecated = (bool)w.asBoolean();
            }

            return param;
        }
        private OpenApiSchema buildSchema(Element schemaClass)
        {
            OpenApiSchema schema = new OpenApiSchema();
            OpenApiSchema property;

            if (schemaClass.Stereotype == "enum")
            {
                schema.Type = "enum";

                List<IOpenApiAny> enumItems = new List<IOpenApiAny>();

                string type = schemaClass.Attributes.GetAt(0).Type;

                foreach (EA.Attribute a in schemaClass.Attributes)
                {
                    enumItems.Add(this.getOpenApiAny(type, a.Name, false));
                }
                schema.Enum = enumItems;

            }
            else if(schemaClass.Stereotype == "AdditionalProperties")
            {
                schema.Type = "object";

                OpenApiSchema additionalPropertySchema = new OpenApiSchema();

                schema.AdditionalProperties = additionalPropertySchema;

                foreach (EA.Attribute a in schemaClass.Attributes)
                {
                    additionalPropertySchema.Properties.Add(a.Name, this.buildSchema(a));
                }
            }
            else
            {
                foreach (EA.Attribute attribute in schemaClass.Attributes)
                {
                    property = this.buildSchema(attribute);

                    schema.Properties.Add(attribute.Name, property);
                }

                string[] requiredsGuidList = this.eaUtils.taggedValuesUtils.get(schemaClass, Constantes.REQUIRED, "").split();

                if (requiredsGuidList.Length > 0)
                {
                    System.Collections.Generic.SortedSet<string> requireds = new System.Collections.Generic.SortedSet<string>();

                    foreach(string guid in requiredsGuidList)
                    {
                        try
                        {
                            requireds.Add(this.eaUtils.repository.GetAttributeByGuid(guid).Name);
                        }
                        catch (Exception)
                        {
                            this.eaUtils.printOut("El elemento con el siguiente GUID no se encuentra: " + guid);
                        }
                    }
                    if (requireds.Count != 0)
                    {
                        schema.Required = requireds;
                    }
                }
            }

            return schema;
        }

        private OpenApiSchema buildSchema(EA.Attribute attribute)
        {
            OpenApiSchema schema = new OpenApiSchema();

            bool isArray = ! attribute.UpperBound.Trim().Equals("1");

            EA.Element element = null;

            if (attribute.ClassifierID > 0)
            {
                element = this.eaUtils.repository.GetElementByID(attribute.ClassifierID);

                schema.Type = isArray ? "array" : "object";

                if(element.Stereotype == "enum")
                {
                    schema.Type = "enum";

                    List<IOpenApiAny> enumItems = new List<IOpenApiAny>();

                    string type = element.Attributes.GetAt(0).Type;

                    foreach(EA.Attribute a in element.Attributes)
                    {
                        enumItems.Add( this.getOpenApiAny(type, a.Name, false));
                    }
                    schema.Enum = enumItems;
                }
                else
                {
                    if (isArray)
                    {                        
                        if (element.HasStereotype("AdditionalProperties")){

                            schema.Items = this.buildSchema(element);

                            schema.Type = "array";
                        }
                        else if (element.HasStereotype("AnonSchemaObject"))
                        {
                            OpenApiSchema anonSchema = this.buildSchema(element);

                            schema.Items = new OpenApiSchema();

                            schema.Items.Properties = anonSchema.Properties;

                            schema.Type = "array";
                        }
                        else
                        {
                            schema.Items = new OpenApiSchema();

                            schema.Items.Properties.Add(attribute.Name, this.buildSchema(element));
                        }
                    }
                    else
                    {
                        if (element.HasStereotype("SchemaObject"))
                        {
                            OpenApiSchema reference;
                            
                            if (this.openAPIDocument.Components.Schemas.ContainsKey(element.Name))
                            {
                                reference = this.openAPIDocument.Components.Schemas[element.Name];
                            }
                            else
                            {
                                reference = this.buildSchema(element);
                                reference.Type = "object";

                                this.openAPIDocument.Components.Schemas.Add(element.Name, reference);
                            }

                            schema.Type = "object";
                            schema.Reference = new OpenApiReference();

                            schema.Reference.Id = element.Name;
                            schema.Reference.Type = ReferenceType.Schema;

                            schema.Properties.Add(attribute.Name, schema);

                            this.eaUtils.printOut("Agregando schema como referencia: " + element.Name);
                        }
                        else
                        {
                            schema.Properties.Add(attribute.Name, this.buildSchema(element));
                        }
                    }
                }
            }
            else
            {
                string type = attribute.Type;
                string format = "";

                if( attribute.Type.Contains('.'))
                {
                    string[] typeSplit = attribute.Type.Split(separator: '.');
                    if(typeSplit.Length > 1)
                    {
                        type = typeSplit[0];
                        format = typeSplit[1];
                    }
                }

                schema.Type = isArray ? "array" : type;

                if (isArray)
                {
                    schema.Items = new OpenApiSchema();

                    schema.Items.Type = type;
                    if( format.Length> 0)
                    {
                        schema.Items.Format = format;
                    }

                    this.completarSchema(attribute, schema.Items, isArray);  
                }
                else
                {
                    if(format.Length > 0)
                    {
                        schema.Format = format;
                    }

                    this.completarSchema(attribute, schema, isArray);
                }
            }
                return schema;
        }
        private void completarSchema(EA.Attribute attribute, OpenApiSchema schema, bool isArray)
        {

            this.eaUtils.printOut("Buscando propiedades de "+ attribute.Name);
            //schema.AdditionalProperties
            //schema.AdditionalPropertiesAllowed
            //schema.AllOf
            //schema.AnyOf
            schema.Deprecated = (bool)this.eaUtils.taggedValuesUtils.get(attribute, Constantes.DEPRECATED, "false").asBoolean();

            string d = this.eaUtils.notes2Txt(attribute.Notes);

            if ( d.Length > 0)
            {
                schema.Description = d;
            }

            string discriminatorGuid = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.DISCRIMINATOR, "").asString();
            if( discriminatorGuid.Length > 0)
            {
                EA.Element discriminatorClass = this.eaUtils.repository.GetElementByGuid(discriminatorGuid);
                EA.Attribute propertyName = discriminatorClass.Attributes.GetByName("propertyName");

                if ( propertyName.Default != "")
                {
                    schema.Discriminator = new OpenApiDiscriminator();
                    schema.Discriminator.PropertyName = propertyName.Default;

                    if(discriminatorClass.Attributes.Count > 1)
                    {
                        schema.Discriminator.Mapping = new Dictionary<string, string>();

                        foreach ( EA.Attribute a in discriminatorClass.Attributes)
                        {
                            if(a.Stereotype != "openapi.mapping" || a.ClassifierID < 1)
                            {
                                continue;
                            }
                            EA.Element e = this.eaUtils.repository.GetElementByID(a.ClassifierID);
                            schema.Discriminator.Mapping.Add(a.Name, e.Name);
                        }
                    }
                }
            }

            //schema.Enum
            string example = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.EXAMPLE, "").asString();
            if (example.Length > 0)
            {
                schema.Example = this.getOpenApiAny(attribute.Type, example, isArray);
            }

            TaggedValueWrapper w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.EXCLUSIVE_MAXIMUM, "");

            if( w.value.Length > 0)
            {
                schema.ExclusiveMaximum = w.asBoolean();
            }

            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.EXCLUSIVE_MINIMUM, "");
            if(w.value.Length > 0)
            {
                schema.ExclusiveMinimum = w.asBoolean();
            }
            //schema.Extensions
            //schema.ExternalDocs

            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MAXIMUM, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.Maximum = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MAXIMUM + " atributo: " + attribute.Name);
                }
            }

            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MAX_ITEMS, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MaxItems = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MAX_ITEMS + " atributo: " + attribute.Name);
                }
            }

            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MAX_LENGTH, "");
            if( w.value.Length > 0)
            {
                try
                {
                    schema.MaxLength = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde "+ Constantes.MAX_LENGTH +" atributo: "+ attribute.Name);
                }
            }
            //schema.
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MAX_PROPERTIES, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MaxProperties = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MAX_PROPERTIES + " atributo: " + attribute.Name);
                }
            }
            //schema.
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MINIMUN, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.Minimum = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MINIMUN + " atributo: " + attribute.Name);
                }
            }
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MIN_ITEMS, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MinItems = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MIN_ITEMS + " atributo: " + attribute.Name);
                }
            }
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MIN_LENGTH, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MinLength = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MIN_LENGTH + " atributo: " + attribute.Name);
                }
            }
            //schema.
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MIN_PROPERTIES, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MinProperties = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MIN_PROPERTIES + " atributo: " + attribute.Name);
                }
            }
            //schema.
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.MULTIPLE_OF, "");
            if (w.value.Length > 0)
            {
                try
                {
                    schema.MultipleOf = w.asInt();
                }
                catch (Exception)
                {
                    this.eaUtils.printOut("No se pudo obtener un entero desde " + Constantes.MIN_PROPERTIES + " atributo: " + attribute.Name);
                }
            }
            //schema.Not
            schema.Nullable  = (bool)this.eaUtils.taggedValuesUtils.get(attribute, Constantes.NULLABLE, "true").asBoolean();
            //schema.OneOf
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.PATTERN, "");
            if(w.value.Length > 0)
            {
                schema.Pattern = w.asString();
            }
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.READ_ONLY, "");
            if(w.value.Length > 0)
            {
                schema.ReadOnly = (bool)w.asBoolean();
            }

            //schema.Title sale de la primera línea de las notas.
            //schema.UniqueItems
            //schema.UnresolvedReference
            w = this.eaUtils.taggedValuesUtils.get(attribute, Constantes.WRITE_ONLY, "");
            if (w.value.Length > 0)
            {
                schema.WriteOnly = (bool)w.asBoolean();
            }

            //schema.Xml
        }
       private IOpenApiAny getOpenApiAny(string attrType, string value, bool isArray)
       {
            IOpenApiAny any = null;

            if (attrType.Equals("string"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiString(value)};
                }
                else
                {
                    any = new OpenApiString(value);
                }
            }
            else if (attrType.Equals("integer") || attrType.Equals("integer.int32"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiInteger(int.Parse(value)) };
                }
                else
                {
                    any = new OpenApiInteger(int.Parse(value));
                }
            }
            else if (attrType.Equals("integer.int64"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiLong(long.Parse(value)) };
                }
                else
                {
                    any = new OpenApiLong(long.Parse(value));
                }
            }
            else if (attrType.Equals("number") || attrType.Equals("number.double"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiDouble(double.Parse(value)) };
                }
                else
                {
                    any = new OpenApiDouble(double.Parse(value));
                }
            }
            else if (attrType.Equals("number.float"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiFloat(float.Parse(value)) };
                }
                else
                {
                    any = new OpenApiFloat(float.Parse(value));
                }
            }
            /*
            else if ( attrType.Equals("string.binary"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiDouble(double.Parse(value)) };
                }
                else
                {
                    any = new OpenApiDouble(double.Parse(value));
                }
            }
            */
            else if (attrType.Equals("string.date"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiDate(DateTime.Parse(value)) };
                }
                else
                {
                    any = new OpenApiDate(DateTime.Parse(value));
                }
            }
            else if (attrType.Equals("string.date-time"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiDateTime(DateTime.Parse(value)) };
                }
                else
                {
                    any = new OpenApiDateTime(DateTime.Parse(value));
                }
            }
            else if (attrType.Equals("string.password"))
            {
                if (isArray)
                {
                    any = new OpenApiArray { new OpenApiPassword(value) };
                }
                else
                {
                    any = new OpenApiPassword(value);
                }
            }
            return any;
        }
    }
}
