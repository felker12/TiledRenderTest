using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Shape
    {
        public Vector2 Position { get; protected set; } = Vector2.Zero;
        public Color Color { get; protected set; } = Color.White; // Default color
        public Texture2D Texture => Game1.CreateTextureFromColor(Color);
        public virtual Vector2[] Points { get; set; }
        public Line[] Sides => ToLines(Points, Color);
        public virtual VertexPositionColor[] Vertices => [.. Points.Select(corner => ToVertexPositionColor(corner, Color))];
        public virtual VertexPositionColor[] FilledVertices => GenerateFilledVertices();
        public virtual Triangle[] Triangles => Triangulate();

        public virtual Vector2 Center => GetCentroid(Points);

        public Shape(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public Shape(Vector2 position)
        {
            Position = position;
        }

        public Shape(Color color)
        {
            Color = color;
        }

        public Shape()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            // Default update logic, can be overridden by derived classes
            // This method can be used to update the shape's state, position, etc.
            throw new NotImplementedException("Update method must be implemented in derived classes.");
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawOutline(spriteBatch, Color);//TODO
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Draw(spriteBatch, outlineColor, outlineThickness);
            }
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch)
        {
            DrawOutline(spriteBatch, Color);
        }

        public virtual void DrawOutLineWithTriangles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Triangles.Length; i++)
                Triangles[i].DrawOutline(spriteBatch);
        }

        public virtual void DrawOutLineWithTriangles(SpriteBatch spriteBatch, Color color)
        {
            foreach (Triangle t in Triangles)
                t.DrawOutline(spriteBatch, color);
        }


        public virtual void DrawOutlineWithTrianglesUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach (Triangle t in Triangles)
            {
                t.DrawOutlineUsingPrimitives(graphicsDevice, viewMatrix);
            }
        }

        public virtual void DrawOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix, int thickness = 2)
        {
            foreach (Triangle t in Triangles)
            {
                t.DrawOutlineThickUsingPrimitives(graphicsDevice, viewMatrix, thickness);
            }
        }

        public virtual void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
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

        public virtual void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            foreach (var side in Sides)
            {
                side.DrawUsingPrimitives(graphicsDevice, transformMatrix);
            }
        }

        public virtual void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public virtual void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 1)
        {
            foreach (var side in Sides)
                side.DrawThickUsingPrimitives(graphicsDevice, transformMatrix, thickness);
        }

        public static Vector3 ToVector3(Vector2 vector)
        {
            return new(vector, 0);
        }

        public static VertexPositionColor ToVertexPositionColor(Vector2 vector, Color? color)
        {
            return new VertexPositionColor(ToVector3(vector), color ?? Color.White);
        }

        public static Line[] ToLines(Vector2[] vectors, Color color)
        {
            return [.. Enumerable.Range(0, vectors.Length - 1).Select(i => new Line(vectors[i], vectors[i + 1], color))];
        }

        public static Line[] ToLines(List<Vector2> vectors, Color color)
        {
            return ToLines(vectors.ToArray(), color);
        }

        /*
        public Triangle[] Triangulate(Vector2[] vectors, Vector2 center)
        {
            Triangle[] triangles = new Triangle[vectors.Length];

            Vector2[] points = vectors;
            //bool isClosed = vectors[0] == vectors[^1];
            // Remove duplicate last point (the closing point)
            points = [.. points.Take(points.Length - 1)];

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Length];

                // Create triangle from center to edge pair
                triangles[i] = new(center, p1, p2, Color);
            }

            return triangles;
        }
        */

        public Triangle[] Triangulate(Vector2[] vectors, Vector2 center)
        {
            // Use epsilon comparison to determine if shape is closed
            bool isClosed = ApproximatelyEqual(vectors[0], vectors[^1]);
            int uniqueCount = isClosed ? vectors.Length - 1 : vectors.Length;

            Triangle[] triangles = new Triangle[uniqueCount];

            for (int i = 0; i < uniqueCount - 1; i++)
            {
                triangles[i] = new(center, vectors[i], vectors[i + 1], Color);
            }

            // Final triangle
            triangles[uniqueCount - 1] = new(center, vectors[uniqueCount - 1], vectors[0], Color);

            return triangles;
        }

        public Triangle[] Triangulate(Vector2[] vectors)
        {
            bool isClosed = ApproximatelyEqual(vectors[0], vectors[^1]);
            int uniqueCount = isClosed ? vectors.Length - 1 : vectors.Length;

            Vector2 center = GetCentroid(vectors); // Automatically compute center

            Triangle[] triangles = new Triangle[uniqueCount];

            for (int i = 0; i < uniqueCount - 1; i++)
            {
                triangles[i] = new(center, vectors[i], vectors[i + 1], Color);
            }

            triangles[uniqueCount - 1] = new(center, vectors[uniqueCount - 1], vectors[0], Color);

            return triangles;
        }

        public Triangle[] Triangulate()
        {
            Vector2[] vectors = Points;

            bool isClosed = ApproximatelyEqual(vectors[0], vectors[^1]);
            int uniqueCount = isClosed ? vectors.Length - 1 : vectors.Length;

            Vector2 center = Center;

            Triangle[] triangles = new Triangle[uniqueCount];

            for (int i = 0; i < uniqueCount - 1; i++)
            {
                triangles[i] = new(center, vectors[i], vectors[i + 1], Color);
            }

            triangles[uniqueCount - 1] = new(center, vectors[uniqueCount - 1], vectors[0], Color);

            return triangles;
        }

        private static bool ApproximatelyEqual(Vector2 a, Vector2 b, float epsilon = 0.001f)
        {
            return Vector2.DistanceSquared(a, b) < epsilon * epsilon;
        }

        public static Vector2 GetCentroid(Vector2[] points)
        {
            // Use only the unique points, excluding closing duplicate if present
            bool isClosed = ApproximatelyEqual(points[0], points[^1]);
            int count = isClosed ? points.Length - 1 : points.Length;

            Vector2 sum = Vector2.Zero;
            for (int i = 0; i < count; i++)
                sum += points[i];

            return sum / count;
        }

        public virtual VertexPositionColor[] GenerateFilledVertices()
        {
            Vector2 center = GetCentroid(Points);
            var triangles = Triangulate(Points, center);

            int count = triangles.Length;
            var verts = new VertexPositionColor[count * 3];

            for (int i = 0, v = 0; i < count; i++, v += 3)
            {
                var t = triangles[i];
                verts[v] = ToVertexPositionColor(t.Position, Color);
                verts[v + 1] = ToVertexPositionColor(t.Position2, Color);
                verts[v + 2] = ToVertexPositionColor(t.Position3, Color);
            }

            return verts;
        }
    }
}
