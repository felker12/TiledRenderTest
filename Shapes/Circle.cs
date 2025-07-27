using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Circle : Shape
    {
        public int Radius { get; private set; } = 40;
        public int PointCount { get; private set; } = 32;
        private Vector2[] Points;
        private Line[] Lines { get; set; }

        public Circle() 
        { 
            Initialize(); 
        }

        public Circle(Vector2 center, int radius, int points, Color color)
        {
            Position = center;
            Radius = radius;
            PointCount = points;
            Color = color;
            Initialize();
        }

        private void Initialize()
        {
            const int minPoints = 3, maxPoints = 256;

            PointCount = MathHelper.Clamp(PointCount, minPoints, maxPoints);
            Lines = new Line[PointCount];
            Points = new Vector2[PointCount];

            float rotation = MathHelper.TwoPi / (float)PointCount;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = Radius;
            float ay = 0f;

            float bx, by;
            float xOffset = Position.X + Radius;
            float yOffset = Position.Y + Radius;

            for(int i = 0; i < PointCount; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                Lines[i] = new(ax + xOffset, ay + yOffset, bx + xOffset, by + yOffset, Color);
                Points[i] = new(ax + xOffset, ay + yOffset);

                if (i == PointCount - 1)
                {
                    //Debug.WriteLine($"ax: {ax + xOffset}, ay: {ay + yOffset}, bx: {bx + xOffset}, by: {by + yOffset}");
                    //Debug.WriteLine(Points[i].ToString());
                    //Debug.WriteLine(Lines[i].ToString());
                }

                ax = bx;
                ay = by;
            }

            Points[PointCount - 1] = new(ax + xOffset, ay + yOffset);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void DrawOutline(SpriteBatch spriteBatch)
        {
            foreach (Line line in Lines)
                line.Draw(spriteBatch);
        }

        public override void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            base.DrawOutlineUsingPrimitives(spriteBatch, transformMatrix);
        }

        public override void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            base.DrawOutlineUsingPrimitives(graphicsDevice, transformMatrix);
        }

        public override void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            base.DrawFilledUsingPrimitives(graphicsDevice, viewMatrix);
        }

        public override void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 1)
        {
            base.DrawOutlineThickUsingPrimitives(graphicsDevice, transformMatrix, thickness);
        }

    }
}
