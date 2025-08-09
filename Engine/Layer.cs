using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TiledRenderTest.Engine
{
    public abstract class Layer
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public bool Visible { get; set; } = true;

        public Layer()
        {
        
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

        public override string ToString()
        {
            return $"Layer ID: {ID}, Name: {Name}, Width: {Width}, Height: {Height}";
        }
    }
}
