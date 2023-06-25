using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement
{
    /// <summary>
    /// El esfuerzo calculado es cuando se obtiene desde la complejidad y la dificultad, en otro caso
    /// el esfuerzo es establecido de forma manual en el elemento effort.
    /// </summary>
    public class EsfuerzoManager
    {
        private EAUtils.EAUtils eaUtils;
        public Effort currentEsfuerzo = null;
        public float esfuerzo = -1;
        public bool tieneEsfuerzo;

        public EsfuerzoManager(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public bool esCalculado()
        {
            return currentEsfuerzo.Name == Properties.Resources.ESFUERZO_CALCULADO;
        }

        public void calcular(Element tarea)
        {
            localizar(tarea);

            if ( this.esCalculado() )
            {
                eaUtils.printOut("Complexity " + tarea.Complexity + " Difficulty " + tarea.Difficulty);

                if (tarea.Complexity == "1" && (tarea.Difficulty == "Low" || tarea.Difficulty == "Bajo"))
                {
                    esfuerzo = (float)2.00;
                }
                else
                    if (tarea.Complexity == "1" && (tarea.Difficulty == "Medium" || tarea.Difficulty == "Mediano" || tarea.Difficulty == "Medio"))
                {
                    esfuerzo = (float)16.00;
                }
                else
                    if (tarea.Complexity == "1" && (tarea.Difficulty == "High" || tarea.Difficulty == "Alto"))
                {
                    esfuerzo = (float)40.00;
                }
                else
                if (tarea.Complexity == "2" && (tarea.Difficulty == "Low" || tarea.Difficulty == "Bajo"))
                {
                    esfuerzo = (float)4.00;
                }
                else
                    if (tarea.Complexity == "2" && (tarea.Difficulty == "Medium" || tarea.Difficulty == "Mediano" || tarea.Difficulty == "Medio"))
                {
                    esfuerzo = (float)24.00;
                }
                else
                    if (tarea.Complexity == "2" && (tarea.Difficulty == "High" || tarea.Difficulty == "Alto"))
                {
                    esfuerzo = (float)80.00;
                }
                else
                if (tarea.Complexity == "3" && (tarea.Difficulty == "Low" || tarea.Difficulty == "Bajo"))
                {
                    esfuerzo = (float)8.00;
                }
                else
                    if (tarea.Complexity == "3" && (tarea.Difficulty == "Medium" || tarea.Difficulty == "Mediano" || tarea.Difficulty == "Medio"))
                {
                    esfuerzo = (float)32.00;
                }
                else
                    if (tarea.Complexity == "3" && (tarea.Difficulty == "High" || tarea.Difficulty == "Alto"))
                {
                    esfuerzo = (float)160.00;
                }
                else
                {
                    esfuerzo = this.currentEsfuerzo.Weight;
                }
            }
        }

        public bool inferir( Element tarea, string esfuerzo)
        {
            bool inferido = false;
            if (esfuerzo == "2")
            {
                tarea.Complexity = "1";
                tarea.Difficulty = "Low";
                inferido = true;
            }
            else if (esfuerzo == "4")
            {
                tarea.Complexity = "2";
                tarea.Difficulty = "Low";
                inferido = true;
            }
            else if (esfuerzo == "8")
            {
                tarea.Complexity = "3";
                tarea.Difficulty = "Low";
                inferido = true;
            }
            else if (esfuerzo == "16")
            {
                tarea.Complexity = "1";
                tarea.Difficulty = "Medium";
                inferido = true;
            }
            else if (esfuerzo == "24")
            {
                tarea.Complexity = "2";
                tarea.Difficulty = "Medium";
                inferido = true;
            }
            else if (esfuerzo == "32")
            {
                tarea.Complexity = "3";
                tarea.Difficulty = "Medium";
                inferido = true;
            }
            else if (esfuerzo == "40")
            {
                tarea.Complexity = "1";
                tarea.Difficulty = "High";
                inferido = true;
            }
            else if (esfuerzo == "80")
            {
                tarea.Complexity = "2";
                tarea.Difficulty = "High";
                inferido = true;
            }
            else if (esfuerzo == "160")
            {
                tarea.Complexity = "3";
                tarea.Difficulty = "High";
                inferido = true;
            }
            return inferido;
        }

        public bool localizar( Element tarea )
        {
            if( this.currentEsfuerzo == null && tarea.Efforts.Count == 1 )
            {
                this.currentEsfuerzo = tarea.Efforts.GetAt(0);
                this.esfuerzo = this.currentEsfuerzo.Time;
            }

            return tarea.Efforts.Count != 0;
        }

        public void agregar(Element tarea, bool calculada )
        {
            this.currentEsfuerzo = tarea.Efforts.AddNew(Properties.Resources.ESFUERZO_CALCULADO, "");

            this.calcular(tarea);
        }

        public void asignar( float esfuerzo )
        {
            this.esfuerzo = esfuerzo;

            this.currentEsfuerzo.Time   = esfuerzo;
            // por algún motivo desconocido EA muestra este valor como tiempo y tiempo no aparece
            this.currentEsfuerzo.Weight = (int)esfuerzo;
        }

        public void persistir()
        {
            this.currentEsfuerzo.Update();
        }

        internal void setNoCalculado(decimal value)
        {
            this.asignar((float)value);
            this.currentEsfuerzo.Name = Properties.Resources.ESFUERZO_MANUAL;
        }
    }
}
