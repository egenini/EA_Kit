using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReporteProcesos_DS_TIC
{
    public class Main : MainCommons
    {
        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            if (itemName == ITEM_MENU__GENERAR)
            {
                generar(getPackage(repository), repository);
            }
        }

        private void generar( Package package, Repository repository)
        {
            GenerarReporte generador = new GenerarReporte( package, this.eaUtils);

            try
            {
                generador.generar();
                generador.generarDiagramas();
            }
            catch( Exception e)
            {
                Clipboard.SetText(e.ToString());
                this.eaUtils.printOut("error el portapapeles");
            }

        }
    }
}
