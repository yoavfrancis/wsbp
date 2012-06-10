//using System;
//using System.Collections.Generic;
//using System.Linq;


//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//using WiimoteLib;

//namespace CgWii1
//{
//    /// <summary>
//    /// This is the main type for your game
//    /// </summary>
//    public class Game1 : Microsoft.Xna.Framework.Game
//    {
//        #region Fields
//        GraphicsDeviceManager graphics;
//        SpriteBatch spriteBatch;
//        SpriteFont promptFont;

//        //Drawing effects HSLS
//        private Effect effect;

//        //View/projection
//        private Matrix viewMatrix;
//        private Matrix projectionMatrix;

//        //TODO: update SetUpWallVertices to support 1024 x 768 then change values here
//        private const int width = 1024;
//        private const int height = 768;

//        private Texture2D tileTexture;
//        private Texture2D readingTexture;

//        //Model and point of the calculated 3D point


//        //The reading from the 2 remotes
//        private Vector2 readingRemote1 = new Vector2(5, 5);
//        private Vector2 readingRemote2 = new Vector2(5, 5);
//        private float projectionPointSize = 40;

//        private Vector3 cameraPosition = new Vector3(5, 5, -5);

//        private float angle;

//        private VertexBuffer wallVerticesBuffer;

//        #region WiiMote
//        Wiimote remote1;
//        Wiimote remote2;
      
//        #endregion

//        #endregion

//        public Game1()
//        {
//            graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";

//            graphics.PreferredBackBufferWidth = 1440;
//            graphics.PreferredBackBufferHeight = 900;
//            graphics.ApplyChanges();
//            Window.Title = "3D Point with 2 perpendicular wiimotes";

//        }

//        /// <summary>
//        /// Allows the game to perform any initialization it needs to before starting to run.
//        /// This is where it can query for any required services and load any non-graphic
//        /// related content.  Calling base.Initialize will enumerate through any components
//        /// and initialize them as well.
//        /// </summary>
//        protected override void Initialize()
//        {
//            // TODO: Add your initialization logic here

//            WiimoteCollection wc = new WiimoteCollection();
//            int index = 1;
//            try
//            {
//                wc.FindAllWiimotes();
//            }
//            catch(WiimoteNotFoundException ex)
//            {
//                throw new Exception("Wiimote not found\n" + ex.Message);
//            }
//            catch(WiimoteException ex)
//            {
//                throw new Exception("Wiimote error : \n" + ex.Message);
//            }
//            catch(Exception ex)
//            {
//                throw new Exception("Unknown error:\n" + ex.Message);
//            }

//            foreach (Wiimote wm in wc)
//            {
//                wm.WiimoteChanged += wm_WiimoteChanged;
//                //wm.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;

//                wm.Connect();

//                // Use Button, accelerometer and IR data
//                wm.SetReportType(InputReport.IRAccel, true);
//                wm.SetLEDs(index++);

//                if (remote1 == null)
//                {
//                    remote1 = wm;
//                }
//                else if (remote2 == null)
//                {
//                    remote2 = wm;
//                    break;      //Quit while loop - we already have 2 remotes
//                }
//            }
            

//            if (remote1 == null || remote2 == null)
//            {
//                //throw new Exception("Could not initialize 2 Wiimotes");
//            }


//            base.Initialize();
//        }

//        /// <summary>
//        /// LoadContent will be called once per game and is the place to load
//        /// all of your content.
//        /// </summary>
//        protected override void LoadContent()
//        {
//            // Create a new SpriteBatch, which can be used to draw textures.
//            spriteBatch = new SpriteBatch(GraphicsDevice);

//            effect = Content.Load<Effect>("effects");

//            tileTexture = Content.Load<Texture2D>("WallTile");
//            readingTexture = Content.Load<Texture2D>("reading");
//            promptFont = Content.Load<SpriteFont>("StatsFont");
//            SetUpCamera();

//            SetUpWallsVerticesEx();

//            //point3DModel = LoadModel("target");
//        }

//        #region Load Content Methods

//        private void SetUpCamera()
//        {
//            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
//            viewMatrix = Matrix.CreateLookAt(new Vector3(width * 1.5f, width * 1f, -2f *width), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
//            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.2f, 4f*width);
//        }

//        //TODO: update SetUpWallVertices to support 1024 x 768 
//        private void SetUpWallsVerticesEx()
//        {
//            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();

//            //Generate the "floor" - width x width matrix

//            float x = width;
//            float z = width;
//            float y = height;



//            //Floor (y=0)
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


//            //Left wall (z=0)

//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0), new Vector3(0, 1, 0), new Vector2(1, 1)));


//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, y, 0), new Vector3(0, 1, 0), new Vector2(1, 1)));
            
//            //Right wall (x=0)
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));


