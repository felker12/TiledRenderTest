using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color);
        }

        /*
        public void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch) //this stays fixed on the screen. If the camera moves this follows
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            BasicEffect basicEffect = new(spriteBatch.GraphicsDevice)
            {
                World = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };

            EffectTechnique effectTechnique = basicEffect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Vertices, 0, Vertices.Length - 1);
            }
        }
        */
    }
}
