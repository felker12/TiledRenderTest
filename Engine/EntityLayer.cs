using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledRenderTest.Entities;

namespace TiledRenderTest.Engine
{
    internal class EntityLayer : Layer
    {
        EntityManager EntityManager { get; set; } = new();

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw all entities in the layer
            foreach (var entity in EntityManager.Entities)
            {
                entity.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update all entities in the layer
            foreach (var entity in EntityManager.Entities)
            {
                entity.Update(gameTime);
            }
        }
    }
}
