using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIResources;

namespace ProjectManagement
{
    /// <summary>
    /// Responsable de asignar o reemplazar un ejecutor si este fuera anónimo.
    /// </summary>
    public class EjecutorManager
    {
        public const string EJECUTOR_ANONIMO = "anon";

        private DateTime eneroDelSetenta = DateTime.Parse("1970-01-01");
        public DateTime noComenzarAntesDel = DateTime.Now;

        private Dictionary<int, bool> paqueteContieneTVNoComenzarAntesDel = new Dictionary<int, bool>();

        private EAUtils.EAUtils eaUtils;

        public Resource ejecutor = null;
        public EsfuerzoManager esfuerzoManager = null;
        private int workingHours = 8;
        private Calendar calendar;

        public EjecutorManager( EAUtils.EAUtils eaUtils)
        {
            this.eaUtils         = eaUtils;
            this.esfuerzoManager = new EsfuerzoManager(eaUtils);

            
        }

        public EjecutorManager(EAUtils.EAUtils eaUtils, EsfuerzoManager esfuerzoManager, Calendar calendar)
        {
            this.eaUtils         = eaUtils;
            this.esfuerzoManager = esfuerzoManager;
            this.calendar        = calendar;
        }

        public int getWorkingHours()
        {
            return this.ejecutor.Time != 0 ? this.ejecutor.Time : this.workingHours;
        }

        private void buscarNoComenzarAntesDel(int packageId)
        {
            Package package = this.eaUtils.repository.GetPackageByID(packageId);

            if( package.Element != null )
            {
                string noIAD = this.eaUtils.taggedValuesUtils.get(package.Element, "No iniciar antes del", "").asString();

                if (noIAD == "")
                {
                    buscarNoComenzarAntesDel(package.ParentID);
                }
                else
                {
                    this.noComenzarAntesDel = DateTime.Parse(noIAD);
                }
            }
        }

        public bool localizar(Element task)
        {
            buscarNoComenzarAntesDel(task.PackageID );
            

            if (task.Resources.Count == 1)
            {
                this.ejecutor = task.Resources.GetAt(0);
            }
            else if(task.Resources.Count > 1){

                // si hay más de 1 hay que elegir que tenga la fecha de fin más alta.
                Resource compateTo = task.Resources.GetAt(0);

                foreach ( Resource resource in task.Resources)
                {
                    if( resource.DateEnd.CompareTo(compateTo) == 1)
                    {
                        compateTo = resource;
                    }  
                }
                
                this.ejecutor = compateTo;
            }

            return task.Resources.Count != 0;
        }

        public bool agregar(Element task)
        {
            return this.agregar(task, EJECUTOR_ANONIMO);
        }

        public bool agregar(Element task, string name)
        {
            this.ejecutor = task.Resources.AddNew(name, "Resource");

            this.ejecutor.Update();

            return true;
        }

        public void asignarEjecutor(Element task, string name)
        {
            if (this.localizar(task))
            {
                if (this.ejecutor.Name.ToLower() == EJECUTOR_ANONIMO)
                {
                    this.ejecutor.Name = name;

                    this.ejecutor.Update();

                    Alert.Success(Properties.Resources.EJECUTOR_REEMPLAZO_ANONIMO);
                    this.eaUtils.printOut(Properties.Resources.EJECUTOR_REEMPLAZO_ANONIMO);
                }
            }
            else
            {
                this.agregar(task, name);

                if( this.esfuerzoManager != null)
                {
                    if( this.esfuerzoManager.localizar(task))
                    {
                        this.asignarEsfuerzo(this.esfuerzoManager.esfuerzo);
                    }
                }
            }
        }

        public void asignarEjecutor( Element task, Element actor)
        {
            if( this.localizar(task))
            {
                if (this.ejecutor.Name.ToLower() == EJECUTOR_ANONIMO)
                {
                    this.ejecutor.Name = actor.Name;

                    this.persistir();

                    Alert.Success(Properties.Resources.EJECUTOR_REEMPLAZO_ANONIMO);
                    this.eaUtils.printOut(Properties.Resources.EJECUTOR_REEMPLAZO_ANONIMO);
                }
            }
            else
            {
                this.agregar(task, actor.Name);

                if ( this.esfuerzoManager != null && this.esfuerzoManager.localizar( task ))
                {
                    this.asignarEsfuerzo(this.esfuerzoManager.esfuerzo);
                }

                this.persistir();

                Alert.Success(Properties.Resources.EJECUTOR_CREADO + " " + this.ejecutor.Name);
                this.eaUtils.printOut(Properties.Resources.EJECUTOR_CREADO + " " + this.ejecutor.Name);
            }
        }

        public void asignarEsfuerzo( float asignarEsfuerzo )
        {
            if( this.ejecutor.ExpectedHours != (int)asignarEsfuerzo )
            {
                this.ejecutor.ExpectedHours = (int)asignarEsfuerzo;
                //this.ejecutor.Time          = this.ejecutor.ExpectedHours;

                this.calcularPorcentualAvance(asignarEsfuerzo);

                this.progamar();
            }
        }

        public void desprogramar()
        {
            this.ejecutor.DateStart = this.eneroDelSetenta;
            this.ejecutor.DateEnd   = this.eneroDelSetenta;
        }

        public void progamar( )
        {
            this.progamar(this.noComenzarAntesDel);
        }

        public void progamar(DateTime inicio)
        {
            this.ejecutor.DateStart = this.calendar.moverSi( inicio );

            if( this.ejecutor.ExpectedHours > this.getWorkingHours())
            {
                // en este caso hay que analizar desde el día después del inicio hasta el final, día por día.
                double days = this.ejecutor.ExpectedHours / this.getWorkingHours();

                /// ponemos tentativamente esta fecha y más abajo la corregimos por fin de semaana o feriado.
                this.ejecutor.DateEnd = this.ejecutor.DateStart.AddDays(days);

                this.ejecutor.DateEnd = this.calendar.moverSi(this.ejecutor.DateStart, this.ejecutor.DateEnd, (int) days);
            }
            else
            {
                this.ejecutor.DateEnd = this.calendar.moverSi( this.ejecutor.DateStart.AddHours(this.ejecutor.ExpectedHours) );
            }
        }

        public void persistir()
        {
            this.ejecutor.Update();
        }

        public void actualizarHorasUtilizadas(decimal value)
        {
            this.ejecutor.ActualHours = (int)value;

            //this.actualizarPorcentualAvance(this.ejecutor.ActualHours);

            this.calcularPorcentualAvance(this.esfuerzoManager.esfuerzo);
        }

        private void calcularPorcentualAvance(float asignarEsfuerzo)
        {
            double completado                = (double)this.ejecutor.ActualHours / (double)asignarEsfuerzo;
            int percentComplete           = (int) (completado * 100);
            this.ejecutor.PercentComplete = percentComplete > 100 ? 100 : percentComplete;
            //this.ejecutor.PercentComplete = completado;
        }

        internal void actualizarPorcentualAvance(decimal avance)
        {
            // actualizamos las horas utilizadas.
            this.ejecutor.ActualHours = (int)((avance / 100) * (int)this.esfuerzoManager.esfuerzo);
        }

        public void actualizarTiempoAsignado( decimal horas)
        {
            this.ejecutor.Time = (int)horas;

            progamar(this.ejecutor.DateStart);
        }
    }
}
