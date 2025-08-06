using Microsoft.Xna.Framework;

namespace TiledRenderTest.Shapes
{
    public class Polygon : Shape
    {
        protected Polygon(Vector2[] vertices, Color color) : base(color)
        {
            Points = vertices;
        }

        protected Polygon(Vector2[] vertices) : base()
        {
            Points = vertices;
        }

        protected Polygon(Vector2 position, Color color) : base(position, color)
        {
        }

        protected Polygon(Vector2 position) : base(position)
        {
        }

        public Polygon() : base()
        {
        }
    }
}
