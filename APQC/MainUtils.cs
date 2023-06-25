using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APQC
{
    public class MainUtils : MainCommons
    {
        public Package getPackageSelected(Repository repository)
        {
            Package package = null;
            ObjectType ot = repository.GetContextItemType();

            switch (ot)
            {
                case EA.ObjectType.otPackage:

                    package = repository.GetContextObject();
                    break;
            }
            return package;
        }
    }
}
