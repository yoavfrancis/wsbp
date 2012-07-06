using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

using GameStateManagement;

namespace CgWii1
{
    public class WsbpDemo : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        
        #endregion

        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public WsbpDemo()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 940;
            graphics.PreferredBackBufferHeight = 735;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            //TODO: replace when running
            //screenManager.AddScreen(new MainMenuScreen(), null);
            screenManager.AddScreen(new Demos.SensorBarIn3dDemo(), PlayerIndex.One);
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion

    }
}
