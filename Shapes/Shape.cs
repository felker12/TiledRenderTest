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
        protected VertexPositionColor[] filledVertices;
        protected Triangle[] triangles;
        protected Line[] sides;
        protected Vector2 center;
        protected bool isDirty = true;
        protected BasicEffect basicEffect;
        protected Vector2[] points;
        protected VertexPositionColor[] edgeVertices;
        protected Texture2D texture;

        public Vector2 Position { get; protected set; } = Vector2.Zero;
        public Color Color { get; protected set; } = Color.White; // Default color
        public Texture2D Texture { get { texture ??= Game1.CreateTextureFromColor(Color); return texture; } }
        public virtual Vector2 Center { get { RebuildIfDirty(); return center; } }
        public virtual Vector2[] Points { get => points; protected set { points = value; MarkDirty(); } }
        public virtual Line[] Sides { get { RebuildIfDirty(); return sides; } }
        public virtual VertexPositionColor[] Vertices { get { RebuildIfDirty(); return edgeVertices; } }
        public virtual VertexPositionColor[] FilledVertices { get { RebuildIfDirty(); return filledVertices; } }
        public virtual Triangle[] Triangles { get { RebuildIfDirty(); return triangles; } }

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

        public void MarkDirty()
        {
            isDirty = true;
        }

        protected virtual void RebuildIfDirty()
        {
            if (!isDirty || points == null || points.Length == 0) return;

            center = GetCentroid(points);
            triangles = Triangulate(points, center);
            filledVertices = GenerateFilledVertices(triangles);
            sides = ToLines(points, Color);
            edgeVertices = [.. points.Select(p => ToVertexPositionColor(p, Color))];

            isDirty = false;
        }

        protected void ClearCache()
        {
            edgeVertices = null;
            filledVertices = null;
            triangles = null;
            sides = null;

            MarkDirty();
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
                side.Draw(spriteBatch, outlineThickness);
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

        public virtual void DrawOutLineThickWithTriangles(SpriteBatch spriteBatch, Color color, int outlineThickness = 2)
        {
            foreach (Triangle t in Triangles)
                t.DrawOutline(spriteBatch, color, outlineThickness);
        }

        public virtual void DrawOutLineThickWithTriangles(SpriteBatch spriteBatch, int outlineThickness = 2)
        {
            foreach (Triangle t in Triangles)
                t.DrawOutline(spriteBatch, Color, outlineThickness);
        }

        public virtual void DrawOutlineWithTrianglesUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach (Triangle t in Triangles)
            {
                t.DrawOutlineUsingPrimitives(graphicsDevice, viewMatrix);
            }
        }

        public virtual void DrawOutlineThickWithTrianglesUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            foreach (Triangle t in Triangles)
            {
                t.DrawOutlineThickUsingPrimitives(graphicsDevice, viewMatrix);
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
            basicEffect ??= InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
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
                side.DrawUsingPrimitives(graphicsDevice, transformMatrix, basicEffect);
            }
        }

        public virtual void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public virtual void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 1)
        {
            foreach (var side in Sides)
                side.DrawThickUsingPrimitives(graphicsDevice, transformMatrix, basicEffect, thickness);
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

        public virtual VertexPositionColor[] GenerateFilledVertices(Triangle[] triangles)
        {
            if (triangles == null || triangles.Length == 0) return [];

            var verts = new VertexPositionColor[triangles.Length * 3];

            for (int i = 0, v = 0; i < triangles.Length; i++, v += 3)
            {
                var t = triangles[i];
                verts[v] = ToVertexPositionColor(t.Position, Color);
                verts[v + 1] = ToVertexPositionColor(t.Position2, Color);
                verts[v + 2] = ToVertexPositionColor(t.Position3, Color);
            }

            return verts;
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
    }
}
