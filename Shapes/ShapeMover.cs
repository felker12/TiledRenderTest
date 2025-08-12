using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata;

namespace TiledRenderTest.Shapes
{
    public static class ShapeMover
    {
        private static readonly Random random = new();

        // Static default
        public const float DefaultSpeed = 150f;
        public static Vector2 DefaultVelocity => new(DefaultSpeed, DefaultSpeed);

        


        //TODO add logic to move the shape around an area






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