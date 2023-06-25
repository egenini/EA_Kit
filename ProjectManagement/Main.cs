using EA;
using ProjectManagement.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIResources;

namespace ProjectManagement
{
    public class Main: MainCommons
    {
        private EffortEditor effortEditor = null;

        public void EA_MenuClick(EA.Repository repository, string location, string MenuName, string ItemName)
        {
            if (ItemName == Properties.Resources.MENU_ITEM_CALCULAR_ESFUERZO)
            {
                calcularEsfuerzo(repository, location);
            }
        }

        private void calcularEsfuerzo( Repository repository, string location)
        {

            try
            {
                Element task = this.getTask(repository);

                if ( task != null)
                {
                    Esfuerzo esfuerzo = new Esfuerzo(this.eaUtils, task);

                    esfuerzo.calcular( task );
                }
                else
                {
                    Esfuerzo esfuerzo = new Esfuerzo(this.eaUtils);
                    esfuerzo.calcular(this.getPackage(repository));
                }

                Alert.Success("");

            }
            catch ( Exception e)
            {
                Clipboard.SetText( e.Message );
                Alert.Error("");
            }
        }
        private void show(Element element)
        {
            if (this.effortEditor == null)
            {
                this.effortEditor = (EffortEditor)eaUtils.repository.AddWindow(EffortEditor.NAME, "ProjectManagement.ui.EffortEditor");
                eaUtils.repository.ShowAddinWindow(EffortEditor.NAME);
            }
            try
            {
                this.effortEditor.show(element, this.eaUtils);
            }
            catch (Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error(Properties.Resources.ERROR_EN_PORTAPAPELES);
            }
        }

        // ******************************
        // *** Suscripcion a eventos. ***
        // ******************************
        public void EA_OnContextItemChanged(EA.Repository repository, String GUID, EA.ObjectType ot)
        {
            if (isProjectOpen(repository))
            {
                switch (ot)
                {
                    case EA.ObjectType.otElement:
                        Element element = repository.GetContextObject();
                        if( element.Type == "Task")
                        {
                            this.show(element);
                        }
                        break;
                }
            }
        }

        public Boolean EA_OnPreDropFromTree(EA.Repository repository, EA.EventProperties info)
        {
            Boolean doIt    = true;
            Element element = null;

            if (repository != null && info != null)
            {
                string type = info.Get("Type").Value;

                if (type == "4")
                {
                    element = repository.GetElementByID(info.Get("ID").Value);
                }

                Diagram diagram = repository.GetDiagramByID(info.Get("DiagramID").Value);
                Element dropped = null;

                try
                {
                    dropped = repository.GetElementByID(info.Get("DroppedID").Value);
                }
                catch (Exception) { }

                if (dropped != null && element != null && dropped.Type == "Actor" && element.Type == "Task")
                {
                    // implica asignar un ejecutor a la tarea.
                    // si 1 o más ejecutores se toma fecha de finalización más alta y al nuevo ejecutor
                    // se le asigna el día siguiente como inicio de la nueva tarea, la puede cambiar si así
                    // lo desea.
                    new EjecutorManager(this.eaUtils).asignarEjecutor(element, dropped);

                    doIt = false;
                }
            }
            return doIt;
        }
    }
}
