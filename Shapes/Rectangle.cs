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
            return point.X >= Position.X && point.X <= Position.X + Width &&
                   point.Y >= Position.Y && point.Y <= Position.Y + Height;
        }
    }
}
