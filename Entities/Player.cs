using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TiledRenderTest.Entities
{
    internal class Player : Entity
    {
        public Player() : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            Vector2 motion = Vector2.Zero;

            // Handle player input and update motion
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
                motion.Y = -1;
            if (keyboardState.IsKeyDown(Keys.S))
                motion.Y = 1;
            if (keyboardState.IsKeyDown(Keys.A))
                motion.X = -1;
            if (keyboardState.IsKeyDown(Keys.D))
                motion.X = 1;

            if(motion != Vector2.Zero)
                motion.Normalize(); // Normalize the motion vector to ensure consistent speed

            Motion = motion;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
