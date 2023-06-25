using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using EAUtils;
using EA;
using Microsoft.OpenApi.Expressions;
using System.Security.Policy;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using EAUtils.model;
using System.CodeDom;

namespace OpenAPI.parser
{
    public class Sincronizar
    {
        OpenApiDocument openApiDocument = null;
        EAUtils.EAUtils eaUtils= null;
        EstructuraSolucion estructuraSolucion = null;

        Dictionary<string, Dictionary<string, Element>> components = null;

        public Sincronizar(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }
        public void sincronizar()
        {
            this.parse();

            if( openApiDocument != null)
            {
                if( establecerPaquete() )
                {
                    DefaultContractResolver contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                    this.estructuraSolucion.Info.Notes = openApiDocument.Info.Description;
                    this.estructuraSolucion.Info.Name = openApiDocument.Info.Title;
                    this.estructuraSolucion.Info.Update();

                    this.estructuraSolucion.Contact.Name = openApiDocument.Info.Contact.Name;
                    this.eaUtils.taggedValuesUtils.set(this.estructuraSolucion.Contact, "email", openApiDocument.Info.Contact.Email);

                    int s = 0;
                    Element element= null;
                    EA.Attribute attribute= null;

                    foreach( OpenApiServer server in openApiDocument.Servers)
                    {
                        if( s < estructuraSolucion.Servers.Count)
                        {
                            element = estructuraSolucion.Servers[s];
                            element.Name = server.Url;
                        }
                        else
                        {
                            element = estructuraSolucion.Principal.Elements.AddNew(server.Url, "Class");
                            element.StereotypeEx = "OpenAPI::Server";
                        }
                        
                        element.Update();

                        foreach( KeyValuePair<string, OpenApiServerVariable> keyValuePair in server.Variables)
                        {
                            attribute = element.Attributes.AddNew(keyValuePair.Key, "");
                            
                            attribute.Notes = keyValuePair.Value.Description;
                            attribute.Default = keyValuePair.Value.Default;

                            attribute.Update();
                        }
                        
                        s++;
                    }

                    Element callback = null; ;

                    if( openApiDocument.Components.Callbacks.Count != 0)
                    {
                        callback = estructuraSolucion.Definicion.Elements.AddNew("Callbacks", "Class");

                        callback.Stereotype = "CallbackDefinition";

                        callback.Update();
                    }

                    OpenApiCallback openApiCallback;
                    OpenApiPathItem openApiPathItem;
                    RuntimeExpression runtimeExpression;
                    OperationType operationType;
                    OpenApiOperation openApiOperation;

                    components.Add("callbacks"      , new Dictionary<string, Element>(openApiDocument.Components.Callbacks.Count));
                    components.Add("examples"       , new Dictionary<string, Element>(openApiDocument.Components.Examples.Count));
                    components.Add("extensions"     , new Dictionary<string, Element>(openApiDocument.Components.Extensions.Count));
                    components.Add("headers"        , new Dictionary<string, Element>(openApiDocument.Components.Headers.Count));
                    components.Add("links"          , new Dictionary<string, Element>(openApiDocument.Components.Links.Count));
                    components.Add("parameters"     , new Dictionary<string, Element>(openApiDocument.Components.Parameters.Count));
                    components.Add("requestBodies"  , new Dictionary<string, Element>(openApiDocument.Components.RequestBodies.Count));
                    components.Add("responses"      , new Dictionary<string, Element>(openApiDocument.Components.Responses.Count));
                    components.Add("schemas"        , new Dictionary<string, Element>(openApiDocument.Components.Schemas.Count));
                    components.Add("securitySchemes", new Dictionary<string, Element>(openApiDocument.Components.SecuritySchemes.Count));

                    foreach (KeyValuePair<string, OpenApiSchema> c in openApiDocument.Components.Schemas)
                    {
                        if( !components["schemas"].ContainsKey(c.Key) ){

                            components["schemas"].Add(c.Key, this.agregarSchemaObject(c.Key, c.Value));

                            foreach (KeyValuePair<string, OpenApiSchema> a in c.Value.Properties)
                            {
                                agregarAtributo(element, a.Key, a.Value);
                            }
                        }
                    }

                    foreach (KeyValuePair<string, OpenApiHeader> c in openApiDocument.Components.Headers)
                    {
                        if (!components["headers"].ContainsKey(c.Key))
                        {

                            components["headers"].Add(c.Key, this.agregarHeaderComponent(c.Key, c.Value));
                        }
                    }

                    foreach ( KeyValuePair<string, OpenApiCallback> c in openApiDocument.Components.Callbacks)
                    {
                        openApiCallback = c.Value;

                        foreach( KeyValuePair<RuntimeExpression, OpenApiPathItem> pi in openApiCallback.PathItems)
                        {
                            runtimeExpression = pi.Key;
                            openApiPathItem = pi.Value;

                            //runtimeExpression.Expression;

                            foreach(KeyValuePair<OperationType, OpenApiOperation> o in openApiPathItem.Operations)                                
                            {
                                operationType= o.Key;
                                openApiOperation = o.Value;

                                //operationType.
                            }
                        }
                        //callback.Methods.AddNew( )
                    }

                    foreach (KeyValuePair<string, OpenApiResponse> c in openApiDocument.Components.Responses)
                    {

                    }
                    foreach (KeyValuePair<string, OpenApiRequestBody> c in openApiDocument.Components.RequestBodies)
                    {

                    }
                    foreach ( KeyValuePair<string, OpenApiPathItem> p in openApiDocument.Paths )
                    {

                        if (this.estructuraSolucion.Definicion.Elements.Count != 0)
                        {
                            if (estructuraSolucion.oneOperationXInterface)
                            {

                            }
                        }
                        else
                        {
                            estructuraSolucion.ServiceComponent = estructuraSolucion.Definicion.Elements.AddNew(p.Key, "Interface");
                            estructuraSolucion.ServiceComponent.Update();

                        }
                        EA.Element e = agregarPath(p.Key, p.Value);
                    }
                    
                    //openApiDocument.SecurityRequirements;
                }
            }
        }

