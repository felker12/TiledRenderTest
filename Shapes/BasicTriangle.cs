using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest.Shapes
{
    public readonly struct BasicTriangle(Vector2 pos1, Vector2 pos2, Vector2 pos3)
    {
        public Vector2 Position1 { get; init; } = pos1;
        public Vector2 Position2 { get; init; } = pos2;
        public Vector2 Position3 { get; init; } = pos3;
        public (Vector2, Vector2)[] Edges { get; } = [(pos1, pos2),
                 (pos2, pos3),
                 (pos3, pos1)];

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

        public readonly bool Intersects(BasicTriangle basicTriangle)
        {
             if(Contains(basicTriangle.Position1) ||
                Contains(basicTriangle.Position2) ||
                Contains(basicTriangle.Position3) ||
                basicTriangle.Contains(Position1) ||
                basicTriangle.Contains(Position2) ||
                basicTriangle.Contains(Position3))
            {
                return true;
            }

            // Check for edge intersections
            var edges1 = Edges;
            var edges2 = basicTriangle.Edges;

            foreach (var edge1 in edges1)
            {
                foreach (var edge2 in edges2)
                {
                    if (LinesIntersect(edge1.Item1, edge1.Item2, edge2.Item1, edge2.Item2))
                        return true;
                }
            }

            return false;

        }

        private static bool LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);

            // Parallel or collinear
            if (MathF.Abs(d) < 0.0001f)
                return false;

            float u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
            float v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

            return u >= 0 && u <= 1 && v >= 0 && v <= 1;
        }

        public override string ToString() => $"Triangle({Position1}, {Position2}, {Position3})";
    }
}
