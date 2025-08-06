using Microsoft.Xna.Framework;

namespace TiledRenderTest.Shapes
{
    public struct Edge(float x, float inverseSlope, int yMax)
    {
        public float x = x;          // Current x at this scanline
        public readonly float inverseSlope = inverseSlope; // 1/m
        public readonly int yMax = yMax;
    }
}
