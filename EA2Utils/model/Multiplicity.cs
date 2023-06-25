using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.model
{
    public class Multiplicity
    {
        public int lowerBound { set; get; }
        public int upperBound { set; get; }
        public bool isLowerBoundMany { set; get; } = false;
        public bool isUpperBoundMany { set; get; } = false;

        public Multiplicity(EA.Attribute attribute, EAUtils eautils)
        {
            if (attribute.LowerBound.Contains("*"))
            {
                isLowerBoundMany = true;
            }
            else
            {
                int lowerBound = 0;
                int.TryParse(attribute.LowerBound, out lowerBound);

                this.lowerBound = lowerBound;
            }

            if (attribute.UpperBound.Contains("*"))
            {
                isUpperBoundMany = true;
            }
            else
            {
                int upperBound = 0;
                int.TryParse(attribute.UpperBound, out upperBound);

                this.upperBound = upperBound;
            }
        }
    }

}