        private Element agregarPath(string key, OpenApiPathItem value)
        {
            value.Description
            value.Operations
        }

        private Element agregarHeaderComponent(string key, OpenApiHeader value)
        {
            Element element = estructuraSolucion.Model.Elements.AddNew(key, "Class");
            element.Stereotype = "ReusableParameter";
            
            //value.AllowEmptyValue;
            //value.AllowReserved;
            //value.Content;

            element.Notes = value.Description;

            //value.Examples;
            //value.Explode;
            //value.Extensions;
            //value.Reference;
            //value.Style;
            //value.UnresolvedReference;

            this.eaUtils.taggedValuesUtils.set(element, "in", "header");
            this.eaUtils.taggedValuesUtils.set(element, "openapiparam.required", value.Required.ToString());
            this.eaUtils.taggedValuesUtils.set(element, "openapiparam.deprecated", value.Deprecated.ToString());
            //this.eaUtils.taggedValuesUtils.set(element, "openapiparam.example", value.Example);


            element.Update();

            EA.Attribute a = agregarAtributo(element, "schema", value.Schema);

            a.Stereotype = "";

            return element;
        }

        public EA.Attribute agregarAtributo(Element element, string nombre, OpenApiSchema schema)
        {
            EA.Attribute attribute;

            attribute = element.Attributes.AddNew(nombre, "");
            attribute.Stereotype = "openapi.attribute";
            attribute.Update();
            completarSchema(attribute, schema);

            return attribute;
        }
        private void completarSchema(EA.Attribute attribute, OpenApiSchema value)
        {
            if( value.AdditionalProperties != null)
            {

            }
            if( value.AdditionalPropertiesAllowed)
            {

            }
            if( value.AllOf != null)
            {
                /*
                 * No encuentro como distinguir uno de otro
                foreach( OpenApiSchema os in value.AllOf)
                {
                    
                }
                */
            }
            if( value.AnyOf != null)
            {

            }
            if( value.Default != null)
            {
                attribute.Default = value.Default.AnyType.ToString();
            }
            if( value.Deprecated )
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.DEPRECATED, "true");
            }
            if (value.Discriminator != null)
            {

            }
            /*
            if ( value.Enum != null)
            {
                
            }
            */
            if (value.Example != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.EXAMPLE, value.Example.ToString());

            }
            if ( value.ExclusiveMaximum != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.EXCLUSIVE_MAXIMUM, value.ExclusiveMaximum.ToString());
            }
            if (value.ExclusiveMinimum != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.EXCLUSIVE_MINIMUM, value.ExclusiveMinimum.ToString());
            }
            if (value.Extensions != null)
            {

            }
            if (value.ExternalDocs != null)
            {

            }
            /* va con type
            if (value.Format != null)
            {
                
            }
            */
            if (value.Items != null)
            {

            }
            if (value.Maximum != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MAXIMUM, value.Maximum.ToString());

            }
            if (value.MaxItems != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MAX_ITEMS, value.MaxItems.ToString());

            }
            if (value.MaxLength != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MAX_LENGTH, value.MaxLength.ToString());

            }
            if (value.MaxProperties != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MAX_PROPERTIES, value.MaxProperties.ToString());

            }
            if (value.Minimum != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MINIMUN, value.Minimum.ToString());

            }
            if (value.MinItems != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MAX_ITEMS, value.MaxItems.ToString());

            }
            if (value.MinLength != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MIN_LENGTH, value.MinLength.ToString());

            }
            if (value.MinProperties != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MIN_PROPERTIES, value.MinProperties.ToString());

            }
            if (value.MultipleOf != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.MULTIPLE_OF, value.MultipleOf.ToString());

            }
            if (value.Not != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.NOT, value.Not.ToString());

            }
            if (value.Nullable)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.NULLABLE, "true");

            }
            if (value.OneOf != null)
            {

            }
            if (value.Pattern != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.PATTERN, value.Pattern);

            }
            if (value.Properties != null)
            {

            }
            if (value.ReadOnly)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.READ_ONLY, "true");

            }
            if (value.Reference != null)
            {

            }
            if (value.Required != null)
            {
                if( attribute.Stereotype == "openapi.attribute")
                {
                    // en este caso el required es una lista en la clase.
                }
            }
            if (value.Title != null)
            {
                if( attribute.Notes.Equals(""))
                {
                    attribute.Notes = value.Title;
                }
                else
                {
                    attribute.Notes = value.Title +"\r\n"+ value.Description;
                }
            }
            if (value.Type != null)
            {
                resolverType(attribute, value);

            }
            if (value.UniqueItems != null)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.UNIQUE_ITEMS, value.UniqueItems.ToString());

            }
            if (value.WriteOnly)
            {
                this.eaUtils.taggedValuesUtils.set(attribute, Constantes.WRITE_ONLY, value.WriteOnly.ToString());

            }
            if (value.Xml != null)
            {

            }
        }

        private void resolverType(EA.Attribute attribute, OpenApiSchema value)
        {
            switch (value.Type)
            {
                case "object":

                    if (value.Properties.Count > 0)
                    {
                        EA.Element element = estructuraSolucion.Definicion.Elements.AddNew(attribute.Name.Substring(0, 1).ToUpper() + attribute.Name.Substring(1), "class");

                        element.Stereotype = "AnonSchemaObject";
                        element.Update();

                        components["shemas"].Add(element.Name, element);

                        attribute.ClassifierID = element.ElementID;

                        foreach (KeyValuePair<string, OpenApiSchema> prop in value.Properties)
                        {
                            agregarAtributo(element, prop.Key, prop.Value);
                        }
                    }
                    else if (value.Reference != null)
                    {
                        if (value.Reference.IsLocal)
                        {
                            if (components["schemas"].ContainsKey(value.Reference.Id))
                            {
                                attribute.ClassifierID = components["shemas"][value.Reference.Id].ElementID;
                            }
                            else
                            {
                                EA.Element element = estructuraSolucion.Definicion.Elements.AddNew(value.Reference.Id, "class");

                                element.Stereotype = "SchemaObject";
                                element.Update();

                                components["shemas"].Add(value.Reference.Id, element);

                                attribute.ClassifierID = element.ElementID;

                                foreach (KeyValuePair<string, OpenApiSchema> prop in openApiDocument.Components.Schemas[value.Reference.Id].Properties)
                                {
                                    agregarAtributo(element, prop.Key, prop.Value);
                                }
                            }
                        }
                    }

                    break;
                case "array":

                    switch (value.Items.Type)
                    {
                        case "object":

                            EA.Element element = estructuraSolucion.Definicion.Elements.AddNew(attribute.Name.Substring(0, 1).ToUpper() + attribute.Name.Substring(1), "class");

                            element.Stereotype = "AnonSchemaObject";
                            element.Update();

                            foreach (KeyValuePair<string, OpenApiSchema> prop in value.Items.Properties)
                            {
                                agregarAtributo(element, prop.Key, prop.Value);
                            }

                            break;
                        case "array":
                            resolverType(attribute, value.Items.Items);
                            break;
                    }

                    break;
                case "enum":
                    EA.Element enumElement = null;
                    enumElement = estructuraSolucion.Definicion.Elements.AddNew(attribute.Name.Substring(0, 1).ToUpper() + attribute.Name.Substring(1) + "Enum", "Enumeration");

                    enumElement.Stereotype = "OpenAPI::enum";

                    enumElement.Update();

                    foreach (IOpenApiAny openApiAny in value.Enum)
                    {
                        enumElement.Attributes.AddNew(openApiAny.ToString(), "").Update();
                    }

                    attribute.ClassifierID = enumElement.ElementID;
                    attribute.Update();
                    break;

                default:
                    attribute.Type = value.Type;
                    if (value.Format != null)
                    {
                        attribute.Type = value.Type + "." + value.Format;
                    }
                    break;
            }
        }

        private EA.Element agregarSchemaObject( string nombre, OpenApiSchema schema)
        {
            EA.Element element = estructuraSolucion.Model.Elements.AddNew(nombre, "Class");

            element.Stereotype = "SchemaObject";

            completarSchema(element,schema);

            element.Update();

            return element;
        }
        private void completarSchema(Element element, OpenApiSchema value)
        {
            if( value.MaxProperties != null )
            {
                this.eaUtils.taggedValuesUtils.set(element, Constantes.MAX_PROPERTIES ,value.MaxProperties.ToString());
            }
            if( value.MinProperties != null )
            {
                this.eaUtils.taggedValuesUtils.set(element, Constantes.MIN_PROPERTIES,value.MinProperties.ToString());
            }
        }

        private bool establecerPaquete()
        {
            this.estructuraSolucion = new EstructuraSolucion();
            
            return this.estructuraSolucion.establecer(eaUtils);
        }
        private void parse()
        {
            // https://github.com/microsoft/OpenAPI.NET

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "JSON (*.json)|*.json|YAML (*.yaml)|*.yaml";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var stream = System.IO.File.Open(openFileDialog1.FileName, FileMode.Open);

                OpenApiDiagnostic openDiagnostic = new OpenApiDiagnostic();

                openApiDocument = new OpenApiStreamReader().Read(stream, out openDiagnostic);

                //var outputString = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

            }
        }
    }
}
