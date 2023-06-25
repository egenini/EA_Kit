using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OnJira
{
    /// <summary>
    /// Responsable por el comportamiento básico de todo un addin.
    /// </summary>
    public class MainCommons : MainUtils
    {
        //private bool m_ShowFullMenus = false;

        const string addinName = "OnJira";

        const string menuHeader = "-&" + addinName;

        protected const string TEST_LOGIN = "test login";

        protected EAUtils.EAUtils eaUtils;

        public String EA_Connect(EA.Repository Repository)
        {
            // No special processing req'd

            return "Productividad";
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
                    string[] ar = { TEST_LOGIN };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                bool tableCondition = this.getTableSelected(repository) != null;

                if (ItemName == TEST_LOGIN)
                {
                    IsEnabled = true;

                }
                /*
                else if (ItemName == ITEM_MENU__GENERAR_CODIGO)
                {
                    IsEnabled = this.getEnumSelected(repository, location) != null;
                }
                */
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }

        /*
        * http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_getribboncategory.html
        */
        /*
        public string EA_GetRibbonCategory(Repository repository)
        {
            return "DISEÑAR";
        }
        */

        // http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_onretrievemodeltemplate.html

        public string EA_OnRetrieveModelTemplate(Repository repository, string location)
        {
            string template = "";

            MessageBox.Show("Plantilla solicitada " + location);

            return template;
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
