using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Buffers;

namespace TiledRenderTest.Shapes
{
    public class Line : IDisposable
    {
        Vector2 distance;
        Vector2 normalizedDirection, normal;
        bool isDirty = true, isDirty2 = true;
        VertexPositionColor[] vertices, thickVertices;
        float angle, length;
        private int thickVertexCount;
        private int lastThickness = 2; 
        private bool disposed = false;

        // Optional: a pool size of 6 is fixed, but scalable if you ever support caps/joins
        private static readonly int ThickVertexPoolSize = 6;

        public Vector2 Position { get;  private set; } = Vector2.Zero;
        public Vector2 Position2 { get; private set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White; // Default color
        public VertexPositionColor[] Vertices { get { RebuildIfDirty(); return vertices; } }
        public VertexPositionColor[] ThickVertices { get { RebuildThickVertices(); return thickVertices; } }
        public Vector2 Distance { get { RebuildIfDirty(); return distance; } }
        public float Angle { get { RebuildIfDirty(); return angle; } }
        public float Length { get { RebuildIfDirty(); return length; } }
        public int ThickVertexCount => thickVertexCount;

        protected virtual void RebuildIfDirty()
        {
            if (isDirty is false)
                return;

            distance = Position2 - Position; 
            angle = (float)Math.Atan2(distance.Y, distance.X);
            length = distance.Length();

            vertices = [
                new VertexPositionColor(new Vector3(Position, 0f), Color),
                new VertexPositionColor(new Vector3(Position2, 0f), Color)];


            isDirty = false;
        }

        public void RebuildThickVertices(int thickness = 2)
        {
            if (!isDirty2 && thickVertices != null && thickness == lastThickness)
                return;

            distance = Position2 - Position;

            if (distance != Vector2.Zero)
            {
                normalizedDirection = Vector2.Normalize(distance);
                normal = new Vector2(-normalizedDirection.Y, normalizedDirection.X);
            }

            lastThickness = thickness;
            isDirty2 = false;

            // Return old array if previously pooled
            if (thickVertices != null)
                ArrayPool<VertexPositionColor>.Shared.Return(thickVertices);

            thickVertices = ArrayPool<VertexPositionColor>.Shared.Rent(ThickVertexPoolSize);
            thickVertexCount = 6;

            Vector2 offset = normal * (thickness / 2f);

            Vector2 a1 = Position + offset;
            Vector2 a2 = Position - offset;
            Vector2 b1 = Position2 + offset;
            Vector2 b2 = Position2 - offset;

            thickVertices[0] = new VertexPositionColor(new Vector3(a1, 0), Color);
            thickVertices[1] = new VertexPositionColor(new Vector3(a2, 0), Color);
            thickVertices[2] = new VertexPositionColor(new Vector3(b1, 0), Color);
            thickVertices[3] = new VertexPositionColor(new Vector3(b1, 0), Color);
            thickVertices[4] = new VertexPositionColor(new Vector3(a2, 0), Color);
            thickVertices[5] = new VertexPositionColor(new Vector3(b2, 0), Color);
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

            //Mark as dirty to rebuild vertices
            isDirty = true; 
            isDirty2 = true;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int thickness = 1)
        {
            spriteBatch.Draw(texture, Position, null, Color, Angle, Vector2.Zero, new Vector2(Length, thickness), SpriteEffects.None, 0f);

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Color color, int thickness = 1)
        {
            spriteBatch.Draw(texture, Position, null, color, Angle, Vector2.Zero, new Vector2(Length, thickness), SpriteEffects.None, 0f);
        }

        public void DrawUsingPrimitives(GraphicsDevice graphicsDevice, Matrix transformMatrix, BasicEffect basicEffect)
        {
            basicEffect ??= Shape.InitializeBasicEffect(graphicsDevice, transformMatrix);

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
            if(isDirty2 || ThickVertices is null)
                RebuildThickVertices(thickness);

            basicEffect ??= Shape.InitializeBasicEffect(graphicsDevice, transformMatrix);

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent finalizer from running, if present
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                if (thickVertices != null)
                {
                    ArrayPool<VertexPositionColor>.Shared.Return(thickVertices, clearArray: true);
                    thickVertices = null;
                }
            }

            // If you ever add unmanaged resources, dispose them here

            disposed = true;
        }
    }
}
