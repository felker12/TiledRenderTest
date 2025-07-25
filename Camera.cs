using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest
{
    internal class Camera(int screenW, int screenH)
    {
        private int _mapWidth, _mapHeight;

        public Vector2 Position = new();
        public int Width { get; set; } = screenW;
        public int Height { get; set; } = screenH;

        //Round the position to whole numbers to align with the grid and prevent subpixel rendering (seams around the edges of tiles)
        public Matrix Transformation => Matrix.CreateTranslation(
            new Vector3(-new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y)), 0f));

        public void Update(Vector2 playerPosition)
        {
            Position.X = playerPosition.X - Width / 2;
            Position.Y = playerPosition.Y - Height / 2;

            //lock the camera to the map
            //Position.X = MathHelper.Clamp(Position.X, 0, _mapWidth - Width);
            //Position.Y = MathHelper.Clamp(Position.Y, 0, _mapHeight - Height);
        }

        public void SetBounds(int width, int height)
        {
            _mapWidth = width;
            _mapHeight = height;
        }
    }
}
