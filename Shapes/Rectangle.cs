using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TiledRenderTest.Shapes
{
    public class Rectangle : Shape
    {
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;

        public Rectangle(int x, int y, int width, int height) :
            this(new Vector2(x, y), width, height) { }

        public Rectangle(int x, int y, int width, int height, Color color)
            : this(new(x, y), width, height, color) { }

        public Rectangle(Vector2 position, int width, int height) 
            : base(position)
        {
            Width = width;
            Height = height;
            Initialize();
        }

        public Rectangle(Vector2 position, int width, int height, Color color)
            : base(position, color)
        {
            Width = width;
            Height = height;
            Initialize();
        }

        private void Initialize()
        {
            Points =
            [
                new Vector2(0, 0) + Position,              // Top-left
                new Vector2(Width, 0) + Position,          // Top-right
                new Vector2(Width, Height) + Position,     // Bottom-right
                new Vector2(0, Height) + Position,         // Bottom-left
                new Vector2(0, 0)  + Position              // Close the loop
            ];
        }

        //public override void DrawFilled(SpriteBatch spriteBatch)
        //{
        //    spriteBatch.Draw(Texture, Position, Color);


        public override bool Contains(Vector2 point)
        {
            RebuildIfDirty(); // Make sure points are up to date with rotation

            if (points == null || points.Length < 4)
                return false;

            // Rectangle = 2 triangles: (0,1,2) and (0,2,3)
            return PointInTriangle(point, points[0], points[1], points[2]) ||
                   PointInTriangle(point, points[0], points[2], points[3]);
        }

        public override bool Intersects(Shape otherShape)
        {
            RebuildIfDirty();

            if (points == null || points.Length < 4 || otherShape.Points == null || otherShape.Points.Length < 3)
                return false;

            // Create the 2 triangles for this rectangle
            BasicTriangle[] thisTriangles =
            [
                new(points[0], points[1], points[2]),
                new(points[0], points[2], points[3])
            ];

            // Triangulate the other shape
            var otherTriangles = otherShape.Triangles;

            foreach (var tri1 in thisTriangles)
            {
                foreach (var tri2 in otherTriangles)
                {
                    if (tri1.Intersects(tri2))
                        return true;
                }
            }

            return false;
        }
    }
}
