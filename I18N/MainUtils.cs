using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace I18N
{
    public class MainUtils : MainCommons
    {

        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public Package getPackageSelected(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Package package = null;

            switch (ot)
            {
                case EA.ObjectType.otPackage:

                    package = repository.GetContextObject();
                    break;

                /*
                case EA.ObjectType.otElement:

                    element = repository.GetContextObject();
                    package = repository.GetPackageByID(element.PackageID);
                    break;
                */
            }
            return package;
        }

        public Element getElementSelected(Repository repository)
        {
            ObjectType ot = repository.GetContextItemType();
            Element element = null;

            switch (ot)
            {
                case EA.ObjectType.otElement:

                    element = repository.GetContextObject();
                    break;
            }
            return element;
        }

    }
}
