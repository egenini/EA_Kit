using EA;
using System;
using System.Collections.Generic;
using System.Xml;

namespace EAUtils
{
    public class EAUtils
    {
        private static int TAB__CLOSE = 0; //  to indicate that it is not visible at all
        private static int TAB__OPEN = 1; //  to indicate that it is open but not top-most
        private static int TAB__ACTIVE = 2; //  to indicate that a tab is open and active (top-most)

        public static int ELEMENT_AND_CONNECTOR__ELEMENT_POSITION = 0;
        public static int ELEMENT_AND_CONNECTOR__CONNECTOR_POSITION = 1;

        public TaggedValuesUtils taggedValuesUtils;
        public ConnectorUtils connectorUtils;
        public DiagramUtils diagramUtils;
        public PackageUtils packageUtils;
        public ElementUtils elementUtils;
        public AttributeUtils attributeUtils;
        public InstanceUtil instanceUtils;
        public RepositoryConfiguration repositoryConfiguration;
        public DataTypeUtils dataTypeUtils;
        public SQLUtils sqlUtils;

        public Repository repository;

        private string output = "out";

        public static object StringUtils { get; internal set; }

        public EAUtils()
        {
            taggedValuesUtils = new TaggedValuesUtils();
        }

        public EAUtils(Repository repository)
        {
            this.setRepositorio(repository);
            taggedValuesUtils = new TaggedValuesUtils();
        }

        public void setRepositorio(Repository repository)
        {
            this.repository = repository;
            this.connectorUtils = new ConnectorUtils(this.repository);
            this.diagramUtils = new DiagramUtils(this.repository);
            this.packageUtils = new PackageUtils(this.repository);
            this.elementUtils = new ElementUtils(this.repository);
            this.elementUtils.setEaUtils(this);
            this.attributeUtils = new AttributeUtils(this.repository);
            this.instanceUtils = new InstanceUtil( this );
            this.repositoryConfiguration = new RepositoryConfiguration(this);
            this.dataTypeUtils = new DataTypeUtils(this.repository);
            this.sqlUtils = new SQLUtils(this);
        }

        public string notes2Txt(string notes)
        {
            return this.repository.GetFormatFromField("TXT", notes);
        }

        public string notes2Html(string notes)
        {
            return this.repository.GetFormatFromField("HTML", notes);
        }

        public string notes2Rtf(string notes)
        {
            return this.repository.GetFormatFromField("RTF", notes);
        }


        public string notes2Txt( Element element)
        {
            return this.repository.GetFormatFromField("TXT", element.Notes);
        }

        public string notes2Html(Element element)
        {
            return this.repository.GetFormatFromField("HTML", element.Notes);
        }

        public string notes2Rtf(Element element)
        {
            return this.repository.GetFormatFromField("RTF", element.Notes);
        }

        public void createOutput(string name)
        {
            output = name;
            createOutput();
        }

        public void createOutput()
        {
            
            repository.CreateOutputTab(this.output);
        }

        public void printOut(string toPrint)
        {
            repository.EnsureOutputVisible(output);
            repository.WriteOutput(output, toPrint, 0);

        }

        public string getAttributeDefaultValue( Element element, string name )
        {
            EA.Attribute attr = this.getAttributeByName(element.Attributes, name);

            return attr != null ? attr.Default : "";
        }

        public EA.Attribute getAttributeByName( Collection collection, string name)
        {
            EA.Attribute attr = null;
            foreach(EA.Attribute item in collection )
            {
                if( item.Name == name )
                {
                    attr = item;
                    break;
                }
            }
            return attr;
        }
        public void setValueToTaggedValue(EA.Element theElement, string taggedValueName, string taggedValueValue)
        {
            this.taggedValuesUtils.set(theElement, taggedValueName, taggedValueValue);
        }

        public void activateTab(string tabName)
        {
            this.repository.ActivateTab(tabName);
        }

        public object addTab(string tabName, string userControl)
        {
            return this.repository.AddTab(tabName, userControl);
        }

        public bool isTabOpen(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__OPEN;
        }
        public bool isTabActive(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__ACTIVE;
        }
        public bool isTabClose(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__CLOSE;
        }

        public void mappingElements(Diagram diagram)
        {
            Dictionary<int, Element> elementosOrigenClasificador = new Dictionary<int, Element>();
            Dictionary<int, Element> elementosDestinoClasificador = new Dictionary<int, Element>();
            DiagramObject diagramObject;
            Element element;
            int swimlaneLeftLimit = -1;

            swimlaneLeftLimit = ((Swimlane)diagram.SwimlaneDef.Swimlanes.Items(0)).Width;

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                diagramObject = diagram.DiagramObjects.GetAt(i);

                element = repository.GetElementByID(diagramObject.ElementID);

                if (element.Stereotype == "Attribute")
                {
                    if (diagramObject.left < swimlaneLeftLimit)
                    {
                        elementosOrigenClasificador[element.ClassifierID] = element;
                    }
                    else
                    {
                        elementosDestinoClasificador[element.ClassifierID] = element;
                    }
                }
            }

