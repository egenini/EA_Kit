using EAUtils;
using LicenceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18N
{
    public class MainCommons : AddinLicenceCommon
    {

        protected static string ITEM_MENU__TRADUCIR = Properties.Resources.ITEM_MENU__TRADUCIR;

        protected static string ITEM_MENU__ACERCA_DE = Properties.Resources.ITEM_MENU__ACERCA_DE;

        protected override void onAddinInit()
        {
            addinInfo = new EAUtils.AddinInfo("I18N", "Publicar", new Licence(), "1.0.0");

            addinInfo.menuItems.Add(ITEM_MENU__TRADUCIR);
            addinInfo.menuItems.Add(ITEM_MENU__ACERCA_DE);
        }
    }
}
