using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TiledRenderTest.Shapes
{
    public class Star : Polygon
    {
        public int NumberOfPoints { get; private set; } = 5;
        public float OuterRadius { get; private set; } = 100; 
        public float InnerRadius { get; private set; } = 40;
        public VertexPositionColor[] FilledVertices => CreateFilledStarVertices(Position, OuterRadius, InnerRadius, Color, NumberOfPoints);

        public List<Line> TriangularFillLines { get; set; } = [];
        public List<Vector2> TriangularPoints { get; set; } = [];
        public List<Triangle> Triangles { get; set; } = [];

        public Star(Vector2 center, int numbOfPoints = 5, int outerRadius = 100, int innerRadius = 40) : base(center)
        {
            NumberOfPoints = numbOfPoints; 
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;

            Points = CreateStarOutline(center, OuterRadius, InnerRadius, NumberOfPoints);
        }

        public Star(Vector2 center, Color color, int numbOfPoints = 5, int outerRadius = 100, int innerRadius = 40) : 
            this(center, numbOfPoints, outerRadius, innerRadius)
        {
            Color = color;
        }

        public void SetData(int outerRadius = 100, int innerRadius = 40, int numOfPoints = 5)
        {
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            NumberOfPoints = numOfPoints;

            Points = CreateStarOutline(Position, OuterRadius, InnerRadius, NumberOfPoints);
            Triangulate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            base.Draw(spriteBatch, outlineThickness);
        }

        public override void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            BasicEffect effect = new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = viewMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
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

        public override void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            base.DrawOutline(spriteBatch, outlineThickness);
        }

        public override void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            base.DrawOutlineUsingPrimitives(graphicsDevice, transformMatrix);
        }

        public override void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 2)
        {
            foreach (var side in Sides)
                side.DrawThickUsingPrimitives(graphicsDevice, transformMatrix, thickness);
        }

        public static Vector2[] CreateStarOutline(Vector2 center, float outerRadius, float innerRadius, int numPoints = 5)
        {
            var points = new Vector2[numPoints * 2 + 1];
            float angleStep = MathF.PI / numPoints; // 180° / numPoints, half angle step for outer/inner
            float currentAngle = -MathF.PI / 2f;    // start at top (-90 degrees)

            for (int i = 0; i < points.Length; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new Vector2(
                    center.X + MathF.Cos(currentAngle) * radius,
                    center.Y + MathF.Sin(currentAngle) * radius
                );
                currentAngle += angleStep;
            }

            // Close the shape
            points[^1] = points[0]; //the last point should be the same as the first

            return points;
        }

        public static VertexPositionColor[] CreateFilledStarVertices(Vector2 center, float outerRadius, float innerRadius, Color color, int numPoints = 5)
        {
            List<VertexPositionColor> vertices = [];

            float angleStep = MathF.PI / numPoints;
            float currentAngle = -MathF.PI / 2f; // Start at top

            Vector2[] points = new Vector2[numPoints * 2];
            for (int i = 0; i < points.Length; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new Vector2(
                    center.X + MathF.Cos(currentAngle) * radius,
                    center.Y + MathF.Sin(currentAngle) * radius
                );
                currentAngle += angleStep;
            }

            // Build triangles from center to each edge pair (triangle fan style)
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Length];

                vertices.Add(new VertexPositionColor(new Vector3(center, 0), color));
                vertices.Add(new VertexPositionColor(new Vector3(p1, 0), color));
                vertices.Add(new VertexPositionColor(new Vector3(p2, 0), color));
            }

            return [.. vertices];
        }




        public void DrawStarOutlineWithTriangles(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach(Triangle t in Triangles)
            {
                t.DrawOutlineUsingPrimitives(graphicsDevice, viewMatrix);
            }
        }

        public void DrawStarOutlineThickWithTriangles(GraphicsDevice graphicsDevice, Matrix viewMatrix, int thickness = 2)
        {
            foreach (Triangle t in Triangles)
            {
                t.DrawOutlineThickUsingPrimitives(graphicsDevice, viewMatrix, thickness);
            }
        }

        public void Triangulate()
        {
            Triangles.Clear();
            TriangularPoints.Clear();
            TriangularFillLines.Clear();

            Vector2[] points = CreateStarOutline(Position, OuterRadius, InnerRadius, NumberOfPoints);

            // Remove duplicate last point (the closing point)
            points = [.. points.Take(points.Length - 1)];

            // Star has 2N points, alternating outer/inner
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 center = Position;
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Length];

                // Create triangle from center to edge pair
                Triangle triangle = new(center, p1, p2, Color);
                Triangles.Add(triangle);
            }

            // Optionally build TriangularFillLines and TriangularPoints
            foreach (var tri in Triangles)
            {
                TriangularFillLines.AddRange(tri.Sides);
                TriangularPoints.AddRange(tri.Sides.Select(s => s.Position));
                TriangularPoints.AddRange(tri.Sides.Select(s => s.Position2));
            }
        }
    }
}
