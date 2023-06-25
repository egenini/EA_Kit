using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenceManager
{
    public interface LicenceInformation
    {
        Dictionary<string[], string> licences();
        Dictionary<string, long> typesTimes();
        void timeRemaining(TimeSpan timeSpan);
        string timeRemaining();
    }
}
