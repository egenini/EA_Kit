using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserInterface
{
    public class MainCommons : MainUtils
    {
        const string addinName = "User Interface";

        const string menuHeader = "-&" + addinName;

        protected EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();

        protected const string ITEM_MENU__GENERAR_UI = "Generar";
        protected const string ITEM_MENU__GENERAR_JSON = "Generar JSON";
        protected const string ITEM_MENU__GENERAR_ARTEFACTO = "Generar artefacto";

        public String EA_Connect(EA.Repository Repository)
        {
            // No special processing req'd

            return addinName;
        }

        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                eaUtils.setRepositorio(Repository);
                eaUtils.createOutput();

                return true;
            }
            catch(Exception e)
            {
                Clipboard.SetText(e.ToString());
                return false;
            }
        }
        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            switch (MenuName)
            {
                case "":
                    return menuHeader;
                case menuHeader:
                    string[] ar = { ITEM_MENU__GENERAR_UI, ITEM_MENU__GENERAR_JSON, ITEM_MENU__GENERAR_ARTEFACTO };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                if (ItemName == ITEM_MENU__GENERAR_UI)
                {
                    IsEnabled = this.getElementSelected(repository) != null;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void buildEAUtils(EA.Repository Repository)
        {
            if (this.eaUtils == null)
            {
                eaUtils = new EAUtils.EAUtils();
            }
            eaUtils.setRepositorio(Repository);
            eaUtils.createOutput();
        }
    }
}
