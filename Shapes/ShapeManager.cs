using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TiledRenderTest.Shapes
{
    public class ShapeManager 
    {
        private readonly List<VertexPositionColor> _perimeterTemp = [];
        private readonly List<VertexPositionColor> _filledTemp = [];
        private readonly List<VertexPositionColor> _triangleTemp = [];
        private readonly List<VertexPositionColor> _thickTemp = [];

        public List<Shape> Shapes { get; private set; } = [];
        private Random Random { get; set; } = new();

        public virtual VertexPositionColor[] PerimeterVertices { get; private set; }
        public virtual VertexPositionColor[] FilledVertices { get; private set; }
        public virtual VertexPositionColor[] TriangleVertices { get; private set; }
        public virtual VertexPositionColor[] ThickLineVertices { get; private set; }
        private BasicEffect BasicEffect { get; set; } = null!;

        public int LineThickness = 2;

        public ShapeManager() { }

        public void Update(GameTime gameTime)
        {
            _perimeterTemp.Clear();
            _filledTemp.Clear();
            _triangleTemp.Clear();
            _thickTemp.Clear();


            foreach (var shape in Shapes)
            {
                shape.Update(gameTime);
                shape.RebuildThickVertices(LineThickness);


                ShapeMover.MoveShapeWithinBounds(shape, new(0, 0), new(1000, 1000));

                _perimeterTemp.AddRange(shape.GetOutlineAsLineList());
                _filledTemp.AddRange(shape.FilledVertices);
                _triangleTemp.AddRange(shape.TriangleVertices);
                _thickTemp.AddRange(shape.ThickLineVertices);
            }

            PerimeterVertices = [.. _perimeterTemp];
            FilledVertices = [.. _filledTemp];
            TriangleVertices = [.. _triangleTemp];
            ThickLineVertices = [.. _thickTemp];
        }

        //SpriteBatch Draw calls 
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

        //GraphicsDevice Draw calls
        public virtual void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            if (PerimeterVertices is null || PerimeterVertices.Length == 0) return;

            EnsureBasicEffect(graphicsDevice, viewMatrix);

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

        public virtual void DrawTriangulatedUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            if (TriangleVertices is null || TriangleVertices.Length == 0) return;

            EnsureBasicEffect(graphicsDevice, viewMatrix);

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

            EnsureBasicEffect(graphicsDevice, viewMatrix);

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
            LineThickness = thickness;

            if (ThickLineVertices is null || ThickLineVertices.Length == 0) return;

            EnsureBasicEffect(graphicsDevice, viewMatrix); ;

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

        //Utility Methods
        public void AddShape(Shape shape)
        {
            ArgumentNullException.ThrowIfNull(shape);
            Shapes.Add(shape);

            int motionX, motionY;

            int random = Random.Next(0, 2);
            if (random == 0)
                motionX = -1;
            else 
                motionX = 1;

            random = Random.Next(0, 2);
            if (random == 0) 
                motionY = -1;
            else 
                motionY = 1;

            shape.Motion = new(motionX, motionY);
            shape.Motion.Normalize();
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

        #nullable enable        
        public Shape? GetShapeAtPoint(Vector2 point)
        {
            return Shapes.FirstOrDefault(shape => shape.Contains(point));
        }
        #nullable disable

        public IEnumerable<Shape> GetIntersectingShapes(Shape other)
        {
            return Shapes.Where(s => s.Intersects(other));
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

        public void EnsureBasicEffect(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            BasicEffect ??= new BasicEffect(graphicsDevice)
                {
                    VertexColorEnabled = true,
                    World = Matrix.Identity,
                    View = viewMatrix,
                    Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
                };

            BasicEffect.View = viewMatrix;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height, 0,
                0, 1);
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
                        AddShape(new Star(position, color, Random.Next(3, 10), Random.Next(70, 150), Random.Next(40, 70))
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 1:
                        AddShape(new Circle(position, Random.Next(20, 100), color, Random.Next(3, 64))
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 2:
                        AddShape(new Shapes.Rectangle(position, Random.Next(50, 150), Random.Next(50, 150), color)
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;

                    case 3:
                        Vector2 p1 = position;
                        Vector2 p2 = position + new Vector2(Random.Next(-50, 50), Random.Next(50, 150));
                        Vector2 p3 = position + new Vector2(Random.Next(50, 150), Random.Next(-50, 50));

                        AddShape(new Triangle(p1, p2, p3, color)
                        {
                            Rotate = true,
                            RotationSpeedDegreesPerSecond = speed,
                        });
                        break;
                }
            }
        }
    }
}
