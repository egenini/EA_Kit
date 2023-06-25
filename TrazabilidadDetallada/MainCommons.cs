using EA;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UIResources;

namespace TrazabilidadDetallada
{
    public class MainCommons : MainUtilites
    {

        public bool m_ShowFullMenus = false;

        string menuHeader = null; 

        public String EA_Connect(EA.Repository Repository)
        {
            //Alert.Warning("Agregar al diccionario las clases");

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            menuHeader = "-&" + Properties.Resources.addin_name;

            return Properties.Resources.addin_name;
        }

        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public object EA_GetMenuItems(Repository Repository, string Location, string MenuName)
        {
            if (MenuName == "")
            {
                return menuHeader;
            }
             else if( MenuName == menuHeader )
            {
                string[] ar = { Properties.Resources.ITEM_MENU__SINCRONIZAR
                        , Properties.Resources.ITEM_MENU__MAPEAR };
                return ar;

            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                if (ItemName == Properties.Resources.ITEM_MENU__MAPEAR)
                {
                    IsEnabled = this.getMapeable(repository, location) != null;
                }
                else if (ItemName == Properties.Resources.ITEM_MENU__SINCRONIZAR)
                {
                    IsEnabled = true;
                }
            }
            else
                // If no open project, disable all menu options
                IsEnabled = false;
        }

        public Element getDropped(Repository repository, EA.EventProperties info)
        {
            Element element = null;
            try
            {
                element = repository.GetElementByID(info.Get("DroppedID").Value);
            }
            catch (Exception) { }

            return element;
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Clients;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /*
         * http://sparxsystems.com/enterprise_architect_user_guide/13.0/automation/ea_getribboncategory.html
         */
 
        
        public string EA_GetRibbonCategory(Repository repository)
        {
            return "DISEÑAR";
        }
        

    }
}
