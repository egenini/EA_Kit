using EA;
using EAUtils;
using UserInterface.frw;

namespace UserInterface.html
{
    internal class Form : HtmlCommon
    {
        public string AcceptCharset { set; get; }
        public string Action { set; get; }
        public string Enctype { set; get; }
        public string Target { set; get; }
        public string Method { set; get; }

        public Form(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Form";
            this.setProperties();
        }

        public new void setProperties()
        {
            base.setProperties();

            this.AcceptCharset = this.eaUtils.taggedValuesUtils.get(element, "accept-charset", "").asString();
            this.Action = this.eaUtils.taggedValuesUtils.get(element, "action", "").asString();
            this.Enctype = this.eaUtils.taggedValuesUtils.get(element, "enctype", "").asString();
            this.Target = this.eaUtils.taggedValuesUtils.get(element, "target", "").asString();
            this.Method = this.eaUtils.taggedValuesUtils.get(element, "method", "").asString();

        }
    }
}