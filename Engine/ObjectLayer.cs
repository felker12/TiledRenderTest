using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TiledRenderTest.Engine.ShapeOutlines;

namespace TiledRenderTest.Engine
{
    public class ObjectLayer : Layer
    {
        public List<MapObject> MapObjects { get; set; } = [];

        private readonly Texture2D texture;
        private Color color = Color.White;

        public ObjectLayer(GraphicsDevice graphicsDevice) : base() 
        {
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData([Color.White]);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw objects for debugging (must be set to visible)

            if (!Visible || MapObjects.Count == 0)
                return;

            foreach (var mapObject in MapObjects)
            {
                if (!mapObject.Visible)
                    continue;

                OutlineDrawer.DrawPolygonOutline(spriteBatch, texture, mapObject.PolygonPoints, color, 1f, true);
            }
        }

        public override string ToString()
        {
            return $"ObjectLayer ID: {ID}, Name: {Name}, Width: {Width}, Height: {Height}, Objects Count: {MapObjects.Count}, Visible: {Visible}";
        }
    }
}
