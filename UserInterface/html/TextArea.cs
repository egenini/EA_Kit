using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface.frw;

namespace UserInterface.html
{
    public class TextArea : FormField
    {
        public int? Cols { set; get; }
        public int? Rows { set; get; }

        public TextArea(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Textarea";
            base.setProperties();
            this.setProperties();
        }

        protected new void setProperties()
        {
            this.Cols = this.eaUtils.taggedValuesUtils.get(element, "cols", "").asInt();
            this.Rows = this.eaUtils.taggedValuesUtils.get(element, "rows", "").asInt();
        }
    }
}