using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TiledRenderTest.Engine
{
    /// <summary>
    /// Represents a single layer of tiles in the map.
    /// Handles both static and animated tiles, updating their state and drawing them efficiently.
    /// </summary>
    public class TileLayer : Layer
    {
        // The width and height of each tile in pixels
        public int TileHeight { get; set; } = 16;
        public int TileWidth { get; set; } = 16;
        public int WidthInTiles => Width;
        public int HeightInTiles => Height;

        // The list of all tiles in this layer (can include both static and animated tiles)
        public List<Tile> Tiles { get; set; } = [];

        public TileLayer() : base() { }

        /// <summary>
        /// Updates the animation state of all animated tiles in this layer.
        /// Called once per frame by the game loop.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (var tile in Tiles)
            {
                // Only animated tiles need to update their animation state
                if (tile is AnimatedTile animatedTile && animatedTile.Frames.Count > 0)
                {
                    animatedTile.AnimationElapsed += delta;
                    var currentFrame = animatedTile.Frames[animatedTile.CurrentFrameIndex];

                    // Advance to the next frame if enough time has passed
                    if (animatedTile.AnimationElapsed >= currentFrame.Duration)
                    {
                        animatedTile.AnimationElapsed -= currentFrame.Duration;
                        animatedTile.CurrentFrameIndex = (animatedTile.CurrentFrameIndex + 1) % animatedTile.Frames.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Draws all tiles in this layer.
        /// Tiles are grouped by their TileSet to minimize texture switches and maximize SpriteBatch batching efficiency.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Group tiles by their TileSet to reduce texture switching and improve batching
            foreach (var group in Tiles.GroupBy(t => t.TileSet))
            {
                var tileSet = group.Key;
                int tilesPerRow = tileSet.Texture.Width / tileSet.TileWidth;

                foreach (var tile in group)
                {
                    int drawTileId = tile.LocalTileId;

                    // If the tile is animated, use the current animation frame's tile ID
                    if (tile is AnimatedTile animatedTile && animatedTile.Frames.Count > 0)
                    {
                        drawTileId = animatedTile.Frames[animatedTile.CurrentFrameIndex].TileId;
                    }

                    // Calculate the source rectangle in the tileset texture
                    int sx = (drawTileId % tilesPerRow) * tileSet.TileWidth;
                    int sy = (drawTileId / tilesPerRow) * tileSet.TileHeight;
                    Rectangle sourceRect = new(sx, sy, tileSet.TileWidth, tileSet.TileHeight);

                    // Draw the tile at its world position
                    spriteBatch.Draw(
                        tileSet.Texture,
                        new Rectangle(tile.X, tile.Y, tileSet.TileWidth, tileSet.TileHeight),
                        sourceRect,
                        Color.White
                    );
                }
            }
        }

        public override string ToString()
        {
            return $"TileLayer ID: {ID}, Name: {Name}, Width: {Width}, Height: {Height}, Tiles Count: {Tiles.Count}";
        }

        /// <summary>
        /// Returns the first tile at the given (x, y) pixel position, or null if none.
        /// </summary>
        public Tile GetTileAt(int x, int y)
        {
            return Tiles.FirstOrDefault(tile => tile.Contains(new Vector2(x, y)));
        }

        public void GetTileAt(int x, int y, out Tile tile)
        {
            tile = GetTileAt(x, y);
        }

        public Tile GetTileAt(Vector2 position)
        {
            return Tiles.FirstOrDefault(tile => tile.Contains(position));
        }

        public void GetTileAt(Vector2 position, out Tile tile)
        {
            tile = GetTileAt(position);
        }

        public Tile GetTileAt(Point point)
        {
            return Tiles.FirstOrDefault(tile => tile.Contains(point));
        }

        public void GetTileAt(Point point, out Tile tile)
        {
            tile = GetTileAt(point);
        }
    }
}