//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector2(0, 0)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, 0), new Vector3(0, 1, 0), new Vector2(0, 1)));
//            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, y, -z), new Vector3(0, 1, 0), new Vector2(1, 1)));
            




//            wallVerticesBuffer = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

//            wallVerticesBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
//        }


//        private Model LoadModel(string assetName)
//        {
//            Model newModel = Content.Load<Model>(assetName);

//            foreach (ModelMesh mesh in newModel.Meshes)
//                foreach (ModelMeshPart meshPart in mesh.MeshParts)
//                    meshPart.Effect = effect.Clone();

//            return newModel;
//        }
//        #endregion

//        /// <summary>
//        /// UnloadContent will be called once per game and is the place to unload
//        /// all content.
//        /// </summary>
//        protected override void UnloadContent()
//        {
//            // TODO: Unload any non ContentManager content here
//        }

//        /// <summary>
//        /// Allows the game to run logic such as updating the world,
//        /// checking for collisions, gathering input, and playing audio.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Update(GameTime gameTime)
//        {
//            // Allows the game to exit
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
//                this.Exit();

//            // TODO: Add your update logic here
//            KeyboardState keyState = Keyboard.GetState();
//            if (keyState.IsKeyDown(Keys.PageUp))
//            {
//                angle += 0.05f;
//            }
//            if (keyState.IsKeyDown(Keys.PageDown))
//            {
//                angle -= 0.05f;
//            }

//            base.Update(gameTime);
//        }

//        #region Move marker vertices

//        #region Wiimote Handlers

//        void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
//        {
//            //var wm = sender as Wiimote;

//            //if (wm == null)
//            //    return;

//            //if (args.Inserted)
//            //    wm.SetReportType(Wiimote.InputReport.IRExtensionAccel, true);    // return extension data
//            //else
//            //    wm.SetReportType(Wiimote.InputReport.IRAccel, true);            // back to original mode
//        }
//        #endregion

//        void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
//        {
//            // current state information
//            WiimoteState ws = args.WiimoteState;

//            //cast sender to a Wiimote
//            var remote = sender as Wiimote;
//            //Check if this is a reading from remote1 or 2 and select point to update in accordance
//            var readingToChange = remote == remote1 ? readingRemote1 : readingRemote2;

//            //Get first valid reading (have both x and y)
//            //TODO: maybe take average or something...
//            //TODO: maybe checking found is enough?
//            //TODO: maybe use mid point (of IR state)
//            var validIr = ws.IRState.IRSensors.Where(ir => ir.Found);
           

//            //Check that we got some valid reading and update point if we did
//            if (validIr.Count() > 0)
//            {
//                if (remote == remote1)
//                {
//                    readingRemote1.X = validIr.Average(ir => (float)ir.RawPosition.X);
//                    readingRemote1.Y = validIr.First().RawPosition.Y;
//                }
//                else
//                {
//                    readingRemote2.X = validIr.Average(ir => (float)ir.RawPosition.X);
//                    readingRemote2.Y = validIr.First().RawPosition.Y;
//                }
//            }
                
//        }
//        #endregion

//        /// <summary>
//        /// This is called when the game should draw itself.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

//            RasterizerState rs = new RasterizerState();
//            rs.CullMode = CullMode.None;
//            GraphicsDevice.RasterizerState = rs;

            
//            // TODO: Add your drawing code here
//            DrawWalls();
//            DrawReadings();
//            DrawRemoteStats(remote1);
//            DrawRemoteStats(remote2);
//            DrawLocationStats();

//            base.Draw(gameTime);
//        }

//        #region Draw Methods

//        private void DrawWalls()
//        {
//            effect.CurrentTechnique = effect.Techniques["Textured"];
//            effect.Parameters["xWorld"].SetValue(Matrix.Identity * Matrix.CreateRotationY(angle));
//            effect.Parameters["xView"].SetValue(viewMatrix);
//            effect.Parameters["xProjection"].SetValue(projectionMatrix);
//            effect.Parameters["xTexture"].SetValue(tileTexture);

//            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//            {
//                pass.Apply();
//                GraphicsDevice.SetVertexBuffer(wallVerticesBuffer);
//                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, wallVerticesBuffer.VertexCount / 3);
//            }
//        }

//        private void DrawReadings()
//        {
//            //Create the vertices of the reading
//            List<VertexPositionNormalTexture> readingVertices = new List<VertexPositionNormalTexture>();
//            bool hasReading1 = false, hasReading2 = false;

//            float offset = 0.5f * projectionPointSize;

//            if (readingRemote1.X >= 0 && readingRemote1.X < width && readingRemote1.X >= 0 && readingRemote1.Y < height)
//            {
//                hasReading1 = true;

                
//                float x = readingRemote1.X - offset;
//                float y = readingRemote1.Y - offset;

