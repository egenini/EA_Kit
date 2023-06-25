using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model2Table
{
    public class MainCommons : MainUtils
    {
        public const string addinName = "Model2Table";

        const string menuHeader = "-&" + addinName;

        protected EAUtils.EAUtils eaUtils;
        protected FrameworkManager frameworkManager = null;
        protected Sincro sincro = null;
        public void buildEAUtils(EA.Repository Repository)
        {
            if (this.eaUtils == null)
            {
                eaUtils = new EAUtils.EAUtils();

                eaUtils.setRepositorio(Repository);

                eaUtils.createOutput(addinName);

                this.frameworkManager = new FrameworkManager(eaUtils);

                this.sincro = new Sincro(eaUtils);
            }
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

                this.buildEAUtils(Repository);

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
                    string[] ar = { "" };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (isProjectOpen(repository))
            {
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }    
    }
}
