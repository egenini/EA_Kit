using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.framework2
{
    public class JsLoader
    {
        public Dictionary<string, string> handlebarsHelpers = new Dictionary<string, string>();
        public List<string> javaScriptFunctions = new List<string>();
        protected EAUtils eaUtils;
        public string name;

        protected void loadJs(Element element, bool reset)
        {
            if( reset && element.Methods.Count != 0)
            {
                handlebarsHelpers.Clear();
                javaScriptFunctions.Clear();
            }

            foreach (Method method in element.Methods)
            {
                if (method.Alias == "Handlebars.registerHelper")
                {
                    this.handlebarsHelpers.Add(method.Name, method.Code);
                }
                else
                {
                    this.javaScriptFunctions.Add(method.Code);
                }
                
                // esto es por si se sobreescribe
                if (this.handlebarsHelpers.ContainsKey(method.Name))
                {
                    this.handlebarsHelpers.Remove(method.Name);
                }
                this.handlebarsHelpers.Add(method.Name, method.Code);
            }
        }
    }
}
