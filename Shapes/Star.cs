using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Star : Polygon
    {
        public Star(Vector2 center, int numbOfPoints = 5) : base(center)
        {

            float outerRadius = 100;
            float innerRadius = 40;

            Points = CreateStarOutline(center, outerRadius, innerRadius, numbOfPoints);
        }

        public Star(Vector2 center, Color color, int numbOfPoints = 5) : this(center, numbOfPoints)
        {
            Color = color;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            base.Draw(spriteBatch, outlineThickness);
        }

        public override void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            base.DrawOutline(spriteBatch, outlineThickness);
        }

        public void DrawUsingPrimitives(GraphicsDevice graphicsDevice, int outlineThickness = 1, int w = 100, int h = 100)
        {
            var starVertices = CreateFilledStarVertices(new Vector2(200, 200), 100f, 40f, Color.Gold);

            BasicEffect effect = new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity,
                Projection = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 0, 1)
            };

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, starVertices, 0, starVertices.Length / 3);
            }
        }

        public static Vector2[] CreateStarOutline(Vector2 center, float outerRadius, float innerRadius, int numPoints = 5)
        {
            var points = new Vector2[numPoints * 2 + 1];
            float angleStep = MathF.PI / numPoints; // 180° / numPoints, half angle step for outer/inner
            float currentAngle = -MathF.PI / 2f;    // start at top (-90 degrees)

            for (int i = 0; i < points.Length; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new Vector2(
                    center.X + MathF.Cos(currentAngle) * radius,
                    center.Y + MathF.Sin(currentAngle) * radius
                );
                currentAngle += angleStep;
            }

            // Close the shape
            points[^1] = points[0]; //the last point should be the same as the first

            return points;
        }

        public static VertexPositionColor[] CreateFilledStarVertices(Vector2 center, float outerRadius, float innerRadius, Color color, int numPoints = 5)
        {
            List<VertexPositionColor> vertices = [];

            float angleStep = MathF.PI / numPoints;
            float currentAngle = -MathF.PI / 2f; // Start at top

            Vector2[] points = new Vector2[numPoints * 2];
            for (int i = 0; i < points.Length; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new Vector2(
                    center.X + MathF.Cos(currentAngle) * radius,
                    center.Y + MathF.Sin(currentAngle) * radius
                );
                currentAngle += angleStep;
            }

            // Build triangles from center to each edge pair (triangle fan style)
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % points.Length];

                vertices.Add(new VertexPositionColor(new Vector3(center, 0), color));
                vertices.Add(new VertexPositionColor(new Vector3(p1, 0), color));
                vertices.Add(new VertexPositionColor(new Vector3(p2, 0), color));
            }

            return [.. vertices];
        }
    }
}
