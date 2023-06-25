using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.model
{
    public class Class
    {
        string Name { set; get; }
        string Alias { set; get; }

        List<Attribute> attributes = new List<Attribute>();

        public void add(EA.Attribute attribute, EAUtils eaUtils)
        {
            attributes.Add(new Attribute(attribute, eaUtils));
        }
    }
}
