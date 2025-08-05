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

        public virtual VertexPositionColor[] PerimeterVertices { get; set; }
        public virtual VertexPositionColor[] FilledVertices { get; set; }
        public virtual VertexPositionColor[] TriangleVertices { get; set; }
        public virtual VertexPositionColor[] ThickLineVertices { get; set; }
        private BasicEffect BasicEffect { get; set; } = null!;

        public ShapeManager() { }

        public void Update(GameTime gameTime)
        {
            PerimeterVertices = [];
            FilledVertices = [];
            TriangleVertices = [];
            ThickLineVertices = [];

            foreach (var shape in Shapes)
            {
                shape.Update(gameTime);
                shape.RebuildThickVertices(shape.LineThickness);

                PerimeterVertices = [.. PerimeterVertices, .. shape.GetOutlineAsLineList()];
                FilledVertices = [.. FilledVertices, .. shape.FilledVertices];
                TriangleVertices = [.. TriangleVertices, .. shape.TriangleVertices];
                ThickLineVertices = [.. ThickLineVertices, .. shape.ThickLineVertices];
            }
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
            if (PerimeterVertices is null || PerimeterVertices.Length == 0) return;

            BasicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    PerimeterVertices,
                    0,
                    PerimeterVertices.Length / 2 // Each line is made of 2 vertices
                );
            }
        }

        public virtual void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, viewMatrix);
        }

        public virtual void DrawTriangulatedUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            if (TriangleVertices is null || TriangleVertices.Length == 0) return;

            BasicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    TriangleVertices,
                    0,
                    TriangleVertices.Length / 2
                );
            }
        }
        public virtual void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            if (FilledVertices is null || FilledVertices.Length == 0) return;

            BasicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    FilledVertices,
                    0,
                    FilledVertices.Length / 3
                );
            }
        }

        public virtual void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix, int thickness = 1)
        {
            if (ThickLineVertices is null || ThickLineVertices.Length == 0) return;

            BasicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    ThickLineVertices,
                    0,
                    ThickLineVertices.Length / 3 // Each quad is made of 2 triangles, so we divide by 3
                );
            }
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

        public static BasicEffect InitializeBasicEffect(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            return new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = viewMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };
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
                y = Random.Next((int)startPos.Y, (int)endPos.Y);

                r = Random.Next(0, 256);
                g = Random.Next(0, 256);
                b = Random.Next(0, 256);
                alpha = Random.Next(150, 256);
                speed = Random.Next(60, 151);

                color = new(r, g, b, alpha);
                position = new(x, y);

                int shape = Random.Next(0, 4); // 0 = Star, 1 = Circle, 2 = Rectangle, 3 = Triangle 

                switch (shape)
                {
                    case 0:
                        Shapes.Add(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 1:
                        Shapes.Add(new Circle(position, Random.Next(20, 100), Random.Next(3, 64), color)
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 2:
                        Shapes.Add(new Shapes.Rectangle(position, Random.Next(50, 150), Random.Next(50, 150), color)
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 3:
                        Vector2 p1 = position;
                        Vector2 p2 = position + new Vector2(Random.Next(-50, 50), Random.Next(50, 150));
                        Vector2 p3 = position + new Vector2(Random.Next(50, 150), Random.Next(-50, 50));

                        Shapes.Add(new Triangle(p1, p2, p3, color)
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;
                }


                /*
                Shapes.Add(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });
                */

                /*
                Shapes.Add(new Circle(position, Random.Next(20, 100), Random.Next(3, 64), color)
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

                Shapes.Add(new Shapes.Rectangle(position, Random.Next(50, 150), Random.Next(50, 150), color)
                {
                    Rotate = true,
                    RotationSpeedDegreesPerSecond = speed,
                });

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
