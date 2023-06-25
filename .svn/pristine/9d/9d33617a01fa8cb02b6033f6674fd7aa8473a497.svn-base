using EA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

namespace RestFul
{
    class JSONUtils
    {
        private EAUtils.EAUtils eaUtils;

        public JSONUtils(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public Object fromString( string json )
        {
            StringReader sreader = new StringReader(json);
            JsonReader reader = new JsonTextReader(sreader);

            reader.Read();

            return reader.Value;
        }
        /**
* rootElement tiene que ser un Message.
*/
        public string toString( Element rootElement )
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Newtonsoft.Json.Formatting.Indented;

            writer.WriteStartObject();

            eaUtils.printOut("{");

            toString(rootElement, writer);

            /*
            writer.WritePropertyName("Factura");
                writer.WriteStartObject();
                    writer.WritePropertyName("cuitEmisor");
                    writer.WriteValue("");
                    writer.WritePropertyName("items");
                        writer.WriteStartArray();
                            writer.WriteStartObject();
                            writer.WritePropertyName("item");
                                writer.WriteStartObject();
                                    writer.WritePropertyName("item_1");
                                    writer.WriteValue("nada");
                                writer.WriteEndObject();
                            writer.WriteEndObject();
                            writer.WriteEndArray();
                writer.WriteEndObject();
            */

            eaUtils.printOut("}");
            writer.WriteEnd();

            /*
            writer.WriteStartObject();
            writer.WritePropertyName("CPU");
            writer.WriteValue("Intel");
            writer.WritePropertyName("PSU");
            writer.WriteValue("500W");
            writer.WritePropertyName("Drives");
            writer.WriteStartArray();
            writer.WriteValue("DVD read/writer");
            writer.WriteComment("(broken)");
            writer.WriteValue("500 gigabyte hard drive");
            writer.WriteValue("200 gigabype hard drive");
            writer.WriteEnd();
            writer.WriteEndObject();
            */

            //toXml(sw.ToString());
            return sw.ToString();
        }

        private void toString(Element element, JsonWriter writer)
        {
            // pueden ser atributos o una lista de objectos
            // la diferencia entre ambos es si el candidato a atributo tiene "hijos", si lo tiene entonces es un objeto.
            List<ArrayList> elementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(true, element, EAUtils.ConnectorUtils.CONNECTOR__NESTING, null, null, null);

            Boolean asObject = (elementsAndConnectors.Count != 0 || element.Attributes.Count != 0);

            if( ! asObject )
            {
                eaUtils.printOut( element.Name );
                writer.WritePropertyName(element.Name);
                writer.WriteValue( "" );
            }
            else
            {
                eaUtils.printOut(element.Name + ":");
                writer.WritePropertyName(element.Name);

                //writeValue(element, writer);

                //writer.WriteComment(element.Notes);

                eaUtils.printOut("{");
                writer.WriteStartObject();

                if (element.Attributes.Count != 0)
                {
                    foreach (EA.Attribute attribute in element.Attributes)
                    {
                        eaUtils.printOut(attribute.Name +":" );

                        writer.WritePropertyName(attribute.Name);
                        //writer.WriteComment(attribute.Alias);
                        writer.WriteValue(attribute.Default);
                        writer.WriteComment(attribute.Notes);
                    }
                }

                // si este elemento tiene una relacion de nesting hay que determinar la cardinalidad.
                Element childElement;
                Connector connector;

                foreach (ArrayList line in elementsAndConnectors)
                {
                    childElement = (Element)line[EAUtils.EAUtils.ELEMENT_AND_CONNECTOR__ELEMENT_POSITION];
                    connector    = (Connector)line[EAUtils.EAUtils.ELEMENT_AND_CONNECTOR__CONNECTOR_POSITION];

                    //MessageBox.Show( "Client "+ repository.GetElementByID( connector.ClientID ).Name +" supplier "+ repository.GetElementByID(connector.SupplierID).Name);
                    EAUtils.ConnectorUtils.Cardinality cardinality = eaUtils.connectorUtils.getCardinality(connector.SupplierEnd);

                    Boolean childAsObject = false;

                    if (cardinality.isCollection())
                    {
                        

                        eaUtils.printOut(connector.SupplierEnd.Role + ":");
                        if(connector.SupplierEnd.Role.Length == 0)
                        {
                            writer.WritePropertyName("anonima");
                        }
                        else
                        {
                            writer.WritePropertyName(connector.SupplierEnd.Role);
                        }
                        
                        eaUtils.printOut("[");
                        writer.WriteStartArray();

                        List<ArrayList> nextElementsAndConnectors = eaUtils.connectorUtils.getFromConnectorFilter(true, childElement, EAUtils.ConnectorUtils.CONNECTOR__NESTING, null, null, null);

                        childAsObject = (nextElementsAndConnectors.Count != 0);

                        if (childAsObject)
                        {
                            writer.WriteStartObject();
                        }

                    }

                    toString(childElement, writer );

                    if (cardinality.isCollection())
                    {
                        if (childAsObject)
                        {
                            writer.WriteEndObject();
                        }
                        eaUtils.printOut("]");
                        writer.WriteEndArray();
                    }
                }
                eaUtils.printOut("}");
                writer.WriteEndObject();
            }
        }

        public string toXmlString( string json )
        {
            //return "";
            eaUtils.printOut(json);
            return toXml(json).ToString();
        }

        public System.Xml.Linq.XNode toXml( string json )
        {
            //string json2xml = @"{"?xml": {'@version': '1.0','@standalone': 'no'},'root':" + json + "}";
            //eaUtils.printOut(json2xml);

            /*
            string json2 = @"{
 '@Id': 1
  'Email': 'egenini@gmail.com',
 'Active': true,
  'CreatedDate': '2013-01-20T00:00:00Z',
  'Roles': [
    'User',
    'Admin'
  ],
  'Team': {
    '@Id': 2,
    'Name': 'Software Developers',
   'Description': 'Creators of fine software products and services.'
  }
}";
*/
            System.Xml.Linq.XNode doc = JsonConvert.DeserializeXNode(json, "Root");
            
            eaUtils.printOut(doc.ToString());
            return doc;
        }

        /// <summary>
        /// Escribe la propiedad y el valor si el valor no es null o no está vacío
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>Verdadero si escribió la propiedad y su valor</returns>
        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, string value)
        {
            if (value != null && value.Length != 0)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
            return (value != null && value.Length != 0);
        }

        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, List<string> values)
        {
            if (values != null && values.Count != 0)
            {
                writer.WritePropertyName(name);

                writer.WriteStartArray();

                foreach ( string value in values )
                {
                    writer.WriteValue(value);
                }
                writer.WriteEndArray();
            }
            return (values != null && values.Count != 0);
        }

        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, Nullable<int> value)
        {
            if (value != null )
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
            return (value != null );
        }
        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, List<Object> values, string objectType )
        {
            if (values != null && values.Count != 0)
            {
                writer.WritePropertyName(name);
                writer.WriteStartObject();
                foreach ( var value in values )
                {
                    
                    writer.WriteValue(value);
                }
                writer.WriteEnd();
            }
            return (values != null && values.Count != 0);
        }
        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, Nullable<Boolean>  value)
        {
            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
            return (value != null);
        }
        
        public Boolean writePorpertyIfNotNullOrEmpty(JsonWriter writer, string name, object value)
        {
            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
            return (value != null);
        }
        
    }


}
