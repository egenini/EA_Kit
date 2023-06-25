using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Persistencia
{
    public class MainCommons : MainUtils
    {
        const string addinName = "Persistencia";

        const string menuHeader = "-&" + addinName;

        protected const string ITEM_MENU__GENERAR = "Generar";
        protected const string ITEM_MENU__GENERAR_ARTEFACTO = "Generar artefactos";
        protected const string ITEM_MENU__GENERAR_JSON = "Generar JSON";

        protected EAUtils.EAUtils eaUtils;

        public void buildEAUtils(EA.Repository Repository)
        {
            if( this.eaUtils == null)
            {
                eaUtils = new EAUtils.EAUtils();
            }
            eaUtils.setRepositorio(Repository);
            eaUtils.createOutput();
        }

        public String EA_Connect(EA.Repository Repository)
        {
            // No special processing req'd

            return "Persistencia";
        }
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                eaUtils = new EAUtils.EAUtils();
                eaUtils.setRepositorio(Repository);
                eaUtils.createOutput();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public string EA_GetRibbonCategory(EA.Repository Repository)
        {
            //return "Especializar";
            return "Construir";

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
                    string[] ar = { ITEM_MENU__GENERAR, ITEM_MENU__GENERAR_ARTEFACTO, ITEM_MENU__GENERAR_JSON };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                bool tableCondition = this.getTableSelected(repository) != null;

                if (ItemName == ITEM_MENU__GENERAR)
                {
                    IsEnabled = tableCondition;
                }
                else if (ItemName == ITEM_MENU__GENERAR_JSON)
                {
                    IsEnabled = tableCondition;
                }
                else if (ItemName == ITEM_MENU__GENERAR_ARTEFACTO)
                {
                    IsEnabled = tableCondition;
                }
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }
    }
}
