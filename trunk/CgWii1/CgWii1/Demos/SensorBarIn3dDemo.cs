using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CgWii1.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CgWii1.Demos
{
    public class SensorBarIn3dDemo : PointIn3dDemo
    {
        #region Fields
        //GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont promptFont;

        //Drawing effects HSLS
        private Effect effect;

        //View/projection
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        //Wiimote camera resolution.
        private const int width = 1024;
        private const int height = 768;

        private Texture2D tileTexture;
        private Texture2D readingTexture;

        //Model and point of the calculated 3D point

        Model sensorBarModel;
        Vector3 modelPosition = new Vector3(0f, 0.0f, -0.0f);
        float curModelRoll = 0.0f;
        float curModelYaw = -45.0f;
        Quaternion modelRotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-45.0f), 0, 0.0f);
        private Vector3 cameraPosition = new Vector3(5, 5, -5);

        private VertexBuffer wallVerticesBuffer;
        #endregion

        #region Overrides
        public override void LoadContent()
        {
            base.LoadContent();

            ScreenManager.Game.Window.Title = "Sensor Bar in 3D Spaces";

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("effects");

            tileTexture = Content.Load<Texture2D>("WallTile");
            readingTexture = Content.Load<Texture2D>("reading");
            promptFont = Content.Load<SpriteFont>("StatsFont");
            sensorBarModel = Content.Load<Model>("sensorbar"); //LoadModel("sensorbar");
            SetUpCamera();

            SetUpWallsVerticesEx();
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

            // TODO: Add your update logic here
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.PageUp))
            {
                curModelYaw += 1f;
                curModelRoll = 0.0f;

            }
            else if (keyState.IsKeyDown(Keys.PageDown))
            {
                curModelYaw += -1f;
                curModelRoll = 0.0f;
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                curModelYaw += 0.0f;
                curModelRoll = 1f;
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                curModelYaw += 0.0f;
                curModelRoll = -1f;
            }
            else
            {
                curModelYaw += 0f;
                curModelRoll = 0.0f;
            }

            modelRotation *= Quaternion.CreateFromYawPitchRoll(0, 0f, MathHelper.ToRadians(curModelRoll));


            //Quaternion.Normalize(ref modelRotation, out modelRotation);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;


            // TODO: Add your drawing code here
            DrawWalls();
            DrawReadings();
            DrawRemoteStats(wiiService.WiiMote1);
            DrawRemoteStats(wiiService.WiiMote2);
            DrawLocationStats();
            DrawModel();

            base.Draw(gameTime);
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

                        //float x = modelPosition.X;
                        //float y = modelPosition.Y;
                        //float z = modelPosition.Z;

                        //float newX = (float)(x * Math.Cos(angle) + z * Math.Sin(angle));
                        //float newZ =(float)(-x * Math.Sin(angle) + z * Math.Cos(angle));

                        localEffect.World = transforms[mesh.ParentBone.Index] *
                                            Matrix.CreateScale(0.15f) *
                                            Matrix.CreateFromQuaternion(modelRotation) * //Roll around object self midpoint.
                                            Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(curModelYaw), 0, 0) * //Yaw around world's Y axis
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



        #endregion
        #endregion

        #region Load Content Methods

        private void SetUpCamera()
        {
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            viewMatrix = Matrix.CreateLookAt(new Vector3(width * 1.5f, width * 1f, -2f * width), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.2f, 4f * width);
        }
        
        private void SetUpWallsVerticesEx()
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





            wallVerticesBuffer = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            wallVerticesBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }

        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
        #endregion
    }
}
