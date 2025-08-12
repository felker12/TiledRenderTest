using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace TiledRenderTest.Entities
{
    public class Sprite
    {
        Shapes.Rectangle shapeRectangle = null;

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
        public Shapes.Rectangle ShapeRectangle => shapeRectangle ??= new(Position, Width, Height, Color);

        public Sprite() { }

        public Sprite(GraphicsDevice graphicsDevice)
        {
            Texture = CreateTextureFromColor(Color, graphicsDevice);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Texture, Position, Color.White);
            spriteBatch.Draw(Texture, Position, Frame, Color);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Motion * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            ShapeRectangle?.SetPosition(Position);

            if (ShapeRectangle?.Position != Position)
            {
                //Debug.WriteLine(ShapeRectangle.Points);

                //ShapeRectangle?.SetPosition(Position);

                //Debug.WriteLine($"Position: {Position}, shape Pos: {ShapeRectangle.Position}"); 
                //Debug.WriteLine(ShapeRectangle.ToString());
                //Debug.WriteLine(ShapeRectangle.Points);
            }
        }

        public static Texture2D CreateTextureFromColor(Color color, GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new(graphicsDevice, 1, 1);
            texture.SetData([color]);

            return texture;
        }
    }
}
