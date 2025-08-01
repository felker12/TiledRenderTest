using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class ShapeManager 
    {
        public List<Shape> Shapes { get; private set; } = [];
        private Random Random { get; set; } = new();

        public ShapeManager() { }

        public void Update(GameTime gameTime)
        {
            foreach (var shape in Shapes)
                shape.Update(gameTime);
        }

        //SpriteBatch Draw calls    
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var shape in Shapes)
                shape.Draw(spriteBatch);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var shape in Shapes)
                shape.DrawOutline(spriteBatch, outlineColor, outlineThickness);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var shape in Shapes)
                shape.DrawOutline(spriteBatch, outlineThickness);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch)
        {
            foreach (var shape in Shapes)
                shape.DrawOutline(spriteBatch);
        }

        public virtual void DrawFilled(SpriteBatch spriteBatch)
        {
            foreach (var shape in Shapes)
                shape.DrawFilled(spriteBatch);
        }

        public virtual void DrawTriangulated(SpriteBatch spriteBatch)
        {
            foreach (var shape in Shapes)
                shape.DrawTriangulated(spriteBatch);
        }

        public virtual void DrawTriangulated(SpriteBatch spriteBatch, Color color)
        {
            foreach (var shape in Shapes)
                shape.DrawTriangulated(spriteBatch, color);
        }

        //GraphicsDevice Draw calls
        public virtual void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach (var shape in Shapes)
                shape.DrawOutlineUsingPrimitives(graphicsDevice, viewMatrix);
        }

        public virtual void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, viewMatrix);
        }

        public virtual void DrawTriangulatedUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach (var shape in Shapes)
                shape.DrawTriangulatedUsingPrimitives(graphicsDevice, viewMatrix);
        }

        public virtual void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
           foreach (var shape in Shapes)
                shape.DrawFilledUsingPrimitives(graphicsDevice, viewMatrix);
        }

        public virtual void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix, int thickness = 1)
        {
            foreach (var shape in Shapes)
                shape.DrawOutlineThickUsingPrimitives(graphicsDevice, viewMatrix, thickness);
        }

        public void AddShape(Shape shape)
        {
            ArgumentNullException.ThrowIfNull(shape);

            Shapes.Add(shape);
        }

        public void RemoveShape(Shape shape)
        {
            ArgumentNullException.ThrowIfNull(shape);
            Shapes.Remove(shape);
        }

        public void ClearShapes()
        {
            Shapes.Clear();
        }

        public void AddRandomShapes(int amount, Vector2 startPos, Vector2 endPos)
        {
            Random = new Random();
            ClearShapes();

            float x, y;
            int r, g, b, alpha;
            Vector2 position;
            Color color;
            int speed;

            for (int i = 0; i < amount; i++)
            {
                x = Random.Next((int)startPos.X, (int)endPos.X);
                y = Random.Next((int)startPos.X, (int)endPos.X);

                r = Random.Next(0, 256);
                g = Random.Next(0, 256);
                b = Random.Next(0, 256);
                alpha = Random.Next(150, 256);
                speed = Random.Next(60, 151);

                color = new(r, g, b, alpha);
                position = new(x, y);
                /*
                if (Random.Next(0, 2) == 0)
                {
                    Shapes.Add(new Circle(new(Random.Next(-500, 500), Random.Next(-500, 500)), Random.Next(20, 100), Random.Next(3, 64), Color.Aquamarine));
                }
                else
                {
                    Shapes.Add(new Star(new(Random.Next(-500, 500), Random.Next(-500, 500)), Color.Aquamarine, Random.Next(3, 10), Random.Next(50, 100), Random.Next(10, 50)));
                }
                */

                Shapes.Add(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

                /*
                Shapes.Add(new
                    Triangle(position,
                    position + new Vector2(0, 100), 
                    position + new Vector2(50, 0))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });
                */
            }




        }



    }
}
