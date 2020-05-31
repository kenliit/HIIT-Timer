using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace HIT.Helper
{
    public static class SystemBeep
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        public static void ShortBeeps()
        {
            Beep(2000, 400); //high frequency, short sound
        }

        public static void LongBeeps()
        {
            Beep(1000, 1600); //low frequency, longer sound
        }
    }
}
