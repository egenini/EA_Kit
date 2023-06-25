using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proceso4Doc
{
    public class MainCommons : MainUtils
    {
        const string addinName = "Proceso4Doc";
        const string menuHeader = "-&" + addinName;

        protected static string MENU_ITEM__GENERAR = Properties.Resources.MENU_ITEM__GENERAR;

        protected EAUtils.EAUtils eaUtils;

        public void buildEAUtils(EA.Repository Repository)
        {
            if (this.eaUtils == null)
            {
                eaUtils = new EAUtils.EAUtils();
            }
            eaUtils.setRepositorio(Repository);
            eaUtils.createOutput();
        }

        public String EA_Connect(EA.Repository Repository)
        {
            // No special processing req'd

            return addinName;
        }

        public bool isProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                eaUtils = new EAUtils.EAUtils();
                eaUtils.setRepositorio(Repository);
                eaUtils.createOutput(addinName);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            /* nb example of out parameter:
			object item;
			EA.ObjectType t = Repository.GetTreeSelectedItem(out item);
			EA.Package r = (EA.Package) item;
			*/

            switch (MenuName)
            {
                case "":
                    return menuHeader;
                case menuHeader:
                    string[] ar = { MENU_ITEM__GENERAR };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (isProjectOpen(repository))
            {
                if (ItemName == MENU_ITEM__GENERAR)
                {
                    IsEnabled= true;
                }
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }
    }
}
