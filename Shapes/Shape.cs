using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TiledRenderTest.Shapes
{
    public class Shape
    {
        // Index-based triangulation data
        protected int[] triangleIndices; // Every 3 indices form a triangle
        protected int[] triangulationLineIndices; // Indices for drawing triangulation lines

        protected Line[] sides;
        protected Vector2 center;
        protected bool isDirty = true;
        protected BasicEffect basicEffect;
        protected Vector2[] points = [],
            rotationPoints; // Original points relative to center for rotation
        protected VertexPositionColor[] perimeterVertices,
            filledVertices,
            triangleVertices;

        private List<VertexPositionColor> thickVertices = [];

        private float _cachedSin, _cachedCos, _lastRotationStep = float.NaN; 
        private BoundingBox _boundingBox;
        private bool _boundingBoxDirty = true;

        protected float currentRotation = 0f; // Track current rotation angle

        protected Color color = Color.White;
        public Color DefaultColor { get; protected set; } = Color.White;
        public float RotationSpeedDegreesPerSecond { get; set; } = 90f;
        public Vector2 Position { get; protected set; } = Vector2.Zero;
        public Color Color { get => color; set { color = value; MarkDirty(); } } // Default color
        public Texture2D Texture { get; set; } = Game1.CreateTextureFromColor(Color.White);
        public virtual Vector2 Center { get; set; }
        public virtual Vector2[] Points { get => points; protected set { points = value; MarkDirty(); } }
        public virtual Line[] Lines { get { return sides; } }
        public virtual VertexPositionColor[] PerimeterVertices { get { return perimeterVertices; } }
        public virtual VertexPositionColor[] FilledVertices { get { return filledVertices; } }
        public virtual VertexPositionColor[] TriangleVertices { get { return triangleVertices; } }
        public List<VertexPositionColor> ThickLineVertices => thickVertices;
        public virtual int[] TriangleIndices { get { return triangleIndices; } }
        public virtual int TriangleCount { get { return triangleIndices?.Length / 3 ?? 0; } }
        public virtual int[] TriangulationLineIndices { get { return triangulationLineIndices; } }
        public bool Rotate { get; set; } = false; // Default rotation state
        public int LineThickness { get; protected set; } = 1; // Default line thickness
        //public virtual BasicTriangle[] Triangles { get { return triangles; } }
        public virtual BasicTriangle[] Triangles { get; set; }

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
            if (isDirty)
                RebuildIfDirty(); // Only rebuild once per frame

            if (Rotate)
            {
                PerformRotation(gameTime);
            }
        }

        public virtual void PerformRotation(GameTime gameTime)
        {
            if (rotationPoints == null || rotationPoints.Length == 0) return;

            float rotationStepRadians = MathHelper.ToRadians(RotationSpeedDegreesPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Cache trig calculations if rotation step hasn't changed
            if (_lastRotationStep != rotationStepRadians)
            {
                _cachedSin = MathF.Sin(rotationStepRadians);
                _cachedCos = MathF.Cos(rotationStepRadians);
                _lastRotationStep = rotationStepRadians;
            }

            // Rotate each point around the center by delta angle
            for (int i = 0; i < rotationPoints.Length; i++)
            {
                Vector2 p = points[i] - center; // current relative to center

                points[i] = new Vector2(
                    _cachedCos * p.X - _cachedSin * p.Y + center.X,
                    _cachedSin * p.X + _cachedCos * p.Y + center.Y
                );
            }

            MarkDirty();
        }

        //SpriteBatch methods for drawing
        public virtual void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var line in Lines)
                line.Draw(spriteBatch, Texture, Color, outlineThickness);
        }

        public virtual void DrawFilled(SpriteBatch spriteBatch)
        {
            var vertices = FilledVertices;
            if (vertices == null || vertices.Length == 0) return;

            int yMin = int.MaxValue, yMax = int.MinValue;

            // Build the edge table from triangle vertices
            for (int i = 0; i < vertices.Length; i += 3)
            {
                if (i + 2 >= vertices.Length) break;

                var p1 = vertices[i].Position;
                var p2 = vertices[i + 1].Position;
                var p3 = vertices[i + 2].Position;

                yMin = Math.Min(yMin, (int)MathF.Floor(MathF.Min(p1.Y, MathF.Min(p2.Y, p3.Y))));
                yMax = Math.Max(yMax, (int)MathF.Ceiling(MathF.Max(p1.Y, MathF.Max(p2.Y, p3.Y))));
            }

            Dictionary<int, List<Edge>> edgeTable = new(yMax - yMin + 1);

            for (int i = 0; i < vertices.Length; i += 3)
            {
                var p1 = vertices[i].Position;
                var p2 = vertices[i + 1].Position;
                var p3 = vertices[i + 2].Position;

                InsertEdge(edgeTable, p1, p2);
                InsertEdge(edgeTable, p2, p3);
                InsertEdge(edgeTable, p3, p1);
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

                    spriteBatch.Draw(Texture, new Microsoft.Xna.Framework.Rectangle(startX, y, width, 1), Color);
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
                    line.Draw(spriteBatch, Texture, Color);
                }
            }
        }

        //Helper methods
        public override string ToString()
        {
            string returnString =
                $"Shake: {this.GetType().Name}, Position1: {Position}, Color: {Color}, Is Rotating: {Rotate}";

            if (Rotate)
                returnString += $", Rotation Speed: {RotationSpeedDegreesPerSecond} degrees/sec";

            return returnString;
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
            _boundingBoxDirty = true;
        }

        public void RebuildIfDirty()
        {
            if (!isDirty)
                return;

            isDirty = false;

            if (points == null || points.Length < 3)
            {
                ClearBuffers();
                return;
            }

            RebuildGeometry();
            RebuildTriangulation();
            RebuildVertices();
        }


        private void RebuildGeometry()
        {
            center = GetCentroid(points);

            // Update bounding boxes if needed
            if (_boundingBoxDirty)
            {
                UpdateBoundingBox();
                _boundingBoxDirty = false;
            }

            // Create rotation points array - store original points relative to center
            if (rotationPoints == null || rotationPoints.Length != points.Length)
                rotationPoints = new Vector2[points.Length];

            // Store the original points relative to center for rotation
            for (int i = 0; i < points.Length; i++)
            {
                rotationPoints[i] = points[i] - center;
            }
        }

        private void RebuildTriangulation()
        {
            // Generate triangle indices instead of Triangle objects
            triangleIndices = GenerateTriangleIndices(points);
            filledVertices = GenerateFilledVerticesFromIndices(points, triangleIndices);
            sides = ToLines(points, Color);

            // Generate triangulation line indices for wireframe triangulation
            triangulationLineIndices = GenerateTriangulationLineIndices(points);
            triangleVertices = GenerateTriangulationVerticesFromIndices(points, triangulationLineIndices); //used when drawing outline with triangles

            // Build BasicTriangle array from triangleIndices
            Triangles = GenerateBasicTriangles(points, triangleIndices);
        }

        private void RebuildVertices()
        {
            int totalVerts = 0;
            foreach (var line in sides)
                totalVerts += line.Vertices.Length;

            var buffer = new VertexPositionColor[totalVerts];
            var span = new Span<VertexPositionColor>(buffer);

            int offset = 0;
            foreach (var line in sides)
            {
                var verts = line.Vertices;
                verts.CopyTo(span.Slice(offset, verts.Length));
                offset += verts.Length;
            }
            perimeterVertices = buffer;
        }

        public void RebuildThickVertices(int thickness)
        {
            thickVertices.Clear();

            foreach (var line in sides)
            {
                line.RebuildThickVertices(thickness);
                thickVertices.AddRange(line.ThickVertices); // Combine all thick vertices
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

        private void ClearBuffers()
        {
            triangleIndices = [];
            sides = [];
            triangleVertices = [];
            filledVertices = [];
            perimeterVertices = [];
            thickVertices = [];
        }

        private void UpdateBoundingBox()
        {
            if (points == null || points.Length == 0)
            {
                _boundingBox = new BoundingBox();
                return;
            }

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            _boundingBox = new BoundingBox(
                new Vector3(minX, minY, 0),
                new Vector3(maxX, maxY, 0)
            );
        }

        public virtual bool Contains(Vector2 point)
        {
            // This method can be overridden in derived classes to implement specific containment logic
            RebuildIfDirty();

            var point3D = new Vector3(point, 0);
            if (!_boundingBox.Contains(point3D).HasFlag(ContainmentType.Contains))
                return false;

            if (Triangles?.Length == 0) return false;

            // Use for loop instead of foreach for slight performance gain
            for (int i = 0; i < Triangles.Length; i++)
            {
                if (Triangles[i].Contains(point))
                    return true;
            }

            return false;
        }

        public virtual bool Intersects(Shape otherShape)
        {
            RebuildIfDirty();
            otherShape.RebuildIfDirty();

            // Quick bounding box check first
            if (!_boundingBox.Intersects(otherShape._boundingBox))
                return false;

            // Check for triangle intersections
            if (Triangles == null || Triangles.Length == 0)
                return false;

            foreach (var tri in Triangles)
            {
                foreach (var otherTri in otherShape.Triangles)
                {
                    if (tri.Intersects(otherTri))
                        return true;
                }
            }

            return false;
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
            var outputSpan = new Span<VertexPositionColor>(triangleVerts);
            var sourceSpan = new ReadOnlySpan<VertexPositionColor>(extendedVertices);

            for (int i = 0; i < indices.Length; i++)
                outputSpan[i] = sourceSpan[indices[i]];

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

            int count = PerimeterVertices.Length;
            var output = new VertexPositionColor[count * 2];

            Span<VertexPositionColor> span = output;

            for (int i = 0; i < count - 1; i++)
            {
                span[i * 2] = PerimeterVertices[i];
                span[i * 2 + 1] = PerimeterVertices[i + 1];
            }

            // Close the loop
            span[(count - 1) * 2] = PerimeterVertices[^1];
            span[(count - 1) * 2 + 1] = PerimeterVertices[0];

            return output;
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

            var triangleSpan = new Span<BasicTriangle>(new BasicTriangle[indices.Length / 3]);
            var pointsSpan = new ReadOnlySpan<Vector2>(extendedPoints);
            var indexSpan = new ReadOnlySpan<int>(indices);

            for (int i = 0; i < triangleSpan.Length; i++)
            {
                int idx = i * 3;
                triangleSpan[i] = new BasicTriangle(
                    pointsSpan[indexSpan[idx]],
                    pointsSpan[indexSpan[idx + 1]],
                    pointsSpan[indexSpan[idx + 2]]
                );
            }

            return triangleSpan.ToArray(); // Or store as field if preferred
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