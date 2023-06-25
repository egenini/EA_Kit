using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MDGBuilder.mdg
{
    public class DiagramPatternXmlUtil
    {
        XmlUtils xmlPattern = new XmlUtils();
        System.Xml.XmlNamespaceManager xmlmanager;
        XmlElement patternTag;
        string patternName;

        public void build(string file, Package patternPackage)
        {
            // leer el file
            // tomar el nodo que está dentro del paquete así podemos poner este en su lugar.
            // agregar el tag 
            xmlPattern.loadFromFile(file);
            xmlmanager = new XmlNamespaceManager(xmlPattern.xmlDOM.NameTable);
            xmlmanager.AddNamespace("UML", "omg.org/UML1.3");

            XmlNode model = xmlPattern.xmlDOM.SelectSingleNode("/XMI/XMI.content/UML:Model", xmlmanager);

            XmlNode content = model.SelectSingleNode("/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement/UML:Package/UML:Namespace.ownedElement", xmlmanager).CloneNode(true);

            XmlNode package = model.SelectSingleNode("/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement", xmlmanager);

            model.RemoveChild(package);

            model.AppendChild(content);

            Diagram diagram = patternPackage.Diagrams.GetAt(0);

            makeUmlPattern(patternPackage, diagram);

            XmlElement patternList = xmlPattern.xmlDOM.CreateElement("UMLPattern.PromptList");

            patternTag.AppendChild(patternList);

            foreach ( Element element in patternPackage.Elements)
            {
                addElement(element, patternList);
            }

            xmlPattern.xmlDOM.Save(file);
        }

        private void addElement(Element element, XmlElement patternList)
        {
            XmlElement elementTag = xmlPattern.xmlDOM.CreateElement("Element");

            XmlAttribute name = xmlPattern.xmlDOM.CreateAttribute("name");
            name.Value = patternName +":"+ element.MetaType;
            elementTag.Attributes.Append(name);

            XmlAttribute guid = xmlPattern.xmlDOM.CreateAttribute("guid");
            guid.Value = element.ElementGUID.Replace("{", "").Replace("}", "");
            elementTag.Attributes.Append(guid);

            XmlAttribute id = xmlPattern.xmlDOM.CreateAttribute("id");
            id.Value = ""; // no se de donde puedo sacar este valor!!! Todo 
            elementTag.Attributes.Append(id);

            XmlAttribute prompt = xmlPattern.xmlDOM.CreateAttribute("prompt");
            prompt.Value = "yes";
            elementTag.Attributes.Append(prompt);

            XmlAttribute link = xmlPattern.xmlDOM.CreateAttribute("link");
            link.Value = "no";
            elementTag.Attributes.Append(link);

            XmlAttribute classifier = xmlPattern.xmlDOM.CreateAttribute("classifier");
            classifier.Value = "no";
            elementTag.Attributes.Append(classifier);

            XmlAttribute type = xmlPattern.xmlDOM.CreateAttribute("type");
            type.Value = "no";
            elementTag.Attributes.Append(type);

            patternList.AppendChild(elementTag);
        }

        void makeUmlPattern(Package patternPackage, Diagram diagram)
        {
            XmlNode extensions = xmlPattern.xmlDOM.SelectSingleNode("/XMI/XMI.extensions");

            patternTag = xmlPattern.xmlDOM.CreateElement("UMLPattern");
            extensions.AppendChild(patternTag);

            patternName = patternPackage.Alias != "" ? patternPackage.Alias : diagram.Name;

            XmlAttribute name = xmlPattern.xmlDOM.CreateAttribute("name");
            name.Value = patternName;

            patternTag.Attributes.Append(name);

            XmlAttribute category = xmlPattern.xmlDOM.CreateAttribute("category");
            category.Value = patternPackage.Element.Tag != "" ? patternPackage.Element.Tag : "Patrones básicos";
            patternTag.Attributes.Append(category);

            XmlAttribute version = xmlPattern.xmlDOM.CreateAttribute("version");
            version.Value = "1.0";
            patternTag.Attributes.Append(version);

            XmlAttribute notes = xmlPattern.xmlDOM.CreateAttribute("notes");
            notes.Value = diagram.Notes;
            patternTag.Attributes.Append(notes);
        }
    }
}
