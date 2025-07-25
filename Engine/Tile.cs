using Microsoft.Xna.Framework;

namespace TiledRenderTest.Engine
{
    public class Tile(int x, int y, int width, int height, int globalTileId, TileSet tileSet)
    {
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public Vector2 Position => new(X, Y);
        public Rectangle Bounds => new(X, Y, Width, Height);
        public int GlobalTileId { get; } = globalTileId;
        public TileSet TileSet { get; } = tileSet;
        public int LocalTileId { get; } = globalTileId - tileSet.FirstGid;

        public bool Contains(Vector2 point) => Bounds.Contains(point);
        public bool Contains(Point point) => Bounds.Contains(point);
        public bool Intersects(Rectangle rectangle) => Bounds.Intersects(rectangle);

        public override string ToString()
        {
            return $"Tile: GlobalTileId={GlobalTileId}, LocalTileId={LocalTileId}, Position=({X}, {Y}), Size=({Width}, {Height}), TileSet={TileSet?.Source ?? "null"}";
        }
    }
}
