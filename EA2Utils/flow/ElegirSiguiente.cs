using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EAUtils.flow
{
    public interface ElegirSiguiente
    {
        void elegir(EA.Element actual, List<string> posiblesSiguientesGuardas, List<object> posiblesSiguiente, ManualResetEvent mre);

        int elegido();
    }
}
