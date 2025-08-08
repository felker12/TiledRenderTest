using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TiledRenderTest.Entities
{
    internal class EntityManager
    {
        public List<Entity> Entities { get; set; } = [];

        public EntityManager() { }

        public EntityManager(List<Entity> entities)
        {
            Entities = entities ?? [];
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in Entities)
            {
                entity.Draw(spriteBatch);
            }
        }

        public void Add(Entity entity)
        {
            if (entity != null)
            {
                Entities.Add(entity);
            }
        }

        public void Remove(Entity entity)
        {
            if (entity != null && Entities.Contains(entity))
            {
                Entities.Remove(entity);
            }
        }

        public void Clear()
        {
            Entities.Clear();
        }
    }
}
