using EA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;

namespace EAUtils
{
    public class SQLUtils
    {
        public Repository repository;
        public EAUtils eaUtils;

        public SQLUtils(EAUtils eaUtils) 
        { 
            this.eaUtils = eaUtils;
            this.repository = eaUtils.repository;
        }

        public Object excecute(string query)
        {
            
            bool asObjects = query.IndexOf("__") != -1;

            Object results = null;
            XmlDocument xmlDOM = new XmlDocument();

            xmlDOM.LoadXml(repository.SQLQuery(query));

            XmlNodeList nodeList = xmlDOM.SelectNodes("descendant::Row");

            if (asObjects)
            {
                results = this.resultAsObjects(nodeList);
            }
            else
            {
                results = this.resultsAsList(nodeList);
            }

            return results;
        }

        private List<Dictionary<string, object>> resultAsObjects(XmlNodeList nodeList)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            foreach (XmlNode row in nodeList)
            {
                results.Add(new Dictionary<string, object>());

                foreach (XmlNode col in row.ChildNodes)
                {
                    string[] nameSplit = col.Name.Split('_');
                    string objName = nameSplit[0];
                    string propName = String.Join("_", nameSplit.Slice(2));
                    string propValue = col.InnerText;

                    if( ! results[results.Count - 1].ContainsKey(objName) )
                    {
                        results[results.Count - 1].Add(objName, new Dictionary<string, object>());
                    }

                    ((Dictionary<string, object>)results[results.Count - 1][objName]).Add(propName,propValue);
                }
            }

            return results;
        }

        private ResultAsList resultsAsList(XmlNodeList nodeList)
        {
            List<List<object>> data = new List<List<object>>();
            List<string> header = new List<string>();
            bool isFirstRow = true;

            foreach (XmlNode row in nodeList)
            {
                data.Add(new List<object>());

                foreach (XmlNode col in row)
                {
                    if (isFirstRow)
                    {
                        header.Add(col.Name);
                    }

                    data[data.Count - 1].Add(col.InnerText);
                }

                isFirstRow = false;
            }

            return new ResultAsList(header, data);
        }
    }

    public class ResultAsList
    {
        public List<string> header = null;
        public List<List<object>> data = null;

        public ResultAsList(List<string> header, List<List<object>> data) 
        {
            this.header = header;
            this.data = data;
        }
    }
}
