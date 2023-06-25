using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface.frw;

namespace UserInterface.framework
{
    public class Framework : FrameworkCommons
    {
        public Framework(EAUtils.EAUtils eaUtils) : base(eaUtils)
        {
        }

        public void build()
        {
            base.build("{A728CCEA-79FF-45d4-BE98-2FD37EEC8108}");

        }

        public static implicit operator Framework(frw.Framework v)
        {
            throw new NotImplementedException();
        }
    }
}
