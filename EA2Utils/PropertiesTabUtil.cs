using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EAUtils
{
    public class PropertiesTabUtil
    {
        public const string TYPE__TEXT     = "text";
        public const string TYPE__COMBOBOX = "combobox";
        public const string TYPE__DATE     = "date";
        public const string TYPE__CHECKBOX = "checkbox";
        public const string TYPE__SPIN     = "spin";
        public const string TYPE__INT      = "int";
        public const string TYPE__DOUBLE   = "double";
        public const string TYPE__MEMO     = "memo";

        const string root = "<?xml version=\"1.0\"?><properties></properties>";
        XmlDocument xmlDOM = new XmlDocument();
        XmlElement currentGroup = null;
        int propertyId = 0;
        XmlElement currenProperty= null;

        public PropertiesTabUtil() 
        {
            xmlDOM.LoadXml(root);
        }

        public PropertiesTabUtil addGroup(string name)
        {
            currentGroup = xmlDOM.CreateElement(name);

            xmlDOM.AppendChild(currentGroup);

            return this;
        }
        public PropertiesTabUtil addProperty(string name, string descripcion)
        {
            return addProperty(name, descripcion, null);
        }
        public PropertiesTabUtil addProperty( string name, string descripcion, string value)
        {
            currenProperty = xmlDOM.CreateElement("property");

            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("id");

            xmlAttr.Value = propertyId.ToString();

            currenProperty.Attributes.Append(xmlAttr);

            propertyId++;

            XmlElement child = xmlDOM.CreateElement("name");

            child.Value = name;

            currenProperty.AppendChild(child);

            child = xmlDOM.CreateElement("description");

            child.Value = descripcion;

            currenProperty.AppendChild(child);


            child = xmlDOM.CreateElement("value");

            if (value != null)
            {
                child.Value = value;
            }

            currenProperty.AppendChild(child);

            return this;
        }

        public PropertiesTabUtil setType(string type)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("type");

            xmlAttr.Value = type;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }
        public PropertiesTabUtil setDefault(string value)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("default");

            xmlAttr.Value = value;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }
        public PropertiesTabUtil setReadonly(string value)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("readonly");

            xmlAttr.Value = value;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }
        public PropertiesTabUtil setShowcheckbox(string value)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("showcheckbox");

            xmlAttr.Value = value;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }

        public PropertiesTabUtil setMin(string value)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("min");

            xmlAttr.Value = value;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }
        public PropertiesTabUtil setMax(string value)
        {
            XmlAttribute xmlAttr = xmlDOM.CreateAttribute("max");

            xmlAttr.Value = value;

            currenProperty.Attributes.Append(xmlAttr);

            return this;
        }

        public PropertiesTabUtil addValueList( params string[] valueList)
        {
            XmlElement valueListElement = xmlDOM.CreateElement("valuelist");

            currenProperty.AppendChild(valueListElement);

            XmlElement item;

            foreach ( string value in valueList)
            {
                item = xmlDOM.CreateElement("item");

                item.Value = value;

                valueListElement.AppendChild(valueListElement);

            }

            return this;
        }

        public string build()
        {
            return xmlDOM.ToString();
        }
    }
}
