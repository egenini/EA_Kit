using EA;
using System.Collections.Generic;
using System;

namespace EAUtils.framework.domain
{
    public class DataType
    {
        public string name = "";
        public string elementGuid = "";
        public int elementId = 0;
        public Element element;
        public Dictionary<string, string> mappings = new Dictionary<string, string>();
         
        public DataType(Element element)
        {
            this.element = element;
            this.name = element.Name;
            this.elementGuid = element.ElementGUID;
            this.elementId = element.ElementID;
        }

        internal string getMapped(string languageSource, string dataTypeSource)
        {
            string dataType = null;
            string mappingKey = languageSource + "." + dataTypeSource;
            if ( mappings.ContainsKey( mappingKey ))
            {
                dataType = mappings[mappingKey];
            }

            return dataType;
        }
    }
}