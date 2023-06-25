using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDGBuilder
{
    public class MainCommons : MainUtils
    {
        const string addinName = "MDG Builder";

        const string menuHeader = "-&" + addinName;

        protected static string ITEM_MENU__CONFIGURAR = Properties.Resources.ITEM_MENU__CONFIGURAR;
        protected static string ITEM_MENU__GENERAR = Properties.Resources.ITEM_MENU__GENERAR;
        protected static string ITEM_MENU__ADD_METATYPE = Properties.Resources.ITEM_MENU__ADD_METATYPE;
        protected static string ITEM_MENU__GENERAR_QUICKLINKER = Properties.Resources.ITEM_MENU__GENERAR_QUICKLINKER;
        protected static string ITEM_MENU__EDITOR_QUICKLINKER = Properties.Resources.ITEM_MENU__EDITOR_QUICKLINKER;
        protected static string ITEM_MENU__IMPORTAR_MTS = Properties.Resources.ITEM_MENU__IMPORTAR_MTS;
        protected static string ITEM_MENU__EXPORT_REFERENCE_DATA = Properties.Resources.ITEM_MENU__EXPORT_REFERENCE_DATA;
        protected static string ITEM_MENU__GENERAR_ENTREGABLE = Properties.Resources.ITEM_MENU__GENERAR_ENTREGABLE;
        protected static string ITEM_MENU__SINCRONIZAR_ESTEREOTIPOS = Properties.Resources.ITEM_MENU__SINCRONIZAR_ESTEREOTIPOS;
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
                    string[] ar = { ITEM_MENU__GENERAR, ITEM_MENU__CONFIGURAR, ITEM_MENU__ADD_METATYPE, 
                        ITEM_MENU__GENERAR_QUICKLINKER, ITEM_MENU__EDITOR_QUICKLINKER, ITEM_MENU__IMPORTAR_MTS, 
                        ITEM_MENU__EXPORT_REFERENCE_DATA, ITEM_MENU__GENERAR_ENTREGABLE, ITEM_MENU__SINCRONIZAR_ESTEREOTIPOS };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (isProjectOpen(repository))
            {
                //bool tableCondition = this.getMDGPackage(repository) != null;
                
                if (ItemName == ITEM_MENU__GENERAR)
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__ADD_METATYPE)
                {
                    IsEnabled = ( this.getProfilePackage(repository) != null ) || ( this.getProfileElement(repository) != null );
                }
                else if (ItemName == ITEM_MENU__CONFIGURAR)
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__GENERAR_QUICKLINKER)
                {
                    IsEnabled = this.getQuicklinkerPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__EDITOR_QUICKLINKER)
                {
                    IsEnabled = this.getQuicklinkerPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__IMPORTAR_MTS )
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__EXPORT_REFERENCE_DATA)
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
                else if (ItemName == ITEM_MENU__GENERAR_ENTREGABLE)
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
                else if( ItemName == ITEM_MENU__SINCRONIZAR_ESTEREOTIPOS)
                {
                    IsEnabled = this.getMDGPackage(repository) != null;
                }
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }

    }
}
