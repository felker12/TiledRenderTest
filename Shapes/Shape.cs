using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Shape
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White; // Default color
        public Texture2D Texture => Game1.CreateTextureFromColor(Color);

        public Shape(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public Shape(Vector2 position)
        {
            Position = position;
        }

        public Shape(Color color)
        {
            Color = color;
        }

        public Shape()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Default draw logic, can be overridden by derived classes
            // This method should be implemented in derived classes to provide specific drawing behavior
            throw new NotImplementedException("Draw method must be implemented in derived classes.");
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch)
        {
            // Default outline draw logic, can be overridden by derived classes
            // This method should be implemented in derived classes to provide specific outline drawing behavior
            throw new NotImplementedException("DrawOutline method must be implemented in derived classes.");
        }

        public virtual void Update(GameTime gameTime)
        {
            // Default update logic, can be overridden by derived classes
            // This method can be used to update the shape's state, position, etc.
            throw new NotImplementedException("Update method must be implemented in derived classes.");
        }

        public static Vector3 ToVector3(Vector2 vector)
        {
            return new(vector, 0);
        }

        public static VertexPositionColor ToVertexPositionColor(Vector2 vector, Color? color)
        {
            return new VertexPositionColor(ToVector3(vector), color ?? Color.White);
        }

        public static Line[] ToLines(Vector2[] vectors, Color color)
        {
            return [.. Enumerable.Range(0, vectors.Length - 1).Select(i => new Line(vectors[i], vectors[i + 1], color))];
        }

        public static Line[] ToLines(List<Vector2> vectors, Color color)
        {
            return ToLines(vectors.ToArray(), color);
        }

        public static VertexPositionColor[] GetThickLineVertices(Vector2 a, Vector2 b, float thickness, Color color)
        {
            Vector2 direction = b - a;
            Vector2 normal = Vector2.Normalize(new Vector2(-direction.Y, direction.X)); // Perpendicular

            Vector2 offset = normal * (thickness / 2f);

            // Four corners of the quad
            Vector2 a1 = a + offset;
            Vector2 a2 = a - offset;
            Vector2 b1 = b + offset;
            Vector2 b2 = b - offset;

            return
            [
                // Triangle 1 (clockwise)
                new VertexPositionColor(new Vector3(a1, 0), color),
                new VertexPositionColor(new Vector3(a2, 0), color),
                new VertexPositionColor(new Vector3(b1, 0), color),

                // Triangle 2 (clockwise)
                new VertexPositionColor(new Vector3(b1, 0), color),
                new VertexPositionColor(new Vector3(a2, 0), color),
                new VertexPositionColor(new Vector3(b2, 0), color),
            ];
        }
    }
}
