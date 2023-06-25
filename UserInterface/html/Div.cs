using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface.frw;

namespace UserInterface.html
{
    public class Div : HtmlCommon
    {
        public Div(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            //Metatype = "Div";
            this.setProperties();
        }
    }
}
