using System.Collections.Generic;

namespace TiledRenderTest.Engine
{
    public class AnimatedTile(
        int x,
        int y,
        int width,
        int height,
        int globalTileId,
        TileSet tileSet,
        List<AnimatedTileFrame> frames
        ) : Tile(x, y, width, height, globalTileId, tileSet)
    {
        public List<AnimatedTileFrame> Frames { get; set; } = frames ?? [];
        public int CurrentFrameIndex { get; set; } = 0;
        public float AnimationElapsed { get; set; } = 0f;
    }
}
