using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TiledRenderTest.Shapes
{
    public class Star : Shape
    {
        public int NumberOfPoints { get; private set; } = 5;
        public float OuterRadius { get; private set; } = 100; 
        public float InnerRadius { get; private set; } = 40;
        public override VertexPositionColor[] FilledVertices => CreateFilledStarVertices(Position, OuterRadius, InnerRadius, Color, NumberOfPoints);

        public Star(Vector2 center, int numbOfPoints = 5, int outerRadius = 100, int innerRadius = 40)
        {
            NumberOfPoints = numbOfPoints; 
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            Position = center + new Vector2(OuterRadius, OuterRadius);

            Points = CreateStarOutline(Position, OuterRadius, InnerRadius, NumberOfPoints);
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
    }
}
