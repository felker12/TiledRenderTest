using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace TiledRenderTest.Engine
{
    public abstract class Layer
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Width { get; set; } = 1f;
        public float Height { get; set; } = 1f;
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
