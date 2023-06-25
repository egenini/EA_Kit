using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia
{
    public class MainUtils
    {
        public Element getTableSelected(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element element = null;

            switch (ot)
            {
                case EA.ObjectType.otElement:

                    element = repository.GetContextObject();
                    if (element.Stereotype != "table")
                    {
                        element = null;
                    }
                    break;
            }
            return element;
        }

    }
}
