using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest.Shapes
{
    public class Ellipse : Shape
    {
        public int RadiusX { get; private set; } = 40;
        public int RadiusY { get; private set; } = 40;
        public int PointCount { get; private set; } = 32;
        public override Vector2 Center => Position + new Vector2(RadiusX, RadiusY);

        public Ellipse()
        {
            Initialize();
        }

        public Ellipse(Vector2 center, int radiusX, int radiusY, int points, Color color)
        {
            Position = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            PointCount = points;
            Color = color;
            DefaultColor = color;
            Initialize();
        }

        private void Initialize()
        {
            const int minPoints = 3, maxPoints = 256;
            PointCount = MathHelper.Clamp(PointCount, minPoints, maxPoints);

            Vector2[] newPoints = new Vector2[PointCount + 1];
            float step = MathHelper.TwoPi / PointCount;

            for (int i = 0; i < PointCount; i++)
            {
                float angle = i * step;
                float x = RadiusX * MathF.Cos(angle);
                float y = RadiusY * MathF.Sin(angle);
                newPoints[i] = Position + new Vector2(RadiusX + x, RadiusY + y);
            }

            newPoints[PointCount] = newPoints[0]; // Close loop
            Points = newPoints; // Will call MarkDirty()
        }

        public override bool Contains(Vector2 point)
        {
            Vector2 diff = point - Center;
            float normX = diff.X / RadiusX;
            float normY = diff.Y / RadiusY;
            return (normX * normX + normY * normY) <= 1f;
        }
    }
}
