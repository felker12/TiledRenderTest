using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Line
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Position2 { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White; // Default color
        public Texture2D Texture => Game1.CreateTextureFromColor(Color);
        public VertexPositionColor[] Vertices => [
                new(new Vector3(Position, 0f), Color),
                new(new Vector3(Position2, 0f), Color)];
        public Vector2 Distance => Position2 - Position;
        public int Thickness { get; set; } = 1;

        public Line() 
        {
        }

        public Line(Vector2 position, Vector2 position2)
        {
            Position = position;
            Position2 = position2;
        }

        public Line(Vector2 position, Vector2 position2, Color color) 
        {
            Position = position;
            Position2 = position2;
            Color = color;
        }

        public Line(float x, float y, float x2, float y2, Color color)
        {
            Position = new Vector2(x, y);
            Position2 = new Vector2(x2, y2);
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 distance = Distance;
            float angle = (float)Math.Atan2(distance.Y, distance.X); // Calculate angle in radians
            float length = distance.Length();
            spriteBatch.Draw(Texture, Position, null, Color, angle, Vector2.Zero, new Vector2(length, Thickness), SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Color color, int thickness)
        {
            Vector2 distance = Distance;
            float angle = (float)Math.Atan2(distance.Y, distance.X); // Calculate angle in radians
            float length = distance.Length();
            spriteBatch.Draw(Texture, Position, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public void DrawUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            BasicEffect basicEffect = new(graphicsDevice)
            {
                VertexColorEnabled = true,
                View = transformMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };

            EffectTechnique effectTechnique = basicEffect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Vertices, 0, Vertices.Length - 1);
            }
        }

        public void DrawUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public void DrawThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness)
        {
            var verts = GetThickLineVertices(Position, Position2, thickness, Color);

            BasicEffect basicEffect = new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = transformMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height, 0,
                    0, 1)
            };

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, 2); // 2 triangles = 1 quad
            }
        }

        public void DrawThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            DrawThickUsingPrimitives(graphicsDevice, transformMatrix, Thickness);
        }

        public override string ToString()
        {
            return $"Position: {Position}, Position2 {Position2}, Distance: {Distance}, Thickness: {Thickness}";
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
