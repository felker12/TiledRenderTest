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
        Vector2 distance;
        bool isDirty = true, isDirty2 = true;
        Texture2D texture;
        VertexPositionColor[] vertices;

        public Vector2 Position { get;  private set; } = Vector2.Zero;
        public Vector2 Position2 { get; private set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White; // Default color
        public Texture2D Texture { get { texture ??= Game1.CreateTextureFromColor(Color); return texture; } }
        public VertexPositionColor[] Vertices { get { RebuildIfDirty(); return vertices; } }
        public VertexPositionColor[] ThickVertices { get; private set; }
        public Vector2 Distance { get { RebuildIfDirty(); return distance; } }

        protected virtual void RebuildIfDirty()
        {
            distance = Position2 - Position;
            vertices = [
                new VertexPositionColor(new Vector3(Position, 0f), Color),
                new VertexPositionColor(new Vector3(Position2, 0f), Color)];

            isDirty = false;
        }

        private void RebuildThickVertices(int thickness)
        {
            ThickVertices = GetThickLineVertices(Position, Position2, thickness, Color);
            isDirty2 = false;
        }

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

        public void SetPosition(Vector2 position1, Vector2 position2)
        {
            Position = position1;
            Position2 = position2;
            isDirty = true; // Mark as dirty to rebuild vertices
            isDirty2 = true;
        }

        public void Draw(SpriteBatch spriteBatch, int thickness = 1)
        {
            Vector2 distance = Distance;
            float angle = (float)Math.Atan2(distance.Y, distance.X); // Calculate angle in radians
            float length = distance.Length();
            spriteBatch.Draw(Texture, Position, null, Color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Color color, int thickness)
        {
            Vector2 distance = Distance;
            float angle = (float)Math.Atan2(distance.Y, distance.X); // Calculate angle in radians
            float length = distance.Length();
            spriteBatch.Draw(Texture, Position, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public void DrawUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, BasicEffect basicEffect)
        {
            basicEffect ??= InitializeBasicEffect(graphicsDevice, transformMatrix);

            EffectTechnique effectTechnique = basicEffect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Vertices, 0, Vertices.Length - 1);
            }
        }

        public void DrawUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix, BasicEffect basicEffect)
        {
            DrawUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix, basicEffect);
        }

        public void DrawThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, BasicEffect basicEffect, int thickness)
        {
            if(isDirty2 = true || ThickVertices is null)
                RebuildThickVertices(thickness);

            basicEffect ??= InitializeBasicEffect(graphicsDevice, transformMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, ThickVertices, 0, 2); // 2 triangles = 1 quad
            }
        }

        public override string ToString()
        {
            return $"Position: {Position}, Position2 {Position2}, Distance: {Distance}";
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

        public static BasicEffect InitializeBasicEffect(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            return new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = viewMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };
        }
    }
}
