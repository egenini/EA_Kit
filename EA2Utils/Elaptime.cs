using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    public class Elaptime
    {
        Stopwatch stopWatch = new Stopwatch();
        
        public Elaptime()
        {
            stopWatch.Start();
        }

        public string stop()
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }
    }
}
