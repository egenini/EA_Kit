using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class MainUtils
    {
        public Element getElementSelected(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element selected = null;

            switch (ot)
            {
                case EA.ObjectType.otElement:

                    selected = repository.GetContextObject();
                    if (selected.Stereotype != "Screen")
                    {
                        selected = null;
                    }
                    break;
            }
            return selected;
        }
    }
}
