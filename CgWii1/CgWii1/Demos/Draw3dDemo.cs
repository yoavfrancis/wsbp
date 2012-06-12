using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CgWii1.Demos
{
    public class Draw3dDemo : PointIn3dDemo
    {
        #region Fields
        class DrawingPoint
        {
            internal Vector3 Location { get; set; }
            internal double CreationTime { get; set; }
            internal double Radius { get; set; }
        }

        const int MAX_POINTS = 1000;         //Maximum number of points to draw
        const int POINT_RADIUS = 20;        //Size of the point
        const int POINT_KEEP_ALIVE = 1000;  //Time a point stays in the screen

        Model pointModel;

        List<DrawingPoint> pointsList = new List<DrawingPoint>();

        Vector3 lightDirection = new Vector3(3, -2, 5);

        #endregion

        public Draw3dDemo()
        {
            Draw3dPoint = false;
        }


        #region Override Game Mechanices
        public override void LoadContent()
        {
            base.LoadContent();

            pointModel = LoadModel("target");

            ScreenManager.Game.Window.Title = "Draw Points in 3D Space";

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                ExitScreen();

            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            //Remove all the points that expired; The predicate just find those points 
            pointsList.RemoveAll(p => currentTime - p.CreationTime > POINT_KEEP_ALIVE);

            //Make sure there are no more points than allowed
            if (pointsList.Count > MAX_POINTS)
                return;

            //Get current location
            pointsList.Add(new DrawingPoint()
            {
                Location = new Vector3(wiiService.AvgReadingWiiMote1.X - POINT_RADIUS, wiiService.AvgReadingWiiMote1.Y - POINT_RADIUS, width - wiiService.AvgReadingWiiMote2.X - POINT_RADIUS),
                CreationTime = gameTime.TotalGameTime.TotalMilliseconds,
                Radius = POINT_RADIUS
            });
          
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawPoints();
        }
        
        #endregion

        #region Draw Methods
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
                int z = (int)(width - wiiService.AvgReadingWiiMote2.X);

                spriteBatch.DrawString(promptFont,
                                        String.Format("X:{0,4} , Y:{1,4}, Z:{2,4}, Points: {3,3}", x, y, z, pointsList.Count),
                                        new Vector2(20, GraphicsDevice.Viewport.Height - 40),
                                        Color.Yellow);
            }
            finally
            {
                spriteBatch.End();
            }
        }


        private void DrawPoints()
        {
            for (int i = 0; i < pointsList.Count; i++)
            {
                Matrix worldMatrix = Matrix.CreateScale((float)pointsList[i].Radius) * Matrix.CreateTranslation(pointsList[i].Location);

                Matrix[] targetTransforms = new Matrix[pointModel.Bones.Count];
                pointModel.CopyAbsoluteBoneTransformsTo(targetTransforms);
                foreach (ModelMesh modmesh in pointModel.Meshes)
                {
                    foreach (Effect currentEffect in modmesh.Effects)
                    {
                        currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                        currentEffect.Parameters["xWorld"].SetValue(targetTransforms[modmesh.ParentBone.Index] * worldMatrix);
                        currentEffect.Parameters["xView"].SetValue(viewMatrix);
                        currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                        currentEffect.Parameters["xEnableLighting"].SetValue(true);
                        currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                        currentEffect.Parameters["xAmbient"].SetValue(0.5f);
                    }
                    modmesh.Draw();
                }
            }
        } 
        #endregion

        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
    }


}
