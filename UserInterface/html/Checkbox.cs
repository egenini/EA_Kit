using EA;
using EAUtils;
using UserInterface.frw;

namespace UserInterface.html
{
    internal class Checkbox : FormField
    {
        public Checkbox(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            Metatype = "Checkbox";
        }
    }
}