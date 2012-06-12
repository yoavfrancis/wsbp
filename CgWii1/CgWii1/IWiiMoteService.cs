using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CgWii1
{
    public interface IWiiMotesService
    {
        /// <summary>
        /// Initialize the WiiMoteService setting WiiMotes if possible
        /// </summary>
        /// <remarks>In case exception arrise the initialize catches it and set LastException property</remarks>
        void Initialize();

        /// <summary>
        /// Get the average IR RawPosition reading of WiiMote1
        /// </summary>
        Vector2 AvgReadingWiiMote1 { get; }

        /// <summary>
        /// Get the average IR RawPosition reading of WiiMote1
        /// </summary>
        Vector2 AvgReadingWiiMote2 { get; }

        /// <summary>
        /// Get WiiMote1
        /// </summary>
        /// <remarks>May be null</remarks>
        WiimoteLib.Wiimote WiiMote1 { get; }

        /// <summary>
        /// Get WiiMote1
        /// </summary>
        /// <remarks>May be null</remarks>
        WiimoteLib.Wiimote WiiMote2 { get; }

        /// <summary>
        /// Get the number of available WiiMotes (up to 2)
        /// </summary>
        int AvailableWiiMotes { get; }

        /// <summary>
        /// Get remotes initialization status
        /// </summary>
        /// <remarks>Return false if no WiiMote (1/2) was found/initialized</remarks>
        bool RemotesInitialized { get; }

        /// <summary>
        /// Get the last exception to occur. Used after calling initialize
        /// </summary>
        Exception LastException { get; }

        /// <summary>
        /// Get whether IR state is available from WiiMote1
        /// </summary>
        /// <remarks>Check that the <code>WiiMote1</code> is not null and at least on of its <code>IRState.IRSensors.Found = true</code></remarks>
        bool WiiMote1IrFound { get; }

        /// <summary>
        /// Get whether IR state is available from WiiMote1
        /// </summary>
        /// <remarks>Check that the <code>WiiMote2</code> is not null and at least on of its <code>IRState.IRSensors.Found = true</code></remarks>
        bool WiiMote2IrFound { get; }
    }
}
