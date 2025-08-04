using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TiledRenderTest.Shapes
{
    public class Triangle : Shape
    {
        public Vector2 Position2 { get; protected set; } = new(50, 50);
        public Vector2 Position3 { get; protected set; } = new(0, 50);

        public Triangle() : base()
        {
            SetTrianglePoints();
        }

        public Triangle(Vector2 position, Vector2 position2, Vector2 position3, Color color) :
            base(position, color)
        {
            Position2 = position2;
            Position3 = position3;

            SetTrianglePoints();
        }

        public Triangle(Vector2 position, Vector2 position2, Vector2 position3) :
            base(position)
        {
            Position2 = position2;
            Position3 = position3;

            SetTrianglePoints();
        }

        private void SetTrianglePoints()
        {
            points = [Position, Position2, Position3, Position];
        }

        public void UpdatePositions(Vector2 position, Vector2 position2, Vector2 position3)
        {
            Position = position;
            Position2 = position2;
            Position3 = position3;
            SetTrianglePoints();
        }

        public override bool Contains(Vector2 point)
        {
            return PointInTriangle(point, Position, Position2, Position3);
        }

        public override void DrawFilledUsingPrimitives(GraphicsDevice graphicsDevice, Matrix viewMatrix)
        {
            basicEffect = InitializeBasicEffect(graphicsDevice, viewMatrix);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    PerimeterVertices,
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
