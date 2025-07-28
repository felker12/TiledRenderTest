using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Circle : Shape
    {
        public int Radius { get; private set; } = 40;
        public int PointCount { get; private set; } = 32;
        private Line[] Lines { get; set; }
        public override Vector2 Center => Position + new Vector2(Radius, Radius);
        public override Triangle[] Triangles => Triangulate(Points, Center);

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
            Lines = new Line[PointCount];
            Points = new Vector2[PointCount + 1];

            float rotation = MathHelper.TwoPi / (float)PointCount;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = Radius;
            float ay = 0f;

            float bx, by;
            float xOffset = Position.X + Radius;
            float yOffset = Position.Y + Radius;

            for(int i = 0; i < PointCount; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                Lines[i] = new(ax + xOffset, ay + yOffset, bx + xOffset, by + yOffset, Color);
                Points[i] = new(ax + xOffset, ay + yOffset);

                ax = bx;
                ay = by;
            }

            Points[PointCount] = Points[0];
        }
    }
}
