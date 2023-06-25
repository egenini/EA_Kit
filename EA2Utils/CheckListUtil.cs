using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EAUtils
{
    public class CheckListUtil
    {
        const string TV_NAME = "Checklist";
        const string KEY_NAME = "Text";
        const string VALUE_NAME = "Checked";

        EAUtils eaUtils;
        public Element checkListElement = null;
        XmlDocument xmlDoc = new XmlDocument();
        Package package = null;

        Dictionary<string, bool> checkList = new Dictionary<string, bool>();

        public CheckListUtil(Element checkList, EAUtils eaUtils)
        {
            this.eaUtils   = eaUtils;
            this.checkListElement = checkList;
        }

        public CheckListUtil(Package package, EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.package = package;
        }

        public bool findByStereotype()
        {
            foreach (Element checkList in package.Elements)
            {
                if (checkList.Stereotype == "Checklist")
                {
                    this.checkListElement = checkList;
                    break;
                }
            }
            return this.checkListElement != null;
        }

        public void parse()
        {
            xmlDoc.LoadXml(this.eaUtils.taggedValuesUtils.get(this.checkListElement, TV_NAME, "").asString());

            XmlNode rootNode = xmlDoc.DocumentElement.SelectSingleNode("/Checklist");

            for (var i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                var currentNode = rootNode.ChildNodes.Item(i);

                checkList.Add(currentNode.Attributes.GetNamedItem(KEY_NAME).Value, currentNode.Attributes.GetNamedItem(VALUE_NAME).Value == "True");
            }
        }
        public bool exist(string key)
        {
            return checkList.ContainsKey(key);
        }

        public bool isChecked( string key)
        {
            return checkList[key];
        }
 
    }
}
