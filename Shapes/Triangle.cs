using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Triangle : Shape
    {
        Vector2 Position2 { get; set; } = new(50, 50);
        Vector2 Position3 { get; set; } = new(0, 50);
        Vector2[] Points => [Position, Position2, Position3, Position];
        public Line[] Sides => ToLines(Points, Color);
        public VertexPositionColor[] Vertices => [.. Points.Select(point => ToVertexPositionColor(point, Color))];
        public VertexPositionColor[] FilledVertices; //TODO

        public Triangle() : base() { }

        public Triangle(Vector2 position, Vector2 position2, Vector2 position3, Color color) :
            base(position, color)
        {
            Position2 = position2;
            Position3 = position3;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //TODO isn't implemented
            base.Draw(spriteBatch);
        }

        public void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Color = outlineColor;
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public override void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
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

        public override void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public override void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 2)
        {
            foreach (var side in Sides)
                side.DrawThickUsingPrimitives(graphicsDevice, transformMatrix, thickness);
        }

        public override void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            BasicEffect basicEffect = new(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = viewMatrix,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0, graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height, 0,
                    0, 1)
            };

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    Vertices,
                    0,
                    1 // one triangle = 1 primitive
                );
            }
        }
    }
}
