using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CgWii1
{
    public interface IWiiMotesService
    {
        void Initialize();

        Vector2 AvgReadingWiiMote1 { get; }
        Vector2 AvgReadingWiiMote2 { get; }

        WiimoteLib.Wiimote WiiMote1 { get; }
        WiimoteLib.Wiimote WiiMote2 { get; }

        int AvailableWiiMotes { get; }

        bool RemotesInitialized { get; }

        Exception LastException { get; }
    }
}
