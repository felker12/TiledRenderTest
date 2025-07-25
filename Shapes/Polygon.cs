using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiledRenderTest.Shapes
{
    public class Polygon : Shape
    {
        public Vector2[] Points { get; set; } =[];
        public Line[] Sides => ToLines(Points, Color);
        public VertexPositionColor[] Vertices => [.. Points.Select(point => ToVertexPositionColor(point, Color))];

        protected Polygon(Vector2[] vertices, Color color) : base(color)
        {
            Points = vertices;
        }

        protected Polygon(Vector2[] vertices) : base()
        {
            Points = vertices;
        }

        protected Polygon(Vector2 position, Color color) : base(position, color)
        {
        }

        protected Polygon(Vector2 position) : base(position)
        {
        }

        public Polygon() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            DrawOutline(spriteBatch, outlineThickness);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            DrawOutline(spriteBatch, Color, outlineThickness);
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, Color outlineColor, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Color = outlineColor;
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, int outlineThickness = 1)
        {
            foreach (var side in Sides)
            {
                side.Thickness = outlineThickness;
                side.Draw(spriteBatch);
            }
        }

        public override void DrawOutlineUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix)
        {
            foreach (var side in Sides)
            {
                side.DrawUsingPrimitives(graphicsDevice, transformMatrix);
            }
        }

        public override void DrawOutlineUsingPrimitives(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            DrawOutlineUsingPrimitives(spriteBatch.GraphicsDevice, transformMatrix);
        }

        public override void DrawOutlineThickUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, int thickness = 1)
        {
            foreach (var side in Sides)
                side.DrawThickUsingPrimitives(graphicsDevice, transformMatrix, thickness);
        }
    }
}
