using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WiimoteLib;
using Microsoft.Xna.Framework;

namespace CgWii1
{
    public class WiiMotesServiceImpl : IWiiMotesService
    {
        #region WiiMote
        Wiimote remote1;
        Wiimote remote2;

        Vector2 readingRemote1;
        Vector2 readingRemote2;

        #endregion

        #region IWiiMotesService Interface Implementation

        #region Properties

        public Vector2 AvgReadingWiiMote1 { get { return readingRemote1; } }
        public Vector2 AvgReadingWiiMote2 { get { return readingRemote2; } }

        public Wiimote WiiMote1 { get { return remote1; } }
        public Wiimote WiiMote2 { get { return remote2; } }

        public int AvailableWiiMotes
        {
            get { return (WiiMote1 == null ? 0 : 1) + (WiiMote1 == null ? 0 : 1); }
        }

        public bool RemotesInitialized
        {
            get { return WiiMote1 != null && WiiMote2 != null; }
        }

        public Exception LastException { get; private set; }
        #endregion

        public void Initialize()
        {
            WiimoteCollection wc = new WiimoteCollection();
            int index = 1;
            try
            {
                //wc.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                LastException = new ApplicationException("Wiimote not found\n" + ex.Message, ex);
                return;
            }
            catch (WiimoteException ex)
            {
                LastException = new ApplicationException("Wiimote error : \n" + ex.Message, ex);
                return;
            }
            catch (Exception ex)
            {
                LastException = new ApplicationException("Unknown error:\n" + ex.Message, ex);
                return;
            }

            foreach (Wiimote wm in wc)
            {
                wm.WiimoteChanged += wm_WiimoteChanged;
                //wm.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;

                wm.Connect();

                // Use Button, accelerometer and IR data
                wm.SetReportType(InputReport.IRAccel, true);
                wm.SetLEDs(index++);

                if (remote1 == null)
                {
                    remote1 = wm;
                }
                else if (remote2 == null)
                {
                    remote2 = wm;
                    break;      //Quit while loop - we already have 2 remotes
                }
            }
        }

        #endregion

        #region Wiimote Handlers

        void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            //var wm = sender as Wiimote;

            //if (wm == null)
            //    return;

            //if (args.Inserted)
            //    wm.SetReportType(Wiimote.InputReport.IRExtensionAccel, true);    // return extension data
            //else
            //    wm.SetReportType(Wiimote.InputReport.IRAccel, true);            // back to original mode
        }
        

        void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            // current state information
            WiimoteState ws = args.WiimoteState;

            //cast sender to a Wiimote
            var remote = sender as Wiimote;
            //Check if this is a reading from remote1 or 2 and select point to update in accordance

            //Get first valid reading (have both x and y)
            //TODO: maybe take average or something...
            //TODO: maybe checking found is enough?
            //TODO: maybe use mid point (of IR state)

            //Update mid-point reading
            var validIr = ws.IRState.IRSensors.Where(ir => ir.Found);

            //Check that we got some valid reading and update point if we did
            if (validIr.Count() > 0)
            {
                if (remote == remote1)
                {
                    readingRemote1.X = validIr.Average(ir => (float)ir.RawPosition.X);
                    readingRemote1.Y = validIr.First().RawPosition.Y;
                }
                else
                {
                    readingRemote2.X = validIr.Average(ir => (float)ir.RawPosition.X);
                    readingRemote2.Y = validIr.First().RawPosition.Y;
                }
            }
        }

       

        #endregion
    }
}
