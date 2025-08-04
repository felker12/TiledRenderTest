using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public readonly struct BasicTriangle(Vector2 pos1, Vector2 pos2, Vector2 pos3)
    {
        public Vector2 Position1 { get; init; } = pos1;
        public Vector2 Position2 { get; init; } = pos2;
        public Vector2 Position3 { get; init; } = pos3;

        public readonly bool Contains(Vector2 pt)
        {
            float dX = pt.X - Position3.X;
            float dY = pt.Y - Position3.Y;
            float dX21 = Position3.X - Position2.X;
            float dY12 = Position2.Y - Position3.Y;
            float D = dY12 * (Position1.X - Position3.X) + dX21 * (Position1.Y - Position3.Y);
            float s = dY12 * dX + dX21 * dY;
            float t = (Position3.Y - Position1.Y) * dX + (Position1.X - Position3.X) * dY;

            if (D < 0)
                return s <= 0 && t <= 0 && s + t >= D;

            return s >= 0 && t >= 0 && s + t <= D;
        }
    }
}
