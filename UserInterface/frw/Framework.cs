using EAUtils.framework2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.frw
{
    public class Framework : FrameworkCommons2
    {
        const string FRAMEWORK_NAME = "UI DG";

        public Framework(EAUtils.EAUtils eaUtils) : base(eaUtils, "{EC704D70-6DBF-44a3-8951-0F1D4D19DD53}")
        {
            frameworkName = FRAMEWORK_NAME;
        }
    }
}
