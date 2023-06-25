using EA;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EAUtils
{
    public class DbUtils
    {
        public Repository repository;
        private XmlUtils xmlUtils = new XmlUtils();
        public XmlNodeList nodeList;
        List<string> header = new List<string>();

        public DbUtils(Repository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Ejecuta una query y deja el resultado en nodeList -propiedad pública-
        /// </summary>
        /// <param name="query"></param>
        /// <returns> true si hay resultados </returns>
        public bool execute(string query)
        {
            bool hasResult = false;

            header.Clear();

            string resultSet = this.repository.SQLQuery(query);

            if (resultSet != null)
            {
                XmlDocument resultSetXmlDocument = xmlUtils.createDOM();

                resultSetXmlDocument.LoadXml(resultSet);

                nodeList = resultSetXmlDocument.DocumentElement.SelectNodes("descendant::Row");

                if( nodeList.Count > 0 )
                {
                    hasResult = true;
                }
            }
            else
            {
                nodeList = null;
            }

            return hasResult;
        }

        public List<Dictionary<string, Dictionary<string, string>>> asObject()
        {
            List<Dictionary<string, Dictionary<string, string>>> results = new List<Dictionary<string, Dictionary<string, string>>>();

            string[] colNameSplit;
            string objName;
            string propName;

            foreach (XmlNode row in nodeList)
            {
                results.Add(new Dictionary<string, Dictionary<string, string>>());

                foreach (XmlNode col in row.ChildNodes)
                {
                    colNameSplit = col.Name.Split('_');

                    objName = colNameSplit[0];

                    propName = string.Join("_", colNameSplit.Slice(2));

                    if( ! results[results.Count - 1].ContainsKey(objName))
                    {                        
                        results[results.Count - 1].Add(objName, new Dictionary<string, string>());
                    }

                    results[results.Count - 1][objName].Add(propName, col.InnerText);
                }
            }

            return results;
        }

        public List<string> getHeader()
        {
            return this.header;
        }

        /// <summary>
        /// Este método genera una lista con los datos obtenidos y otra con los nombres de las columnas, para obtener está última utilizar el método getHeader()
        /// </summary>
        /// <returns> Lista de lista de objetos, la primer lista contiene las rows y las interiores las cols</returns>
        public List<List<string>> asList()
        {
            
            List<List<string>> data  = new List<List<string>>();

            foreach (XmlNode row in nodeList)
            {
                data.Add(new List<string>());

                foreach (XmlNode col in row.ChildNodes)
                {
                    if(header.Count != row.ChildNodes.Count)
                    {
                        header.Add(col.Name);
                    }

                    data[data.Count- 1].Add(col.InnerText);
                }
            }

            return data;
        }
    }
}
