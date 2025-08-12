using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TiledRenderTest.Engine.ShapeOutlines
{
    public static class OutlineDrawer
    {
        private const float PointEpsilon = 0.0001f; // Tolerance for considering two points equal

        public static void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 distance = end - start;
            float angle = (float)Math.Atan2(distance.Y, distance.X);

            spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, 
                new Vector2(distance.Length(), thickness), SpriteEffects.None, 0f);
        }

        public static void DrawPolygonOutline(SpriteBatch spriteBatch, Texture2D pixel, Vector2[] points, Color color, float thickness = 1f, bool closeLoop = false)
        {
            if (spriteBatch == null || pixel == null || points == null || points.Length < 2)
                return;

            int lastIndex = points.Length - 1;

            // Use a small epsilon to check if the first and last points are close enough to be considered equal
            bool alreadyClosed = Vector2.DistanceSquared(points[0], points[lastIndex]) < (PointEpsilon * PointEpsilon);

            // Draw each segment
            for (int i = 0; i < lastIndex; i++)
            {
                Vector2 start = points[i];
                Vector2 end = points[i + 1];
                DrawLine(spriteBatch, pixel, start, end, color, thickness);
            }

            // Close loop only if requested and not already closed
            if (closeLoop && !alreadyClosed)
            {
                DrawLine(spriteBatch, pixel, points[lastIndex], points[0], color, thickness);
            }
        }
    }
}
