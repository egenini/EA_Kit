using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMN.framework.domain
{
    public class Language
    {
        public string name = "";
        public int elementId = 0;
        public Element element;

        public Dictionary<string, string> handlebarsTemplates = new Dictionary<string, string>();
        public Dictionary<string, string> handlebarsHelpers = new Dictionary<string, string>();
        public Dictionary<string, string> javaScriptFunctions = new Dictionary<string, string>();

        public List<DataType> dataTypes = new List<DataType>();
        //internal Func<object, string> template;

        public object partialsTemplates = null;
        public string mainTemplate;

        public Language(Element languageElement)
        {
            
            this.element = languageElement;
            this.name = languageElement.Name;
            this.elementId = languageElement.ElementID;
        }

        internal string getDataType(string languageSource, string dataTypeSource)
        {
            string dataTypeMapped = null;

            foreach( DataType dataType in dataTypes)
            {
                dataTypeMapped = dataType.getMapped(languageSource, dataTypeSource);
                if( dataTypeMapped != null)
                {
                    break;
                }
            }

            return dataTypeMapped != null ? dataTypeMapped : dataTypeSource;
        }

        public Dictionary<string, string> getFunctions()
        {
            Dictionary<string, string> functions = new Dictionary<string, string>();

            foreach( Method function in this.element.Methods)
            {
                functions.Add( function.Name, function.Code );
            }

            return functions;
        }
    }
}
