using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WiimoteLib;

using CgWii1.Screens;

namespace CgWii1.Demos
{
    public class PointIn3dDemo : DemoBaseScreen
    {
        #region Fields
        protected SpriteBatch spriteBatch;
        protected SpriteFont promptFont;

        //Drawing effects HSLS
        protected Effect effect;

        //View/projection
        protected Matrix viewMatrix;
        protected Matrix projectionMatrix;

        //TODO: update SetUpWallVertices to support 1024 x 768 then change values here
        protected const int width = 1024;
        protected const int height = 768;

        private Texture2D tileTexture;
        private Texture2D readingTexture;

        protected float projectionPointSize = 40;

        protected Vector3 cameraPosition = new Vector3(5, 5, -5);

        protected float angle;

        private VertexBuffer wallVerticesBuffer;

        protected IWiiMotesService wiiService;
        #endregion

        public PointIn3dDemo()
        {
            Draw3dPoint = true;
        }

        #region Properties
        public bool Draw3dPoint { get; set; }
        #endregion



        #region Overrides

        public override void LoadContent()
        {
            base.LoadContent();            

            ScreenManager.Game.Window.Title = "3D Point with 2 perpendicular wiimotes";
            wiiService = ScreenManager.Game.Services.GetService(typeof(IWiiMotesService)) as IWiiMotesService;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(ScreenManager.Game.GraphicsDevice);

            effect = Content.Load<Effect>("effects");

            tileTexture = Content.Load<Texture2D>("WallTile");
            readingTexture = Content.Load<Texture2D>("reading");
            promptFont = Content.Load<SpriteFont>("StatsFont");
            SetUpCamera();

            SetUpWallsVerticesEx();
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            
            // TODO: Add your update logic here
            //Check for page up/down
            KeyboardState keyState = input.CurrentKeyboardStates[0];

            if (keyState.IsKeyDown(Keys.PageUp))
            {
                angle += 0.05f;
            }
            if (keyState.IsKeyDown(Keys.PageDown))
            {
                angle -= 0.05f;
            }
        }        

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);

            base.Draw(gameTime);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;


