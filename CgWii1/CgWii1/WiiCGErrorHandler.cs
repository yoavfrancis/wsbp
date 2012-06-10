using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CgWii1
{
    public class WiiCGErrorHandler : Microsoft.Xna.Framework.Game
    {
        public string ErrorText = "No Error Found";
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont ErrorFont;

        public WiiCGErrorHandler()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// 
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// 
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// 
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// 
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ErrorFont = Content.Load<SpriteFont>("ErrorFont");
            // TODO: use this.Content to load your game content here
        }

        /// 
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// 
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        Color warningColor = Color.Yellow;
        double lastUpdate = 0.0;

        /// 
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// 
        /// <param name="gameTime">Provides a snapshot of timing values.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Change warning color every 100msec
            if (gameTime.TotalGameTime.TotalMilliseconds- lastUpdate > 100)
            {
                warningColor = warningColor == Color.Yellow ? Color.Orange : Color.Yellow;
                lastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// 
        /// This is called when the game should draw itself.
        /// 
        /// <param name="gameTime">Provides a snapshot of timing values.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.DrawString(ErrorFont, "An Error occured in your CGWii, the error is as follows :", Vector2.Zero, Color.Yellow);
            spriteBatch.DrawString(ErrorFont, ErrorText, new Vector2(0, 40), warningColor);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
