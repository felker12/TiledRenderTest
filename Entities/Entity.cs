using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Entities
{
    internal class Entity : Sprite
    {

        public Entity() { }

        public Entity(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            // Initialize entity-specific properties if needed
        }
    }
}
