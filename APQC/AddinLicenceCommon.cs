using EAUtils;
using LicenceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APQC
{
    public abstract class AddinLicenceCommon
    {
        protected EAUtils.EAUtils eaUtils;
        protected Manager licenceManager;

        protected AddinInfo addinInfo;

        protected abstract void onAddinInit();

        public void buildEAUtils(EA.Repository Repository)
        {
            if (this.eaUtils == null)
            {
                eaUtils = new EAUtils.EAUtils();
            }
            eaUtils.setRepositorio(Repository);
            eaUtils.createOutput();
        }

        public bool IsProjectOpen(EA.Repository Repository)
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

        public string EA_GetRibbonCategory(EA.Repository Repository)
        {
            return addinInfo.ribbonCategory;
        }

        public void EA_GetMenuState(EA.Repository repository, string location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(repository))
            {
                IsEnabled = true;
            }
            else
                IsEnabled = false;
        }

        public String EA_Connect(EA.Repository Repository)
        {
            if (this.licenceManager == null)
            {
                this.onAddinInit();

                this.licenceManager = new LicenceManager.Manager(this.addinInfo.name, this.addinInfo.licence);
            }

            return this.addinInfo.name;
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string menuName)
        {
            if (this.addinInfo.licence == null || this.licenceManager.isLicensed())
            {
                if (menuName == "")
                {
                    return this.addinInfo.menuHeader();
                }
                else if (menuName == this.addinInfo.menuHeader())
                {
                    string[] ar = this.addinInfo.menuItems.ToArray<string>();
                    if (this.addinInfo.licence != null)
                    {
                        ar = this.licenceManager.addLicencedMenu(ar);
                    }
                    return ar;
                }
            }
            else
            {
                return this.licenceManager.menu(menuName, this.addinInfo.menuHeader());
            }
            return "";
        }

    }
}
