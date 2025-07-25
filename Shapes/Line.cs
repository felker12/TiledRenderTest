using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Line : Shape
    {
        public Vector2 Position2 { get; set; } = Vector2.Zero;
        public Vector2 Distance => Position2 - Position;
        public int Thickness { get; set; } = 1;
        public Vector2[] Points => [Position, Position2];
        public VertexPositionColor[] Vertices => [.. Points.Select(point => ToVertexPositionColor(point, Color))];

        public Line() : base()
        {
        }

        public Line(Vector2 position, Vector2 position2)
            : base(position)
        {
            Position2 = position2;
        }

        public Line(Vector2 position, Vector2 position2, Color color)
            : base(position, color)
        {
            Position2 = position2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 distance = Distance;
            float angle = (float)Math.Atan2(distance.Y, distance.X); // Calculate angle in radians
            float length = distance.Length();
            spriteBatch.Draw(Texture, Position, null, Color, angle, Vector2.Zero, new Vector2(length, Thickness), SpriteEffects.None, 0f);
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
    }
}
