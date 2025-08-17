using Microsoft.Xna.Framework;
using System;

namespace TiledRenderTest.Shapes
{
    public static class ShapeMover
    {
        public static void MoveShapeWithinBounds(Shape shape, Vector2 topLeft, Vector2 bottomRight)
        {
            if (shape.Points == null || shape.Points.Length == 0 || shape.CanMove is false || shape.Motion == Vector2.Zero)
                return;

            Vector2 motion = shape.Motion;

            // If shape is out of bounds, bounce
            if (!IsShapeInBounds(shape, topLeft, bottomRight))
            {
                // Undo motion
                shape.SetPosition(shape.Position - motion);

                // Bounce by reversing velocity component(s)
                ShapeBounds bounds = GetShapeBounds(shape);

                bool hitHorizontal = false;
                bool hitVertical = false;

                // Check horizontal bounds
                if (shape.Position.X + bounds.MinX <= topLeft.X)
                {
                    motion.X *= -1;
                    hitHorizontal = true;
                    shape.SetPosition(new Vector2(topLeft.X - bounds.MinX, shape.Position.Y));
                }
                else if (shape.Position.X + bounds.MaxX >= bottomRight.X)
                {
                    motion.X *= -1;
                    hitHorizontal = true;
                    shape.SetPosition(new Vector2(bottomRight.X - bounds.MaxX, shape.Position.Y));
                }

                // Check vertical bounds
                if (shape.Position.Y + bounds.MinY <= topLeft.Y)
                {
                    motion.Y *= -1;
                    hitVertical = true;
                    shape.SetPosition(new Vector2(shape.Position.X, topLeft.Y - bounds.MinY));
                }
                else if (shape.Position.Y + bounds.MaxY >= bottomRight.Y)
                {
                    motion.Y *= -1;
                    hitVertical = true;
                    shape.SetPosition(new Vector2(shape.Position.X, bottomRight.Y - bounds.MaxY));
                }

                // Flip rotation direction if there was any collision
                if (hitHorizontal || hitVertical)
                {
                    shape.RotationSpeedDegreesPerSecond *= -1; 
                }

                shape.Motion = motion;
            }
        }

        // Check if shape is completely within bounds
        public static bool IsShapeInBounds(Shape shape, Vector2 topLeft, Vector2 bottomRight)
        {
            if (shape.Points == null || shape.Points.Length == 0)
                return true; // Empty shape is considered in bounds

            foreach (var point in shape.Points)
            {
                if (point.X < topLeft.X || point.X > bottomRight.X ||
                    point.Y < topLeft.Y || point.Y > bottomRight.Y)
                {
                    return false;
                }
            }

            return true;
        }

        // Helper method to calculate shape bounds relative to its position
        private static ShapeBounds GetShapeBounds(Shape shape)
        {
            if (shape.Points == null || shape.Points.Length == 0)
                return new ShapeBounds { MinX = 0, MaxX = 0, MinY = 0, MaxY = 0, Width = 0, Height = 0 };

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            Vector2 position = shape.Position;

            foreach (var point in shape.Points)
            {
                // Calculate relative position from shape position
                float relativeX = point.X - position.X;
                float relativeY = point.Y - position.Y;

                if (relativeX < minX) minX = relativeX;
                if (relativeX > maxX) maxX = relativeX;
                if (relativeY < minY) minY = relativeY;
                if (relativeY > maxY) maxY = relativeY;
            }

            return new ShapeBounds
            {
                MinX = minX,
                MaxX = maxX,
                MinY = minY,
                MaxY = maxY,
                Width = maxX - minX,
                Height = maxY - minY
            };
        }

        // Helper struct for shape bounds
        private struct ShapeBounds
        {
            public float MinX, MaxX, MinY, MaxY;
            public float Width, Height;
        }
    }
}