using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReporteProcesos_DS_TIC
{
    public class MainUtils
    {
        public EA.Package getPackage( EA.Repository repository)
        {
            EA.Package package = null;

            ObjectType ot = repository.GetContextItemType();

            switch (ot)
            {
                case ObjectType.otPackage:

                    Package current = repository.GetContextObject();

                    for( short i = 0; i < current.Elements.Count; i++)
                    {
                        if( current.Elements.GetAt(i).Type == "Change")
                        {
                            package = current;
                            break;
                        }
                    }
                    break;
            }
            return package;
        }
    }
}
