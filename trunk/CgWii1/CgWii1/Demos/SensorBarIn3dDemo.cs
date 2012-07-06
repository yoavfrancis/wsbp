using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CgWii1.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WiimoteLib;
using System.Diagnostics;

namespace CgWii1.Demos
{
    public class SensorBarIn3dDemo : PointIn3dDemo
    {
        #region Fields

        Model sensorBarModel;
        Vector3 modelPosition = new Vector3(0f,0.0f, -0.0f);
        float curModelRoll = 0.0f;
        float curModelYaw = 0.0f;
        Quaternion modelRotation = Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver4, 0, 0.0f);
        double z1=0, z2=0;

        #endregion

        public SensorBarIn3dDemo()
        {
            Draw3dPoint = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            ScreenManager.Game.Window.Title = "3D Point with 2 perpendicular wiimotes";
            sensorBarModel = Content.Load<Model>("sensorbar");
        }

        #region Load Content Methods

        #endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void  Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                ExitScreen();

            #region Update the Model Position

            float offset = 0.5f * projectionPointSize;

            float x = wiiService.AvgReadingWiiMote1.X - offset;
            float y = wiiService.AvgReadingWiiMote1.Y - offset;
            float z = width - wiiService.AvgReadingWiiMote2.X - offset;

            Vector3 pos = new Vector3(x / 512, y / 384, -z / 512);
            modelPosition = pos; 
            #endregion            

            #region Update the model's orientation

            //Get the "most extreme" points from each remote
            if (wiiService.WiiMote1 != null)
            {
                var foundSensors = wiiService.WiiMote1.WiimoteState.IRState.IRSensors.Where(r => r.Found);

                if (foundSensors.Count() >= 2)
                {
                    /**
                     * tan(a) = dy/dx --> a (roll degree) = atan(dy/dx)
                     * Doesn't matter if we look on the first or the second wiimote.
                     */

                    var r1Min = foundSensors.OrderBy(s => s.RawPosition.X).First();
                    var r1Max = foundSensors.OrderBy(s => s.RawPosition.X).Last();

                    //We have the data for "roll"
                    Vector2 relPos = new Vector2(r1Max.RawPosition.X - r1Min.RawPosition.X, r1Max.RawPosition.Y - r1Min.RawPosition.Y);

                    curModelRoll = (float)Math.Atan2(relPos.Y, relPos.X);


                    //Debug.WriteLine(MathHelper.ToDegrees(curModelRoll));
                }
            }

            if (wiiService.WiiMote1 != null && wiiService.WiiMote2 != null)
            {
                var foundSensors1 = wiiService.WiiMote1.WiimoteState.IRState.IRSensors.Where(r => r.Found);
                var foundSensors2 = wiiService.WiiMote2.WiimoteState.IRState.IRSensors.Where(r => r.Found);

                int numSensors1 = foundSensors1.Count();
                int numSensors2 = foundSensors2.Count();

                //If only 1 remote visible - then guess rotation to the right.

                // both remotes can see the sensor.
                if (foundSensors1.Count() >= 2 && foundSensors2.Count() >= 2)
                {
                    var r1Min = foundSensors1.OrderBy(s => s.RawPosition.X).First();
                    var r1Max = foundSensors1.OrderBy(s => s.RawPosition.X).Last();
                    var r2Min = foundSensors2.OrderBy(s => s.RawPosition.X).First();
                    var r2Max = foundSensors2.OrderBy(s => s.RawPosition.X).Last();


                    float dx1 = r1Max.RawPosition.X - r1Min.RawPosition.X;
                    float dx2 = r2Max.RawPosition.X - r2Min.RawPosition.X;
                    
                    // We have the x distance on both remotes.
                    curModelYaw = -(float)Math.Atan2(dx2,dx1);

                    //// Computer distances.
                    double angularFOV = MathHelper.ToRadians(41.0f) / 1024; ;
                    double r1 = Math.Pow((double)(r1Min.RawPosition.X - r1Max.RawPosition.X), 2.0) +
                               Math.Pow((double)(r1Min.RawPosition.Y - r1Max.RawPosition.Y), 2.0);
                    r1 = Math.Sqrt(r1);

                    double alpha1 = r1 * angularFOV / 2.0;
                    z1 = 22.0 * Math.Cos(Math.Abs(curModelYaw)) / (2 * Math.Tan(alpha1));
                    
                    //##########################

                    double r2 = Math.Pow((double)(r2Min.RawPosition.X - r2Max.RawPosition.X), 2.0) +
                    Math.Pow((double)(r2Min.RawPosition.Y - r2Max.RawPosition.Y), 2.0);
                    r2 = Math.Sqrt(r2);

                    double alpha2 = r2 * angularFOV / 2.0;
                    z2 = 22.0 * Math.Sin(Math.Abs(curModelYaw)) / (2 * Math.Tan(alpha2));

                    

                    //Debug.WriteLine(MathHelper.ToDegrees(curModelYaw));
                }
            }

            modelRotation = Quaternion.CreateFromYawPitchRoll(curModelYaw, 0f,curModelRoll);
            #endregion

  



        



            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(GameStateManagement.InputState input)
        {
            base.HandleInput(input);
            angle = 0;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            DrawModel();
        }

        #region Draw Methods


        private void DrawModel()
        {

            Matrix[] transforms = new Matrix[sensorBarModel.Bones.Count];
            sensorBarModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in sensorBarModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect localEffect = effect as BasicEffect;
                        localEffect.EnableDefaultLighting();

                        localEffect.World = transforms[mesh.ParentBone.Index] *
                                            Matrix.CreateScale(0.15f) *
                                            Matrix.CreateFromQuaternion(modelRotation) * //Roll around object self midpoint.
                                            Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(curModelYaw), 0,0) * //Yaw around world's Y axis                                            
                                            Matrix.CreateTranslation(modelPosition);

                        localEffect.View = Matrix.CreateLookAt(cameraPosition,
                                                          Vector3.Zero, Vector3.Up);

                        localEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio,
                            1.0f, 10000.0f);

                    }

                }
                mesh.Draw();
            }
        }

        protected override void DrawLocationStats()
        {
            //if (wiiService.AvailableWiiMotes < 2)
            //{
            //    return;
            //}

            try
            {
                spriteBatch.Begin();

                int x = (int)wiiService.AvgReadingWiiMote1.X;
                int y = (int)(wiiService.AvgReadingWiiMote1.Y);
                int z = wiiService.WiiMote2 != null ? (int)(width - wiiService.AvgReadingWiiMote2.X) : 0;

                string msg = String.Format("X:{0,4} , Y:{1,4}, Z:{2,4}, Roll:{3,6}, Yaw:{4,6}", x, y, z, 
                                           MathHelper.ToDegrees(curModelRoll).ToString("0.00"),
                                           MathHelper.ToDegrees(curModelYaw).ToString("0.00"));
                spriteBatch.DrawString(promptFont,
                                        msg,
                                        new Vector2(20, GraphicsDevice.Viewport.Height - 40),
                                        Color.Yellow);

                msg = String.Format("Z1:{0,4}, Z2:{1,4}", z1, z2);
                spriteBatch.DrawString(promptFont,
                        msg,
                        new Vector2(20, GraphicsDevice.Viewport.Height - 80),
                        Color.Yellow);

            }
            finally
            {
                spriteBatch.End();
            }
        }

        #endregion
    }
}
