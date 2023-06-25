using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;
using UserInterface.frw;

namespace UserInterface.html
{
    public class FormField : HtmlCommon
    {
        public bool? Required { set; get; }
        public bool? Readonly { set; get; }
        public bool? Disabled { set; get; }
        public string Value { set; get; }
        public string Label { set; get; }
        public string Placeholder { set; get; }
        public string Title { set; get; }
        public string Autofocus { set; get; }

        public FormField(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            this.setProperties();
        }

        protected new void setProperties()
        {
            base.setProperties();

            this.Label = element.Alias;
            this.Required = eaUtils.taggedValuesUtils.get(element, "required", "").asBoolean();
            this.Disabled = eaUtils.taggedValuesUtils.get(element, "disabled", "").asBoolean();
            this.Value = this.eaUtils.taggedValuesUtils.get(element, "value", "").asString();
            this.Placeholder = this.eaUtils.taggedValuesUtils.get(element, "placeholder", "").asString();
            this.Title = this.eaUtils.taggedValuesUtils.get(element, "title", "").asString();
            this.Autofocus = this.eaUtils.taggedValuesUtils.get(element, "autofocus", "").asString();
        }
    }
}
