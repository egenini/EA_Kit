using EA;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APQC
{
    public class Main : MainUtils
    {
        public void EA_MenuClick(Repository repository, string location, string menuName, string itemName)
        {
            if (this.licenceManager.manageMenuClick(menuName, itemName))
            {
                // nada acá si corresponde lo hace adentro.
            }
            else if (itemName == ITEM_MENU__ACERCA_DE)
            {
                AboutBox ab = new AboutBox(addinInfo.name, "", addinInfo.version, addinInfo.licence.timeRemaining());
                ab.Show();
            }
            else if (itemName == ITEM_MENU__IMPORTAR)
            {
                this.import(this.getPackageSelected(repository), repository);
            }
        }

        void import(Package package, Repository repository)
        {
            this.buildEAUtils(repository);

            try
            {
                Importer importer = new Importer(this.eaUtils);

                importer.smartImport(package);
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
            }
        }
    }
}
