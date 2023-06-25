using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface.frw;

namespace UserInterface.html
{
    public class Input : FormField
    {
        public string Type { set; get; }

        public Input(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Input";
            base.setProperties();
            this.setProperties();
        }

        protected new void setProperties()
        {
            this.Type = this.eaUtils.taggedValuesUtils.get(element, "type", "").asString();
        }
    }
}