using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest.Engine.ShapeOutlines
{
    public static class ShapeCreator
    {
        public static Vector2[] RectanglePoints(Vector2 position, float width, float height)
        {
            return [
                position,                                  // Top-left
                new Vector2(width, 0) + position,          // Top-right
                new Vector2(width, height) + position,     // Bottom-right
                new Vector2(0, height) + position,         // Bottom-left
                position                                   // Close the loop
            ];
        }

        //position is the top left corner of the bounding box of the ellipse
        public static Vector2[] EllipsePoints(Vector2 position, float width, float height, int pointsCount = 64)
        {
            const int minPoints = 3, maxPoints = 256;
            pointsCount = MathHelper.Clamp(pointsCount, minPoints, maxPoints);

            Vector2[] points = new Vector2[pointsCount + 1];

            float radiusX = width / 2f;
            float radiusY = height / 2f;

            float step = MathHelper.TwoPi / pointsCount;

            for (int i = 0; i < pointsCount; i++)
            {
                float angle = i * step;
                float x = radiusX * MathF.Cos(angle);
                float y = radiusY * MathF.Sin(angle);
                points[i] = position + new Vector2(radiusX + x, radiusY + y);
            }

            points[pointsCount] = points[0]; // Close loop

            return points;
        }
    }
}
