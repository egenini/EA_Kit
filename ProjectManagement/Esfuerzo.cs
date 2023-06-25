using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EAUtils.ConnectorUtils;

namespace ProjectManagement
{
    class Esfuerzo
    {
        private EAUtils.EAUtils eaUtils;
        private float totalEsfuerzo = 0;
        private bool update = true;

        Element currentTarea;

        public EjecutorManager ejecutorManager;
        public EsfuerzoManager esfuerzoManager;
        private Calendar calendar;

        public Esfuerzo(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils         = eaUtils;
            this.esfuerzoManager = new EsfuerzoManager(eaUtils);
            this.calendar        = new Calendar(eaUtils);

            this.calendar.load();

            this.ejecutorManager = new EjecutorManager(eaUtils, this.esfuerzoManager, this.calendar);
            
        }

        public Esfuerzo(EAUtils.EAUtils eaUtils, Element task)
        {
            this.eaUtils         = eaUtils;
            this.currentTarea    = task;
            this.esfuerzoManager = new EsfuerzoManager(eaUtils);
            this.calendar        = new Calendar(eaUtils);

            this.calendar.load();

            this.ejecutorManager = new EjecutorManager(eaUtils, this.esfuerzoManager, this.calendar);
        }

        public void cargar( Element task )
        {
            this.ejecutorManager.localizar(task);
            this.esfuerzoManager.localizar(task);
        }

        public void calcular(Element element, bool update)
        {
            this.update = update;

            this.calcular(element);
        }

        public void calcular(Element element)
        {
            currentTarea = element;

            this.esfuerzoManager.calcular(currentTarea);

            if ( this.esfuerzoManager.esfuerzo != -1 )
            {
                totalEsfuerzo += this.esfuerzoManager.esfuerzo;

                asignar( this.esfuerzoManager.esfuerzo );
            }
        }
        public void calcular(Package package)
        {
            TareaPriorizada tareaPriorizada = new TareaPriorizada();

            this.recorrerPaquetes(package, tareaPriorizada);

            tareaPriorizada.construirArbol();

            DateTime ultimaFecha = this.ejecutorManager.noComenzarAntesDel;

            foreach (KeyValuePair<string, List<Nodo>> arbolDelEjecutor in tareaPriorizada.arbolPorEjecutor)
            {
                foreach (Nodo nodo in arbolDelEjecutor.Value)
                {
                    /// estas son todas del primer nivel y no dependen de ninguna entonces vamos a 
                    /// programar estas primero y después las volvemos a recorrer para programar el resto.

                    ultimaFecha = calcularPrimerNivel(nodo, ultimaFecha);
                }
            }
            foreach (KeyValuePair<string, List<Nodo>> arbolDelEjecutor in tareaPriorizada.arbolPorEjecutor)
            {
                foreach (Nodo nodo in arbolDelEjecutor.Value)
                {
                    /// estas son todas del primer nivel y no dependen de ninguna entonces vamos a 
                    /// programar estas primero y después las volvemos a recorrer para programar el resto.

                    calcular(nodo, this.ejecutorManager.ejecutor.DateEnd);
                }
            }
            this.eaUtils.printOut("Fin");
        }

        public void recorrerPaquetes( Package package, TareaPriorizada tareaPriorizada)
        {
            // hay que analizar las dependencias de las tareas.
            // si una tarea depende de otra entonces esta comienza cuando termina la otra.
            //
            // después analizar las dependencias de los elementos de las cuales estas dependen.
            // Si al final no hay 1 sólo árbol se define por prioridad, si no hay prioridad se manda error.

            foreach (Element element in package.Elements)
            {
                if (element.Type == "Task")
                {
                    eaUtils.printOut("Task " + element.Name);

                    this.ejecutorManager.localizar(element);

                    // buscamos aquello de lo cual depende esta tarea (deberían ser sólo tareas)
                    List<ElementConnectorInfo> elementsAndConnectors = this.eaUtils.connectorUtils.get(element, EAUtils.ConnectorUtils.CONNECTOR__DEPENDENCY, null, null, null, false, null);

                    tareaPriorizada.add(this.ejecutorManager, element, elementsAndConnectors);
                }
            }

            foreach (Package child in package.Packages)
            {
                this.recorrerPaquetes(child, tareaPriorizada);
            }
        }

        private DateTime calcularPrimerNivel(Nodo nodo, DateTime ultimaFecha)
        {
            if (nodo.esTarea && nodo.programar)
            {
                this.eaUtils.printOut(nodo.element.Name);

                calcular(nodo.element);

                this.ejecutorManager.localizar(nodo.element);

                //this.eaUtils.printOut(ultimaFecha.AddDays(1).ToString());

                this.ejecutorManager.progamar(ultimaFecha.AddDays(1));

                this.ejecutorManager.persistir();

                ultimaFecha = this.ejecutorManager.ejecutor.DateEnd;
            }
            return ultimaFecha;
        }

        private void calcular( Nodo nodo, DateTime ultimaFecha)
        {
            if ( nodo.esTarea && nodo.programar)
            {
                this.eaUtils.printOut(nodo.element.Name);

                calcular(nodo.element );

                this.ejecutorManager.localizar(nodo.element);

                this.eaUtils.printOut(ultimaFecha.AddDays(1).ToString());

                this.ejecutorManager.progamar(ultimaFecha.AddDays(1));

                this.ejecutorManager.persistir();

                ultimaFecha = this.ejecutorManager.ejecutor.DateEnd;
            }

            foreach (Nodo child in nodo.children)
            {
                calcular(child, ultimaFecha);
            }
        }

        private void asignar( float esfuerzo )
        {
            this.esfuerzoManager.asignar(esfuerzo);

            if (this.update)
            {
                this.esfuerzoManager.persistir();
            }

            this.ejecutorManager.asignarEsfuerzo(esfuerzo);

            if (this.update)
            {
                this.ejecutorManager.persistir();
            }
        }

        internal void persistir()
        {
            this.ejecutorManager.persistir();
            this.esfuerzoManager.persistir();
            this.currentTarea.Update();
        }
    }
}
