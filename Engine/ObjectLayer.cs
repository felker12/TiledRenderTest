using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TiledRenderTest.Engine
{
    public class ObjectLayer : Layer
    {
        public List<MapObject> MapObjects { get; set; } = [];

        public ObjectLayer() : base() { }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Optional: draw objects for debugging
        }

        public override string ToString()
        {
            return $"ObjectLayer ID: {ID}, Name: {Name}, Width: {Width}, Height: {Height}, Objects Count: {MapObjects.Count}";
        }
    }
}
