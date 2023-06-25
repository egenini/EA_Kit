using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace EAUtils
{
    public class XmlUtils
    {
        private const string searchField = "<Field name=\"__fieldname__\" />";
        private const string addRow_AddXMLSearchFieldTemplate = "	AddXMLSearchField( xmlDOM, row, \"__fieldname__\", param );\r\n";
        private string addRowTemplate = "\r\nfunction AddRow( parameters )\r\n" +
            "{\r\n" +
            "	var row = xmlDOM.createElement( \"Row\" );\r\n" +
            "#AddXMLSearchFieldTemplate#" +
            "	rowsNode.appendChild( row );\r\n" +
            "}\r\n";

        public XmlDocument xmlDOM;

        public void loadFromFile(string fullFileName)
        {
            xmlDOM = new XmlDocument();

            xmlDOM.Load(fullFileName);
        }
        
        public XmlDocument createDOM()
        {
            xmlDOM = new XmlDocument();
            return this.xmlDOM;
        }

        public XmlNode createSearchDOM(List<Dictionary<string, string>> xmlFieldsDef, Boolean createCode)
        {
            XmlNode rowsNode = null;

            var fields = "";
            var parameters = "";
            var addXMLSearchFieldLine = "";

            createDOM();

            var SEARCH_SPECIFICATION = "<ReportViewData>" +
                                        "<Fields>" +
                                        "@fields@" +
                                        "</Fields>" +
                                        "<Rows/>" +
                                    "</ReportViewData>";
            // lista de diccionarios
            for (short i = 0; i < xmlFieldsDef.Count; i++)
            {
                fields += searchField.Replace("__fieldname__", xmlFieldsDef[i]["field"]);

                if (createCode)
                {
                    if (xmlFieldsDef[i].ContainsKey("param"))
                    {
                        addXMLSearchFieldLine += addRow_AddXMLSearchFieldTemplate.Replace("__fieldname__", xmlFieldsDef[i]["field"]).Replace("param", xmlFieldsDef[i]["literal"]);
                    }
                    else
                    {
                        parameters += xmlFieldsDef[i]["param"] + ",";

                        addXMLSearchFieldLine += addRow_AddXMLSearchFieldTemplate.Replace("__fieldname__", xmlFieldsDef[i]["field"]).Replace("param", xmlFieldsDef[i]["param"]);
                    }
                }
            }

            SEARCH_SPECIFICATION = SEARCH_SPECIFICATION.Replace("@fields@", fields);

            try
            {
                xmlDOM.LoadXml(SEARCH_SPECIFICATION);
                rowsNode = xmlDOM.SelectSingleNode("//ReportViewData//Rows");
            }
            catch (XmlException) { }

            if (createCode)
            {
                addRowTemplate = addRowTemplate.Replace("parameters", parameters.Substring(0, parameters.Length - 1));
            }
            return rowsNode;
        }
        /*
 * Adds an Element to the DOM called Field which makes up the Row data for the Model Search window.
 * <Field name "" value ""/>
 */
        public void AddXMLSearchField( XmlNode row, string name, string value )
        {
            var fieldNode = xmlDOM.CreateElement("Field");

            // Create first attribute for the name
            var nameAttribute = xmlDOM.CreateAttribute("name");
            nameAttribute.Value = name;
            fieldNode.Attributes.SetNamedItem(nameAttribute);

            // Create second attribute for the value
            var valueAttribute = xmlDOM.CreateAttribute("value");
            valueAttribute.Value = value;
            fieldNode.Attributes.SetNamedItem(valueAttribute);

            // Append the fieldNode
            row.AppendChild(fieldNode);
        }

        public XmlNode createReportFragmentDOM()
        {
            createDOM();

            var node = xmlDOM.CreateProcessingInstruction("xml", "version='1.0' encoding='ISO-8859-1'");
            xmlDOM.AppendChild(node);

            var xmlRoot = xmlDOM.CreateElement("EADATA");
            xmlDOM.AppendChild(xmlRoot);

            var xmlDataSet = xmlDOM.CreateElement("Dataset_0");
            xmlRoot.AppendChild(xmlDataSet);

            var xmlData = xmlDOM.CreateElement("Data");
            xmlDataSet.AppendChild(xmlData);

            var xmlRow = xmlDOM.CreateElement("Row");
            xmlData.AppendChild(xmlRow);

            return xmlRow;
        }
        public XmlElement createDOMElement(string elementName, XmlNode xmlRow)
        {
            return createDOMElement( elementName, null, xmlRow);
        }

        public XmlElement createDOMElement( string elementName, string textValue, XmlNode xmlRow )
        {
            var xmlElement = xmlDOM.CreateElement(elementName);

            if (textValue != null)
            {
                xmlElement.InnerText = textValue;
            }

            if (xmlRow != null)
            {
                xmlRow.AppendChild(xmlElement);
            }
            return xmlElement;
        }

        public XmlAttribute createAttribute(string name, string value)
        {
            return createAttribute(name, value);
        }

        public XmlAttribute createAttribute(string name, string value, XmlNode xmlElement)
        {
            XmlAttribute xmlAttribute = xmlDOM.CreateAttribute(name);

            if (value != null)
            {
                xmlAttribute.Value = value;
            }

            if (xmlElement != null)
            {
                xmlElement.Attributes.Append(xmlAttribute);
            }
            return xmlAttribute;
        }

        public List<string> getNodeTextList(XmlDocument xmlDOM, string nodePath )
        {
            var nodeList = xmlDOM.DocumentElement.SelectNodes(nodePath);
            List<string> textArray = new List<string>();

            for (var i = 0; i < nodeList.Count; i++)
            {
                var currentNode = nodeList.Item(i);
                if( currentNode.Value == null)
                {
                    textArray.Add(currentNode.InnerText);
                }
                else
                {
                    textArray.Add(currentNode.Value);
                }
            }

            return textArray;
        }

        public string xml2String()
        {
            return this.xmlDOM.OuterXml;
        }
    }
}
