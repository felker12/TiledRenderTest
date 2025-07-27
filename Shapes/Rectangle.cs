using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace TiledRenderTest.Shapes
{
    public class Rectangle : Shape
    {
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;

        public Vector2[] Points =>
        [
            Position,
            new (Position.X + Width, Position.Y),
            new (Position.X + Width, Position.Y + Height),
            new (Position.X, Position.Y + Height),
            Position
        ];

        public Line[] Sides => ToLines(Points, Color);
        public VertexPositionColor[] Vertices => [.. Points.Select(corner => ToVertexPositionColor(corner, Color))];

        public Rectangle(int x, int y, int width, int height) : base(new Vector2(x, y))
        {
            Width = width;
            Height = height;
        }

        public Rectangle(int x, int y, int width, int height, Color color)
            : base(new(x,y), color)
        {
            Width = width;
            Height = height;
        }

        public Rectangle(Vector2 position, int width, int height) : base(position)
        {
            Width = width;
            Height = height;
        }

        public Rectangle(Vector2 position, int width, int height, Color color)
            : base(position, color)
        {
            Width = width;
            Height = height;
        }

        public Rectangle() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color);
        }

        public void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Draw(spriteBatch, outlineColor, outlineThickness);
            }
        }

        public void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch) //this stays fixed on the screen. If the camera moves this follows
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            BasicEffect basicEffect = new(spriteBatch.GraphicsDevice)
            {
                World = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1)
            };

            EffectTechnique effectTechnique = basicEffect.Techniques[0];
            EffectPassCollection effectPassCollection = effectTechnique.Passes;
            foreach (EffectPass pass in effectPassCollection)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Vertices, 0, Vertices.Length - 1);
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
    }
}
