using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APQC
{
    public class MainCommons : AddinLicenceCommon
    {
        protected bool traducirAlImportar = false;

        protected static string ITEM_MENU__IMPORTAR = Properties.Resources.ITEM_MENU__IMPORTAR;
        protected static string ITEM_MENU__ACERCA_DE = Properties.Resources.ITEM_MENU__ACERCA_DE;

        protected override void onAddinInit()
        {
            addinInfo = new EAUtils.AddinInfo("APQC-EA", "Publicar", new Licence(), "1.0.0");

            addinInfo.menuItems.Add(ITEM_MENU__IMPORTAR);
            addinInfo.menuItems.Add(ITEM_MENU__ACERCA_DE);
        }




    }
}
