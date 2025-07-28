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
        public Vector2 Position2 { get; set; } = new(50, 50);
        public Vector2 Position3 { get; set; } = new(0, 50);
        public override Vector2[] Points => [Position, Position2, Position3, Position];

        public Triangle() : base() { }

        public Triangle(Vector2 position, Vector2 position2, Vector2 position3, Color color) :
            base(position, color)
        {
            Position2 = position2;
            Position3 = position3;
        }

        public Triangle(Vector2 position, Vector2 position2, Vector2 position3) :
            base(position)
        {
            Position2 = position2;
            Position3 = position3;
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

        public override string ToString()
        {
            return $"Position1: {Position}, Position2: {Position2}, Position3: {Position3}";
        }
    }
}
