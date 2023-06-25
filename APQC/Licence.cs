using LicenceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APQC
{
    class Licence : LicenceInformation
    {
        private const string TRIAL = "Trial";
        private const string STANDARD = "Standard";
        private const string PERPETUAL = "Perpetual";

        private const int TRIAL_DAYS = 30;
        private const int STANDARD_DAYS = 365;
        private const int PERPETUAL_DAYS = 3650;
        private TimeSpan remains;

        Dictionary<string[], string> LicenceInformation.licences()
        {
            Dictionary<string[], string> licences = new Dictionary<string[], string>();

            string[] trials = new string[]
            {
                ""
            };

            licences.Add(trials, TRIAL);

            string[] standards = new string[]
            {
                "3a545b67-9fba-4a6b-8586-c35fb6ff2874",
                "cd7ee067-ee7c-4560-9a3c-37e5a69d5669",
                "dc8b2aea-df7b-48ca-877c-1f944b157d75"
            };

            licences.Add(standards, STANDARD);

            /*
             * Esta es sólo una muestra, no conviene una licencia taaaaan larga.
            string[] perpetuals = new string[]
            {
                "1"
            };
            licences.Add(perpetuals, PERPETUAL);
            */

            return licences;
        }

        Dictionary<string, long> LicenceInformation.typesTimes()
        {
            Dictionary<string, long> typesTimes = new Dictionary<string, long>();

            // en la linea de abajo se suman 30 días a la fecha del momento que consulta este método.
            // esto se debería hacer sólo cuando se registran las licencias.

            // la clave de este diccionario debe coincidir con el valor del otro diccionario, 
            // la idea es que se busquen las licencias y cuando estas coincidan se busca por el valor
            // en este diccionario que retorna el tiempo total de vigencia de la licencia.

            typesTimes.Add(TRIAL, (DateTime.Now + (new TimeSpan(TRIAL_DAYS, 0, 0, 0))).Ticks);
            typesTimes.Add(STANDARD, (DateTime.Now + (new TimeSpan(STANDARD_DAYS, 0, 0, 0))).Ticks);
            typesTimes.Add(PERPETUAL, (DateTime.Now + (new TimeSpan(PERPETUAL_DAYS, 0, 0, 0))).Ticks);

            return typesTimes;
        }

        public void timeRemaining(TimeSpan timeSpan)
        {
            this.remains = timeSpan;
        }
        public string timeRemaining()
        {
            return this.remains.ToString("%d") +" "+ Properties.Resources.DIAS;
        }
    }
}
