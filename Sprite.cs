using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TiledRenderTest
{
    internal class Sprite
    {
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Motion { get; set; } = Vector2.Zero;
        public Texture2D Texture { get; set; } = null!;
        public Rectangle Frame { get; set; } = new(0, 0, 32, 32);
        public float Speed { get; set; } = 250f;
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle => new((int)Position.X, (int)Position.Y, Width, Height);
        public Shapes.Rectangle ShapeRectangle => new(Position, Width, Height, Color);

        public Sprite() { }

        public Sprite(GraphicsDevice graphicsDevice)
        {
            Texture = CreateTextureFromColor(Color, graphicsDevice);
            Position = new(50, 50);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.Draw(Texture, Position, Frame, Color);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Motion * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static Texture2D CreateTextureFromColor(Color color, GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new(graphicsDevice, 1, 1);
            texture.SetData([color]);

            return texture;
        }
    }
}
