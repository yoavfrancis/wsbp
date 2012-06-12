#region Using Statements
using Microsoft.Xna.Framework;
#endregion

using GameStateManagement;
using CgWii1.Screens;
using CgWii1.Demos;

namespace CgWii1
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Create our menu entries.
            DemoTypes[] demoTypes = { DemoTypes.PointIn3D, DemoTypes.SensorBarIn3D, DemoTypes.SensorBarIn3DWithSize, DemoTypes.WiiMoteRelativePositions, DemoTypes.Draw3D };
            string[] demoNames = { "Point in 3D", "Sensor Bar in 3D", "Sensor Bar in 3D w/Size", "WiiMote Relative Position", "3D Draw" };

            for (int i = 0; i < demoTypes.Length; i++)
            {
                MenuEntry me = new MenuEntry(string.Format("{0}) {1}", i + 1, demoNames[i]))
                {
                    DemoTypeToRun = demoTypes[i]
                };

                me.Selected += new System.EventHandler<PlayerIndexEventArgs>(me_Selected);

                MenuEntries.Add(me);
            }

            //TODO: 
            var svc = ScreenManager.Game.Services.GetService(typeof(IWiiMotesService)) as IWiiMotesService;

            if (svc == null)
            {
                //Some error...
            }
            else if (svc.AvailableWiiMotes < 2)
            {
                //Add option to try to re-initialize the wiimote service...
                MenuEntry reInitiazlieWiiMotes = new MenuEntry("Re-initialize WiiMotes") { IsEnabled = true };
                reInitiazlieWiiMotes.Selected += new System.EventHandler<PlayerIndexEventArgs>(reInitiazlieWiiMotes_Selected);
                //disable all the other entries
                foreach (var entry in MenuEntries)
                {
                    //entry.IsEnabled = false;
                }

                MenuEntries.Add(reInitiazlieWiiMotes);
            }


            MenuEntry exitMenuEntry = new MenuEntry("Exit") { IsEnabled = true };
            exitMenuEntry.Selected += OnCancel;

            // Hook up menu event handlers.
            MenuEntries.Add(exitMenuEntry);
        }


        void reInitiazlieWiiMotes_Selected(object sender, PlayerIndexEventArgs e)
        {
            //TODO: make a screen
            ScreenManager.AddScreen(new ReInitializeWiiScreen(), e.PlayerIndex);
        }

        #endregion

        #region Handle Input

        void me_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Start the demo by calling LoadingScreen.Load...
            var me = sender as MenuEntry;

            if (me == null)
                return;

            GameScreen screenToLoad = null;

            switch (me.DemoTypeToRun)
            {
                case DemoTypes.PointIn3D:
                    screenToLoad = new PointIn3dDemo();
                    break;
                case DemoTypes.SensorBarIn3D:
                    screenToLoad = new SensorBarIn3dDemo();
                    break;
                case DemoTypes.SensorBarIn3DWithSize:
                    break;
                case DemoTypes.WiiMoteRelativePositions:
                    break;
                case DemoTypes.Draw3D:
                    screenToLoad = new Draw3dDemo();
                    break;
                default:
                    break;
            }

            if (screenToLoad == null)
                return;

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, screenToLoad);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
    }
}
