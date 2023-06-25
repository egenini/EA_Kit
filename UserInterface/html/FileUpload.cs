using EA;
using EAUtils;
using UserInterface.frw;

namespace UserInterface.html
{
    internal class FileUpload : FormField
    {
        public bool? Multiple { set; get; }
        public string Accept { set; get; }

        protected new void setProperties()
        {
            this.Multiple = this.eaUtils.taggedValuesUtils.get(element, "multiple", "").asBoolean();
            this.Accept   = this.eaUtils.taggedValuesUtils.get(element, "accept", "").asString();
        }

        public FileUpload(Element elementChild, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(elementChild, frameworkInstance, eaUtils)
        {
            Metatype = "FileUpload";
            base.setProperties();
            this.setProperties();
        }


    }
}