//                //Add the reading 1 marker on the left wall
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -10), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -10), new Vector3(0, 0, 1), new Vector2(1, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(0, 0)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -10), new Vector3(0, 0, 1), new Vector2(1, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -10), new Vector3(0, 0, 1), new Vector2(1, 0)));
//            }

//            if (readingRemote2.X >= 0 && readingRemote2.X < width && readingRemote2.X >= 0 && readingRemote2.Y < height)
//            {
//                hasReading2 = true;
//                float z = width - readingRemote2.X - offset;
//                float y = readingRemote2.Y - offset;

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y+projectionPointSize, -z ), new Vector3(0, 0, 1), new Vector2(1, 1)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(10, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));
                
//            }

//            if (hasReading1 && hasReading2)
//            {
//                //Assume that both wiimotes are at same height

//                //Create the vertices of the 3D point

//                //front (right wall)

//                float x = readingRemote1.X - offset;
//                float y = readingRemote1.Y - offset;
//                float z = width - readingRemote2.X - offset;


//                // Set Model Position
//                //modelPosition.X = 7f;//readingRemote1.X - offset;
//                //modelPosition.Y = 7f; //readingRemote1.Y - offset;
//                //modelPosition.Z = 7f;// width - readingRemote2.X - offset;

//                // (front)face from left wall
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 1)));

//                // (back) face from left wall.
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));


//                // (right)face from right wall
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));
                
//                // (left) face from right wall
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 0, 1), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 0, 1), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 0, 1), new Vector2(1, 1)));

//                //Top
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y + projectionPointSize, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(1, 1)));

//                //Bottom
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y , -z), new Vector3(0, 1, 0), new Vector2(0, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize,  y, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));

//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z), new Vector3(0, 1, 0), new Vector2(1, 0)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(0, 1)));
//                readingVertices.Add(new VertexPositionNormalTexture(new Vector3(x + projectionPointSize, y, -z - projectionPointSize), new Vector3(0, 1, 0), new Vector2(1, 1)));
                
//            }

//            if (readingVertices.Count == 0)
//                return;

//            effect.CurrentTechnique = effect.Techniques["Textured"];
//            effect.Parameters["xWorld"].SetValue(Matrix.Identity * Matrix.CreateRotationY(angle));
//            effect.Parameters["xView"].SetValue(viewMatrix);
//            effect.Parameters["xProjection"].SetValue(projectionMatrix);
//            effect.Parameters["xTexture"].SetValue(readingTexture);

//            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//            {
//                pass.Apply();
//                GraphicsDevice.SetVertexBuffer(wallVerticesBuffer);
//                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, readingVertices.ToArray(), 0, readingVertices.Count / 3, VertexPositionNormalTexture.VertexDeclaration);
//            }
//        }


//        private void DrawLocationStats()
//        {
//            if(remote1 == null || remote2 == null) {
//                return;
//            }

//            try
//            {
//                spriteBatch.Begin();

//                int x = (int)readingRemote1.X ;
//                int y = (int)(readingRemote1.Y);
//                int z = (int)(width - readingRemote2.X);

//                spriteBatch.DrawString(promptFont,
//                                        String.Format("X:{0,4} , Y:{1,4}, Z:{2,4}", x, y, z),
//                                        new Vector2(20, graphics.GraphicsDevice.Viewport.Height- 40),
//                                        Color.Yellow);
//            }
//            finally
//            {
//                spriteBatch.End();
//            }
//        }

//        private void DrawRemoteStats(Wiimote wm)
//        {
//            try
//            {
//                spriteBatch.Begin();
                
//                //Set x value to whether we should draw on the left or right side of the screen, depending on the wiimote.
//                float x = wm == remote1 ? 0 : graphics.GraphicsDevice.Viewport.Width - 200;

//                if (wm == null)
//                {
//                    spriteBatch.DrawString(promptFont, "N/A", new Vector2(x, 0), Color.Yellow);
//                    return;
//                }


//                for (int i = 0; i < wm.WiimoteState.IRState.IRSensors.Length; i++)
//                {
//                    IRSensor sensor = wm.WiimoteState.IRState.IRSensors[i];

//                    string text = String.Format("IR{0}:Not Visible", i + 1);

//                    if (sensor.Found)
//                    {
//                        float px = wm == remote1 ? sensor.RawPosition.X : width - sensor.RawPosition.X;
//                        float py = sensor.RawPosition.Y;
//                        text = String.Format("IR{0}:{{{1,4},{2,4}}}", i + 1, px, py);
//                    }

//                    spriteBatch.DrawString(promptFont,
//                        text,
//                        new Vector2(x, i * 16),
//                        Color.Yellow);

//                }
//            }
//            finally
//            {
//                spriteBatch.End();
//            }
//        }

//        #endregion
//    }
//}
