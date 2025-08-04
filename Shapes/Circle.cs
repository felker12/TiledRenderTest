using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest.Shapes
{
    public class Circle : Shape
    {
        public int Radius { get; private set; } = 40;
        public int PointCount { get; private set; } = 32;
        public override Vector2 Center => Position + new Vector2(Radius, Radius);

        public Circle() 
        { 
            Initialize(); 
        }

        public Circle(Vector2 center, int radius, int points, Color color)
        {
            Position = center;
            Radius = radius;
            PointCount = points;
            Color = color;
            Initialize();
        }

        private void Initialize()
        {
            const int minPoints = 3, maxPoints = 256;

            PointCount = MathHelper.Clamp(PointCount, minPoints, maxPoints);
            var newPoints = new Vector2[PointCount + 1]; // Include closing point

            float rotation = MathHelper.TwoPi / PointCount;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = Radius;
            float ay = 0f;

            float bx, by;
            float xOffset = Position.X + Radius;
            float yOffset = Position.Y + Radius;

            for (int i = 0; i < PointCount; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                newPoints[i] = new Vector2(ax + xOffset, ay + yOffset);

                ax = bx;
                ay = by;
            }

            newPoints[PointCount] = newPoints[0]; // Close the shape

            Points = newPoints; // This will call MarkDirty(), triggering rebuild
        }

        public override bool Contains(Vector2 point)
        {
            return Vector2.DistanceSquared(point, Center) <= Radius * Radius;
        }
    }
}