            // buscar el clasificador en el destino
            foreach (var item in elementosOrigenClasificador)
            {
                if (elementosDestinoClasificador.ContainsKey(item.Key))
                {
                    this.connectorUtils.addConnector(elementosDestinoClasificador[item.Key], elementosOrigenClasificador[item.Key], "Dependency", null, null);
                }
            }
        }

        public List<Usuario> getUsers()
        {
            List<Usuario> users = new List<Usuario>();
            Usuario user;
            var sql = "SELECT userid, userlogin, firstname, surname, department FROM t_secuser ORDER by surname, firstname";

            string queryResult = repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNode rootNode = xmlDOM.DocumentElement.SelectSingleNode("/EADATA/Dataset_0/Data");

                    //XmlNodeList rowList = rootNode.SelectNodes("/Row");

                    for (var i = 0; i < rootNode.ChildNodes.Count; i++)
                    {
                        user = new Usuario();
                        var currentNode = rootNode.ChildNodes.Item(i);

                        user.id = currentNode.ChildNodes[0].InnerText;
                        user.login = currentNode.ChildNodes[1].InnerText;
                        user.nombre = currentNode.ChildNodes[2].InnerText;
                        user.apellido = currentNode.ChildNodes[3].InnerText;
                        user.departamento = currentNode.ChildNodes[4].InnerText;

                        users.Add(user);
                    }
                }
                catch ( Exception w) { w.ToString(); }
            }
            return users;
        }

        public ElementStyle getDefaultStyle( Element element)
        {
            ElementStyle elementStyle = new ElementStyle();

            var sql = "select backcolor, borderstyle, borderwidth, fontcolor, bordercolor from t_object where object_id = " + element.ElementID;

            string queryResult = repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNode rootNode = xmlDOM.DocumentElement.SelectSingleNode("/EADATA/Dataset_0/Data");

                    //XmlNodeList rowList = rootNode.SelectNodes("/Row");
                    if( rootNode.ChildNodes.Count == 1)
                    {
                        var currentNode = rootNode.ChildNodes.Item(0);

                        elementStyle.setBackColor(   currentNode.ChildNodes[0].InnerText );
                        elementStyle.setBorderStyle( currentNode.ChildNodes[1].InnerText );
                        elementStyle.setBorderWidth( currentNode.ChildNodes[2].InnerText );
                        elementStyle.setFontColor(   currentNode.ChildNodes[3].InnerText );
                        elementStyle.setBorderColor( currentNode.ChildNodes[4].InnerText );
                    }
                }
                catch (Exception w) { w.ToString(); }
            }
            return elementStyle;
        }

        public List<string> DbSelectColumnAsArray(string columnName, string table, string whereClause)
        {
            List<string> resultArray = new List<string>();

            // Construct and execute the query
            var sql = "SELECT " + columnName + " FROM " + table;
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;

            string queryResult = repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlUtils xmlUtils = new XmlUtils();
                    resultArray = xmlUtils.getNodeTextList(xmlDOM, "//EADATA//Dataset_0//Data//Row//" + columnName);
                }
                catch (Exception e)
                {
                    queryResult = queryResult.Replace(@"\", "@");
                    xmlDOM.Load(queryResult);

                    XmlUtils xmlUtils = new XmlUtils();
                    resultArray = xmlUtils.getNodeTextList(xmlDOM, "//EADATA//Dataset_0//Data//Row//" + columnName);

                    e.ToString();
                }
            }

            return resultArray;
        }
    }

    public class ElementStyle
    {
        string backColor;
        string borderColor;
        string borderStyle;
        string borderWidth;
        string fontColor;

        public void setBackColor(string backColor)
        {
            this.backColor = backColor;
        }
        public string getBackColor()
        {
            return this.backColor;
        }

        public void setBorderColor(string borderColor)
        {
            this.borderColor = borderColor;
        }
        public string getBorderColor( )
        {
            return this.borderColor;
        }

        public void setBorderStyle(string borderStyle)
        {
            this.borderStyle = borderStyle;
        }

        public string getBorderStyle( )
        {
            return this.borderStyle;
        }

        public void setBorderWidth(string borderWidth)
        {
            this.borderWidth = borderWidth;
        }

        public string getBorderWidth( )
        {
            return this.borderWidth;
        }

        public void setFontColor(string fontColor)
        {
            this.fontColor = fontColor;
        }

        public string getFontColor( )
        {
            return this.fontColor;
        }
    }
}
