using Microsoft.Xna.Framework;

namespace TiledRenderTest.Engine.ShapeOutlines
{
    public static class OutlineCollisionDetection
    {
        public static bool IsPointInsideOutline(Vector2 point, Vector2[] outline)
        {
            if (outline == null || outline.Length < 3)
                return false; // Not a valid polygon
            bool inside = false;
            int j = outline.Length - 1;
            for (int i = 0; i < outline.Length; i++)
            {
                if ((outline[i].Y > point.Y) != (outline[j].Y > point.Y) &&
                    (point.X < (outline[j].X - outline[i].X) * (point.Y - outline[i].Y) / (outline[j].Y - outline[i].Y) + outline[i].X))
                {
                    inside = !inside;
                }
                j = i;
            }
            return inside;
        }

        public static bool IsPointInsideOutline(Vector2[] shape, Vector2[] outline)
        {
            if (outline == null || outline.Length < 3)
                return false; // Not a valid polygon

            for (int i = 0; i < shape.Length; i++)
            {
                if(IsPointInsideOutline(shape[i], outline))
                {
                    return true;
                }
            }

            return false;
        }

        //adds steps between the outline points for more precise collision detection
        public static bool IsPointInsideOutlineDetailed(Vector2[] shape, Vector2[] outline, int interpolationSteps = 5)
        {
            if (outline == null || outline.Length < 3 || shape == null || shape.Length < 2)
                return false; // Not valid polygons

            // Calculate and check the centroid (center) of the shape
            Vector2 center = CalculatePolygonCentroid(shape);
            if (IsPointInsideOutline(center, outline))
            {
                return true;
            }

            // Check each original vertex
            for (int i = 0; i < shape.Length; i++)
            {
                if (IsPointInsideOutline(shape[i], outline))
                {
                    return true;
                }
            }

            // Check interpolated points between each pair of vertices
            for (int i = 0; i < shape.Length; i++)
            {
                Vector2 start = shape[i];
                Vector2 end = shape[(i + 1) % shape.Length]; // Wrap around to first point

                // Add interpolated points along this edge
                for (int step = 1; step <= interpolationSteps; step++)
                {
                    float t = (float)step / (interpolationSteps + 1); // t from 0 to 1 (exclusive)
                    Vector2 interpolatedPoint = Vector2.Lerp(start, end, t);

                    if (IsPointInsideOutline(interpolatedPoint, outline))
                    {
                        return true;
                    }
                }
            }

            return false; // No points (original or interpolated) were inside
        }

        public static Vector2 CalculatePolygonCentroid(Vector2[] vertices)
        {
            Vector2 centroid = Vector2.Zero;

            // Simple average of all vertices (works well for convex polygons)
            foreach (Vector2 vertex in vertices)
            {
                centroid += vertex;
            }

            return centroid / vertices.Length;
        }

        // Alternative: More accurate centroid calculation for complex polygons
        public static Vector2 CalculatePolygonCentroidAccurate(Vector2[] vertices)
        {
            Vector2 centroid = Vector2.Zero;
            float area = 0f;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 current = vertices[i];
                Vector2 next = vertices[(i + 1) % vertices.Length];

                float cross = current.X * next.Y - next.X * current.Y;
                area += cross;
                centroid.X += (current.X + next.X) * cross;
                centroid.Y += (current.Y + next.Y) * cross;
            }

            area *= 0.5f;
            centroid *= 1.0f / (6.0f * area);

            return centroid;
        }
    }
}
