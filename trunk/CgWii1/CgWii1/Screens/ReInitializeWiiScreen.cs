using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameStateManagement;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CgWii1.Screens
{
    public class ReInitializeWiiScreen : DemoBaseScreen
    {
        private enum InitState
        {
            None,
            Trying,
            Waiting,
            Failed
        }

        #region Fields

        private const int MAX_RETRIES = 5;              //Maximum retries
        private const int TIME_BETWEEN_RETRIES = 2000;  //The time (msec) between retries
        private const int STATE_CHANGE_DELAY = 400;     //The delay to keep message after state change

        int actualRetries = 0;      //Internal counter of actual retires
        double nextRetry = 0;       //Internal variable to hold next retry time
        double lastRetry = 0;       //Internal variable to hold last retry time

        //Some static strings
        private static string PROMPT = "Trying to Initialize WiiMotes";
        private static string TRY_MSG = "Trying to initialize...";
        private static string FAIL_INIT = "Failed to initialize WiiMotes";
        private static string RETRY_MSG = "Press R to try again";
        private static string QUIT_MSG = "Press Escape for Main Menu";

        IWiiMotesService wiiSvc;    //WiiMotesService instance to use. The value if read from ScreenManager

        InitState currentState = InitState.None;    //Hold internal state of re-initialization

        Vector2 promptPosition;     //A variable for prompt position (not to calculate each time)
        SpriteFont font;            //A variable for font. Read from ScreenManager

        //Used for flashing text
        private static Color WARN_COLOR_1 = Color.LightGray;
        private static Color WARN_COLOR_2 = Color.LightGray;
        private Color warningColor = WARN_COLOR_2;
        private double lastFlashColorChange = 0.0; 
        #endregion

        public override void LoadContent()
        {
            base.LoadContent();
            wiiSvc = ScreenManager.Game.Services.GetService(typeof(IWiiMotesService)) as IWiiMotesService;
            
            font = ScreenManager.Font;

            float x = ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(PROMPT).X / 2;

            promptPosition = new Vector2(x, 40);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Get the current total time
            double currentTotalTime = gameTime.TotalGameTime.TotalMilliseconds;            

            //Check if already tried all the allowed times
            if (actualRetries < MAX_RETRIES)
            {                
                //Can still retry - check if it is time of another retry
                if (nextRetry < currentTotalTime)
                {
                    //Change state to trying
                    currentState = InitState.Trying;

                    //Do the actual initialization
                    wiiSvc.Initialize();

                    //Increment the retry counter
                    actualRetries++;

                    //Updatre the last retry time (used for drawing messages)
                    lastRetry = currentTotalTime;
                    //Update the next retry time
                    nextRetry = currentTotalTime + TIME_BETWEEN_RETRIES;
                }
                else if (currentTotalTime > lastRetry + STATE_CHANGE_DELAY)
                {
                    //Wait STATE_CHANGE_DELAY msec before changing back to "waiting" state
                    currentState = InitState.Waiting;
                }
            }
            else
            {
                //delay the change of state and then change it 
                if (currentTotalTime > lastRetry + STATE_CHANGE_DELAY)
                    currentState = InitState.Failed;

                //Update the flashing warning color every STATE_CHANGE_DELAY msec
                if (currentTotalTime - lastFlashColorChange > STATE_CHANGE_DELAY)
                {
                    //Update the color of the flashing warning text
                    warningColor = warningColor == WARN_COLOR_1 ? WARN_COLOR_2 : WARN_COLOR_1;
                    //Save the last change to control flashing
                    lastFlashColorChange = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            PlayerIndex pIdx = PlayerIndex.One;

            //See if R key is newly pressed
            if (input.IsNewKeyPress(Keys.R, null, out pIdx))
            {
                //Reset all state variables
                ResetState();
            }
        }
     
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, PROMPT, promptPosition, Color.White, 0, origin, 1f, SpriteEffects.None, 0);

            string tryCount = string.Format("Initialization retry {0} of {1}", actualRetries, MAX_RETRIES);

            if (currentState != InitState.Failed)
            {
                spriteBatch.DrawString(font, tryCount, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(tryCount).X / 2, 80), Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            }

            if (currentState == InitState.Waiting)
            {
                //Print the time until next try
                string message = string.Format("Will try again in {0,3} ms", (nextRetry - gameTime.TotalGameTime.TotalMilliseconds).ToString("0"));

                spriteBatch.DrawString(font, message, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(message).X/2, 120), Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            }
            else if (currentState == InitState.Trying)
            {
                //Print that trying
                spriteBatch.DrawString(font, TRY_MSG, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(TRY_MSG).X / 2, 120), Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            }
            else if (currentState == InitState.Failed)
            {
                spriteBatch.DrawString(font, FAIL_INIT, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(FAIL_INIT).X / 2, 120), warningColor, 0, origin, 1f, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, RETRY_MSG, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(RETRY_MSG).X / 2, 240), Color.White, 0, origin, 1f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, QUIT_MSG, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - font.MeasureString(QUIT_MSG).X / 2, 280), Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        private void ResetState()
        {
            actualRetries = 0;
            lastRetry = 0;
            lastFlashColorChange = 0;
            nextRetry = 0;
            currentState = InitState.None;
        }

    }
}