            // TODO: Add your drawing code here
            DrawWalls();
            DrawReadings();
            DrawRemoteStats(wiiService.WiiMote1);
            DrawRemoteStats(wiiService.WiiMote2);
            DrawLocationStats();

            
        }

        #endregion

        #region Load Content Methods

        private void SetUpCamera()
        {
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            viewMatrix = Matrix.CreateLookAt(new Vector3(width * 1.5f, width * 1f, -2f * width), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.2f, 4f * width);
        }

        //TODO: update SetUpWallVertices to support 1024 x 768 
        protected void SetUpWallsVerticesEx()
        {
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

            //Generate the "floor" - width x width matrix

            float x = width;
            float z = width;
            float y = height;

            //Floor (y=0)
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


            //Left wall (z=0)

            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0), new Vector3(0, 1, 0), new Vector2(1, 1)));


            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0), new Vector3(0, 1, 0), new Vector2(1, 1)));

            //Right wall (x=0)
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


            wallVerticesBuffer = new VertexBuffer(ScreenManager.Game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            wallVerticesBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }
        #endregion

        #region Draw Methods

        protected virtual void DrawWalls()
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity * Matrix.CreateRotationY(angle));
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(tileTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.SetVertexBuffer(wallVerticesBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, wallVerticesBuffer.VertexCount / 3);
            }
        }

        /// <summary>
        /// DrawThe readings generated by the remotes
        /// Draw projection on the walls and a 3D cude in space
        /// </summary>
        protected virtual void DrawReadings()
        {
            //Create the vertices of the reading
            List<VertexPositionNormalTexture> readingVertices = new List<VertexPositionNormalTexture>();
            bool hasReading1 = false, hasReading2 = false;

            float offset = 0.5f * projectionPointSize;

            //Get reference to the readings for easy access
            var readingRemote1 = wiiService.AvgReadingWiiMote1;
            var readingRemote2 = wiiService.AvgReadingWiiMote2;

            if (readingRemote1.X >= 0 && readingRemote1.X < width && readingRemote1.X >= 0 && readingRemote1.Y < height)
            {
                hasReading1 = true;


                float x = readingRemote1.X - offset;
                float y = readingRemote1.Y - offset;

                //Add the reading 1 marker on the left wall
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -10), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -10), new Vector3(0, 0, 1), new Vector2(1, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(0, 0)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -10), new Vector3(0, 0, 1), new Vector2(1, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(1, 0)));
            }

            if (readingRemote2.X >= 0 && readingRemote2.X < width && readingRemote2.X >= 0 && readingRemote2.Y < height)
            {
                hasReading2 = true;
                float z = width - readingRemote2.X - offset;
                float y = readingRemote2.Y - offset;

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 1)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));

            }

            if (hasReading1 && hasReading2 && Draw3dPoint)
            {
                //Assume that both wiimotes are at same height

                //Create the vertices of the 3D point

                //front (right wall)

                float x = readingRemote1.X - offset;
                float y = readingRemote1.Y - offset;
                float z = width - readingRemote2.X - offset;


                // Set Model Position
                //modelPosition.X = 7f;//readingRemote1.X - offset;
                //modelPosition.Y = 7f; //readingRemote1.Y - offset;
                //modelPosition.Z = 7f;// width - readingRemote2.X - offset;

                // (front)face from left wall
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 1)));

                // (back) face from left wall.
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));


                // (right)face from right wall
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));

                // (left) face from right wall
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));

                //Top
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(1, 1)));

                //Bottom
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z), new Vector3(0, 1, 0), new Vector2(0, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));

                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(1, 1)));

            }

            if (readingVertices.Count == 0)
                return;

            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity * Matrix.CreateRotationY(angle));
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(readingTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.SetVertexBuffer(wallVerticesBuffer);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, readingVertices.ToArray(), 0, readingVertices.Count / 3, VertexPositionNormalTexture.VertexDeclaration);
            }
        }

        /// <summary>
        /// Draw the statistics of the current location found by using 2 WiiMotes
        /// </summary>
        protected virtual void DrawLocationStats()
        {

            if (wiiService.AvailableWiiMotes < 2)
            {
                return;
            }

            try
            {
                spriteBatch.Begin();

                int x = (int)wiiService.AvgReadingWiiMote1.X;
                int y = (int)(wiiService.AvgReadingWiiMote1.Y);
                int z = (int)(width - wiiService.AvgReadingWiiMote2.X);

                spriteBatch.DrawString(promptFont,
                                        String.Format("X:{0,4} , Y:{1,4}, Z:{2,4}", x, y, z),
                                        new Vector2(20, GraphicsDevice.Viewport.Height - 40),
                                        Color.Yellow);
            }
            finally
            {
                spriteBatch.End();
            }
        }

        protected virtual void DrawRemoteStats(Wiimote wm)
        {
            try
            {
                spriteBatch.Begin();

                //Set x value to whether we should draw on the left or right side of the screen, depending on the wiimote.
                float x = wm == wiiService.WiiMote1 ? 0 : GraphicsDevice.Viewport.Width - 200;

                if (wm == null)
                {
                    spriteBatch.DrawString(promptFont, "N/A", new Vector2(x, 0), Color.Yellow);
                    return;
                }


                for (int i = 0; i < wm.WiimoteState.IRState.IRSensors.Length; i++)
                {
                    IRSensor sensor = wm.WiimoteState.IRState.IRSensors[i];

                    string text = String.Format("IR{0}:Not Visible", i + 1);

                    if (sensor.Found)
                    {
                        float px = wm == wiiService.WiiMote1 ? sensor.RawPosition.X : width - sensor.RawPosition.X;
                        float py = sensor.RawPosition.Y;
                        text = String.Format("IR{0}:{{{1,4},{2,4}}}", i + 1, px, py);
                    }

                    spriteBatch.DrawString(promptFont,
                        text,
                        new Vector2(x, i * 16),
                        Color.Yellow);

                }
            }
            finally
            {
                spriteBatch.End();
            }
        }

        #endregion
    }
}
