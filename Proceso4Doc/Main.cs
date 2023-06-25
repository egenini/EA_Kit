using EA;
using EAUtils;
using EAUtils.flow;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using UIResources.ui.flow;

namespace Proceso4Doc
{
    public class Main : MainCommons, ElegirSiguiente
    {
        SiguienteActividadUI elegirSiguienteUI = null;
        string directorio = null;

        [STAThreadAttribute]
        public void EA_MenuClick(EA.Repository repository, string location, string menuName, string itemName)
        {
            if (itemName == MENU_ITEM__GENERAR)
            {
                
                script.Test test = new script.Test();
                test.repository = this.eaUtils.repository;
                test.probar();
                
                
                if (this.eaUtils.repository.GetCurrentDiagram() == null)
                {
                    this.eaUtils.printOut("Debe haber un diagrama activo para recorrer");
                }
                else
                {
                    if (directorio == null)
                    {
                        EA.Package package = this.eaUtils.repository.GetPackageByID(this.eaUtils.repository.GetCurrentDiagram().PackageID);
                        
                        DirectorioArchivoUsuarioHelper directorioUsuario = new DirectorioArchivoUsuarioHelper(this.eaUtils)
                            .withPaquete( package );

                        directorioUsuario.initSaveFileInfo().fileName("temporal.docx");

                        directorio = directorioUsuario.directorio();
                    }

                    Thread thread = new Thread(new ThreadStart(worker));

                    thread.Start();
                }                
            }
        }

        void worker()
        {
            Recorre4Doc recorre4Doc = new Recorre4Doc(eaUtils);

            recorre4Doc.elector = this;
            recorre4Doc.directorio = directorio;
            recorre4Doc.comenzar();

            /*
            RecorreFlow recorreFlow = new RecorreFlow(eaUtils);

            recorreFlow.elector = this;
            recorreFlow.comenzar();
            */
        }

        public void elegir(Element actual, List<string> posiblesSiguientesGuardas, List<object> posiblesSiguiente, ManualResetEvent mre)
        {
            if (this.elegirSiguienteUI == null)
            {
                this.elegirSiguienteUI = (SiguienteActividadUI)eaUtils.repository.AddTab(SiguienteActividadUI.NAME, "UIResources.ui.flow.SiguienteActividadUI");
            }

            this.elegirSiguienteUI.caller = mre;

            this.elegirSiguienteUI.prepararOpctiones(actual, posiblesSiguientesGuardas, posiblesSiguiente);

            eaUtils.activateTab(SiguienteActividadUI.NAME);
        }

        public int elegido()
        {
            return this.elegirSiguienteUI.select;
        }
    }
}
