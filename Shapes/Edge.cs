using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public struct Edge(float x, float inverseSlope, int yMax)
    {
        public float x = x;          // Current x at this scanline
        public float inverseSlope = inverseSlope; // 1/m
        public int yMax = yMax;
    }
}
