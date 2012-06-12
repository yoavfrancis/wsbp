using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WiimoteLib;

namespace CgWii1
{
    public static class WiiMoteIrExtensions
    {
        public static bool FoundAnyIrs(this Wiimote wm)
        {
            return wm.WiimoteState.IRState.IRSensors.Count(ir => ir.Found) > 0;
        }
    }
}
