using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TiledRenderTest.Shapes
{
    public class Shape
    {
        //The fields should only be set through rebuilding methods (in this case RebuildIfDirty())
        // Index-based triangulation data
        protected int[] triangleIndices; // Every 3 indices form a triangle
        protected int[] triangulationLineIndices; // Indices for drawing triangulation lines

        protected Line[] sides; 
        protected BasicTriangle[] triangles;
        protected Vector2 center;
        protected bool isDirty = true;
        protected BasicEffect basicEffect;
        protected Vector2[] points = [],
            rotationPoints; // Original points relative to center for rotation
        protected VertexPositionColor[] perimeterVertices,
            filledVertices,
            triangleVertices,
            thickVertices;
        protected Texture2D texture;
        protected float currentRotation = 0f; // Track current rotation angle

        protected Color color = Color.White;
        public Color DefaultColor { get; protected set; } = Color.White;
        public float RotationSpeedDegreesPerSecond { get; set; } = 90f;
        public Vector2 Position { get; protected set; } = Vector2.Zero;
        public Color Color { get => color; set { color = value; MarkDirty(); } } // Default color
        //public Texture2D Texture { get { texture ??= Game1.CreateTextureFromColor(Color); return texture; } }
        public Texture2D Texture { get; set; } = Game1.CreateTextureFromColor(Color.White);
        public virtual Vector2 Center { get { RebuildIfDirty(); return center; } }
        public virtual Vector2[] Points { get => points; protected set { points = value; MarkDirty(); } }
        public virtual Line[] Lines { get { RebuildIfDirty(); return sides; } }
        public virtual VertexPositionColor[] PerimeterVertices { get { RebuildIfDirty(); return perimeterVertices; } }
        public virtual VertexPositionColor[] FilledVertices { get { RebuildIfDirty(); return filledVertices; } }
        public virtual VertexPositionColor[] TriangleVertices { get { RebuildIfDirty(); return triangleVertices; } }
        public virtual VertexPositionColor[] ThickLineVertices { get { RebuildIfDirty(); return thickVertices; } }
        public virtual int[] TriangleIndices { get { RebuildIfDirty(); return triangleIndices; } }
        public virtual int TriangleCount { get { RebuildIfDirty(); return triangleIndices?.Length / 3 ?? 0; } }
        public virtual int[] TriangulationLineIndices { get { RebuildIfDirty(); return triangulationLineIndices; } }
        public bool Rotate { get; set; } = false; // Default rotation state
        public int LineThickness { get; protected set; } = 1; // Default line thickness
        public virtual BasicTriangle[] Triangles { get { RebuildIfDirty(); return triangles; } }

        public Shape(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            DefaultColor = color;
        }

        public Shape(Vector2 position)
        {
            Position = position;
        }

        public Shape(Color color)
        {
            Color = color;
            DefaultColor = color;
        }

        public Shape()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Rotate)
            {
                PerformRotation(gameTime);
            }
        }

        public virtual void PerformRotation(GameTime gameTime)
        {
            if (rotationPoints == null || rotationPoints.Length == 0) return;

            float rotationStepRadians = MathHelper.ToRadians(RotationSpeedDegreesPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float sin = MathF.Sin(rotationStepRadians);
            float cos = MathF.Cos(rotationStepRadians);

            // Rotate each point around the center by delta angle
            for (int i = 0; i < rotationPoints.Length; i++)
            {
                Vector2 p = points[i] - center; // current relative to center

                points[i] = new Vector2(
                    cos * p.X - sin * p.Y + center.X,
                    sin * p.X + cos * p.Y + center.Y
                );
            }

            MarkDirty();
        }

        //SpriteBatch methods for drawing
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawOutline(spriteBatch);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var line in Lines)
                line.Draw(spriteBatch, Texture, outlineColor, outlineThickness);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            DrawOutline(spriteBatch, Color, outlineThickness);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch)
        {
            DrawOutline(spriteBatch, Color);
        }

        public virtual void DrawFilled(SpriteBatch spriteBatch)
        {
            DrawFilled(spriteBatch, Color);
        }

        public virtual void DrawFilled(SpriteBatch spriteBatch, Color fillColor)
        {
            var vertices = FilledVertices;
            if (vertices == null || vertices.Length == 0) return;

            Dictionary<int, List<Edge>> edgeTable = [];
            int yMin = int.MaxValue, yMax = int.MinValue;

            // Build the edge table from triangle vertices
            for (int i = 0; i < vertices.Length; i += 3)
            {
                if (i + 2 >= vertices.Length) break;

                var p1 = vertices[i].Position;
                var p2 = vertices[i + 1].Position;
                var p3 = vertices[i + 2].Position;

                InsertEdge(edgeTable, p1, p2);
                InsertEdge(edgeTable, p2, p3);
                InsertEdge(edgeTable, p3, p1);

                yMin = Math.Min(yMin, (int)MathF.Floor(MathF.Min(p1.Y, MathF.Min(p2.Y, p3.Y))));
                yMax = Math.Max(yMax, (int)MathF.Ceiling(MathF.Max(p1.Y, MathF.Max(p2.Y, p3.Y))));
            }

            List<Edge> activeEdges = [];

            for (int y = yMin; y < yMax; y++)
            {
                if (edgeTable.TryGetValue(y, out var newEdges))
                    activeEdges.AddRange(newEdges);

                activeEdges.RemoveAll(e => e.yMax <= y);
                activeEdges.Sort((a, b) => a.x.CompareTo(b.x));

                for (int i = 0; i < activeEdges.Count - 1; i += 2)
                {
                    float xStart = activeEdges[i].x;
                    float xEnd = activeEdges[i + 1].x;

                    int startX = (int)MathF.Floor(xStart);
                    int endX = (int)MathF.Ceiling(xEnd);
                    int width = Math.Max(1, endX - startX);

                    spriteBatch.Draw(Texture, new Microsoft.Xna.Framework.Rectangle(startX, y, width, 1), fillColor);
                }

                for (int i = 0; i < activeEdges.Count; i++)
                {
                    var edge = activeEdges[i];
                    edge.x += edge.inverseSlope;
                    activeEdges[i] = edge;
                }
            }
        }

        public virtual void DrawTriangulated(SpriteBatch spriteBatch)
        {
            DrawTriangulated(spriteBatch, Color);
        }

        public virtual void DrawTriangulated(SpriteBatch spriteBatch, Color color)
        {
            // Draw triangulation lines using the triangulation vertices
            for (int i = 0; i < TriangleVertices.Length; i += 2)
            {
                if (i + 1 < TriangleVertices.Length)
                {
                    var line = new Line(
                        new Vector2(TriangleVertices[i].Position.X, TriangleVertices[i].Position.Y),
                        new Vector2(TriangleVertices[i + 1].Position.X, TriangleVertices[i + 1].Position.Y),
                        color
                    );
                    line.Draw(spriteBatch, Texture, color);
                }
            }
        }

        //Methods for drawing outlines using primitives
        public virtual void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            basicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineStrip,
                    PerimeterVertices,
                    0,
                    PerimeterVertices.Length - 1
                );
            }
        }

        public virtual void DrawTriangulatedUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            basicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
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
            basicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

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

        public virtual void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public virtual void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 1)
        {
            RebuildThickVertices(thickness);

            basicEffect = InitializeBasicEffect(graphicsDevice, transformMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ThickLineVertices, 0, ThickLineVertices.Length / 3); 
            }
        }

        //Helper methods
        public override string ToString()
        {
            string returnString = 
                $"Shake: {this.GetType().Name}, Position1: {Position}, Color: {Color}, Is Rotating: {Rotate}";

            if(Rotate)
                returnString += $", Rotation Speed: {RotationSpeedDegreesPerSecond} degrees/sec";

            return returnString ;
        }

        public static Vector3 ToVector3(Vector2 vector)
        {
            return new(vector, 0);
        }

        public static VertexPositionColor ToVertexPositionColor(Vector2 vector, Color? color)
        {
            return new VertexPositionColor(ToVector3(vector), color ?? Color.White);
        }

        public static VertexPositionColor[] ToVertexPositionColor(Vector2[] vector, Color? color)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                vertices[i] = ToVertexPositionColor(vector[i], color);

            return vertices;
        }

        public static Line[] ToLines(Vector2[] vectors, Color color)
        {
            int lineCount = vectors.Length - 1;
            var lines = new Line[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                lines[i] = new Line(vectors[i], vectors[i + 1], color);
            }

            return lines;
        }

        public static Line[] ToLines(List<Vector2> vectors, Color color)
        {
            return ToLines(vectors.ToArray(), color);
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

        public void MarkDirty()
        {
            isDirty = true;
        }

        protected virtual void RebuildIfDirty()
        {
            if (!isDirty) return;

            if (points == null || points.Length == 0)
            {
                triangleIndices = [];
                triangulationLineIndices = [];
                sides = [];
                filledVertices = [];
                perimeterVertices = [];
                triangleVertices = [];
                thickVertices = [];
                isDirty = false;
                return;
            }

            center = GetCentroid(points);

            // Create rotation points array - store original points relative to center
            if (rotationPoints == null || rotationPoints.Length != points.Length)
                rotationPoints = new Vector2[points.Length];

            // Store the original points relative to center for rotation
            for (int i = 0; i < points.Length; i++)
            {
                rotationPoints[i] = points[i] - center;
            }

            // Generate triangle indices instead of Triangle objects
            triangleIndices = GenerateTriangleIndices(points);
            filledVertices = GenerateFilledVerticesFromIndices(points, triangleIndices);
            sides = ToLines(points, Color);

            //perimeterVertices = new VertexPositionColor[points.Length];
            //for (int i = 0; i < points.Length; i++)
            //{
            //    perimeterVertices[i] = ToVertexPositionColor(points[i], Color);
            //}

            perimeterVertices = [];
            foreach (var line in sides)
            {
                perimeterVertices = [.. perimeterVertices, .. line.Vertices]; // Combine all vertices
            }

            // Generate triangulation line indices for wireframe triangulation
            triangulationLineIndices = GenerateTriangulationLineIndices(points);
            triangleVertices = GenerateTriangulationVerticesFromIndices(points, triangulationLineIndices); //used when drawing outline with triangles

            // Build BasicTriangle array from triangleIndices
            triangles = GenerateBasicTriangles(points, triangleIndices);

            isDirty = false;
        }

        public void RebuildThickVertices(int thickness)
        {
            RebuildIfDirty();
            LineThickness = thickness;

            thickVertices = [];
            foreach (var line in sides)
            {
                line.RebuildThickVertices(thickness); // Ensure each line has its thick vertices built
                thickVertices = [.. thickVertices, .. line.ThickVertices]; // Combine all thick vertices
            }
        }

        protected void ClearCache()
        {
            perimeterVertices = null;
            filledVertices = null;
            triangleIndices = null;
            triangulationLineIndices = null;
            thickVertices = null;
            sides = null;
            rotationPoints = null; // Clear rotation points cache

            MarkDirty();
        }

        public virtual bool Contains(Vector2 point)
        {
            // This method can be overridden in derived classes to implement specific containment logic
            RebuildIfDirty();

            if (triangles == null || triangles.Length == 0)
                return false;

            foreach (var tri in triangles)
            {
                if (tri.Contains(point))
                    return true;
            }

            return false;
        }

        public virtual void Intersects(Shape otherShape)
        {
            // This method can be overridden in derived classes to implement specific intersection logic
            // For now, it does nothing
        }

        /// <summary>
        /// Generates triangle indices for fan triangulation from center point
        /// </summary>
        protected virtual int[] GenerateTriangleIndices(Vector2[] vertices)
        {
            if (vertices == null || vertices.Length < 3) return [];

            bool isClosed = ApproximatelyEqual(vertices[0], vertices[^1]);
            int uniqueCount = isClosed ? vertices.Length - 1 : vertices.Length;

            if (uniqueCount < 3) return [];

            // Create extended vertex array that includes center point
            // Center will be at index uniqueCount
            int centerIndex = uniqueCount;

            // Each triangle uses: center, current vertex, next vertex
            var indices = new List<int>();

            for (int i = 0; i < uniqueCount; i++)
            {
                int nextIndex = (i + 1) % uniqueCount;

                // Add triangle indices: center, current, next
                indices.Add(centerIndex); // center point
                indices.Add(i);          // current vertex
                indices.Add(nextIndex);  // next vertex
            }

            return [.. indices];
        }

        /// <summary>
        /// Generates line indices for triangulation wireframe (center to vertices + perimeter)
        /// </summary>
        protected virtual int[] GenerateTriangulationLineIndices(Vector2[] vertices)
        {
            if (vertices == null || vertices.Length < 3) return [];

            bool isClosed = ApproximatelyEqual(vertices[0], vertices[^1]);
            int uniqueCount = isClosed ? vertices.Length - 1 : vertices.Length;

            if (uniqueCount < 3) return [];

            int centerIndex = uniqueCount;
            var indices = new List<int>();

            // Lines from center to each vertex
            for (int i = 0; i < uniqueCount; i++)
            {
                indices.Add(centerIndex);
                indices.Add(i);
            }

            // Perimeter lines
            for (int i = 0; i < uniqueCount; i++)
            {
                int nextIndex = (i + 1) % uniqueCount;
                indices.Add(i);
                indices.Add(nextIndex);
            }

            return [.. indices];
        }

        /// <summary>
        /// Generates filled vertices from triangle indices
        /// Creates extended vertex array with center point included
        /// </summary>
        protected virtual VertexPositionColor[] GenerateFilledVerticesFromIndices(Vector2[] vertices, int[] indices)
        {
            if (vertices == null || indices == null || indices.Length == 0) return [];

            bool isClosed = ApproximatelyEqual(vertices[0], vertices[^1]);
            int uniqueCount = isClosed ? vertices.Length - 1 : vertices.Length;

            // Create extended points array that includes center
            var extendedVertices = new VertexPositionColor[uniqueCount + 1];

            // Add original vertices
            for (int i = 0; i < uniqueCount; i++)
            {
                extendedVertices[i] = ToVertexPositionColor(vertices[i], Color);
            }

            // Add center vertex
            extendedVertices[uniqueCount] = ToVertexPositionColor(center, Color);

            // Create triangle vertices using indices
            var triangleVerts = new VertexPositionColor[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                triangleVerts[i] = extendedVertices[indices[i]];
            }

            return triangleVerts;
        }

        /// <summary>
        /// Generates vertices for triangulation wireframe from line indices
        /// </summary>
        protected virtual VertexPositionColor[] GenerateTriangulationVerticesFromIndices(Vector2[] vertices, int[] lineIndices)
        {
            if (vertices == null || lineIndices == null || lineIndices.Length == 0) return [];

            bool isClosed = ApproximatelyEqual(vertices[0], vertices[^1]);
            int uniqueCount = isClosed ? vertices.Length - 1 : vertices.Length;

            // Create extended points array that includes center
            var extendedPoints = new Vector2[uniqueCount + 1];
            Array.Copy(vertices, extendedPoints, Math.Min(uniqueCount, vertices.Length));
            extendedPoints[uniqueCount] = center;

            // Create line vertices using indices
            var lineVerts = new VertexPositionColor[lineIndices.Length];
            for (int i = 0; i < lineIndices.Length; i++)
            {
                lineVerts[i] = ToVertexPositionColor(extendedPoints[lineIndices[i]], Color);
            }

            return lineVerts;
        }

        private static void InsertEdge(Dictionary<int, List<Edge>> edgeTable, Vector3 p1, Vector3 p2)
        {
            if (p1.Y == p2.Y) return; // skip horizontal edges

            if (p1.Y > p2.Y)
                (p1, p2) = (p2, p1);

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float invSlope = dx / dy;

            int yStart = (int)MathF.Ceiling(p1.Y);
            int yEnd = (int)MathF.Ceiling(p2.Y);

            float xStart = p1.X + invSlope * (yStart - p1.Y); // interpolate x at yStart

            if (!edgeTable.TryGetValue(yStart, out var list))
                edgeTable[yStart] = list = [];

            list.Add(new Edge(xStart, invSlope, yEnd));
        }

        public virtual VertexPositionColor[] GetOutlineAsLineList()
        {
            if (PerimeterVertices == null || PerimeterVertices.Length < 2)
                return [];

            var lines = new List<VertexPositionColor>();

            for (int i = 0; i < PerimeterVertices.Length - 1; i++)
            {
                lines.Add(PerimeterVertices[i]);
                lines.Add(PerimeterVertices[i + 1]);
            }

            // Close the loop
            lines.Add(PerimeterVertices[^1]);
            lines.Add(PerimeterVertices[0]);

            return [.. lines];
        }

        protected virtual BasicTriangle[] GenerateBasicTriangles(Vector2[] vertices, int[] indices)
        {
            if (vertices == null || indices == null || indices.Length % 3 != 0) return [];

            bool isClosed = ApproximatelyEqual(vertices[0], vertices[^1]);
            int uniqueCount = isClosed ? vertices.Length - 1 : vertices.Length;

            // Create extended vertices array with center point
            var extendedPoints = new Vector2[uniqueCount + 1];
            Array.Copy(vertices, 0, extendedPoints, 0, uniqueCount);
            extendedPoints[uniqueCount] = center;

            var result = new BasicTriangle[indices.Length / 3];
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector2 a = extendedPoints[indices[i]];
                Vector2 b = extendedPoints[indices[i + 1]];
                Vector2 c = extendedPoints[indices[i + 2]];

                result[i / 3] = new BasicTriangle(a, b, c);
            }

            return result;
        }

        protected static bool PointInTriangle(Vector2 pt, Vector2 a, Vector2 b, Vector2 c)
        {
            float dX = pt.X - c.X;
            float dY = pt.Y - c.Y;
            float dX21 = c.X - b.X;
            float dY12 = b.Y - c.Y;
            float D = dY12 * (a.X - c.X) + dX21 * (a.Y - c.Y);
            float s = dY12 * dX + dX21 * dY;
            float t = (c.Y - a.Y) * dX + (a.X - c.X) * dY;

            if (D < 0)
                return s <= 0 && t <= 0 && s + t >= D;

            return s >= 0 && t >= 0 && s + t <= D;
        }
    }
}