using EA;
using EAUtils;
using UserInterface.frw;

namespace UserInterface.html
{
    internal class Radio : FormField
    {
        public Radio(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Radio";
        }
    }
}