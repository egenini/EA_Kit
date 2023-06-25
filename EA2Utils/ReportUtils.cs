using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EAUtils
{
    public class FragmentHelper
    {
        XmlDocument xmlDOM = new XmlDocument();
        XmlNode xmlData = null;
        XmlNode currentRow = null;
        public FragmentHelper() 
        {
            var node = this.xmlDOM.CreateProcessingInstruction("xml", "version='1.0' encoding='ISO-8859-1'");

            this.xmlDOM.AppendChild(node);

            var xmlRoot = this.xmlDOM.CreateElement("EADATA");

            this.xmlDOM.AppendChild(xmlRoot);

            var xmlDataSet = this.xmlDOM.CreateElement("Dataset_0");

            xmlRoot.AppendChild(xmlDataSet);

            this.xmlData = this.xmlDOM.CreateElement("Data");

            xmlDataSet.AppendChild(this.xmlData);
        }

        public void addRow()
        {
            this.currentRow = this.xmlDOM.CreateElement("Row");

            this.xmlData.AppendChild(this.currentRow);
        }

        public void addData(string name, string value)
        {
            var xmlElement = this.xmlDOM.CreateElement(name);

            xmlElement.InnerText = value;

            this.currentRow.AppendChild(xmlElement);
        }
    }
}
