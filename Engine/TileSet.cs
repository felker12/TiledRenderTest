using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledRenderTest.Engine
{
    public class TileSet
    {
        public int FirstGid { get; set; } = 1;
        public string Source { get; set; } = string.Empty;
        public int TileWidth { get; set; } = 16;
        public int TileHeight { get; set; } = 16;
        public Texture2D Texture { get; set; }
        public Dictionary<int, AnimatedTile> AnimatedTiles { get; set; } = [];

        public TileSet() { }

        public override string ToString()
        {
            return $"TileSet: FirstGid={FirstGid}, Source={Source}, TileWidth={TileWidth}, TileHeight={TileHeight}, Texture={Texture?.Name ?? "null"}";
        }

        public void LoadAnimatedTiles(string tsxPath)
        {
            var tsxDoc = XDocument.Load(tsxPath);
            foreach (var tileElem in tsxDoc.Descendants("tile"))
            {
                var animationElem = tileElem.Element("animation");
                if (animationElem != null)
                {
                    int tileId = int.Parse(tileElem.Attribute("id").Value);
                    var frames = new List<AnimatedTileFrame>();
                    foreach (var frameElem in animationElem.Elements("frame"))
                    {
                        frames.Add(new AnimatedTileFrame
                        {
                            TileId = int.Parse(frameElem.Attribute("tileid").Value),
                            Duration = int.Parse(frameElem.Attribute("duration").Value)
                        });
                    }

                    AnimatedTile animTile = new(
                        TileWidth,
                        TileHeight,
                        0, // X position (defaulted to 0)
                        0, // Y position (defaulted to 0)
                        tileId,
                        this,
                        frames
                    );

                    AnimatedTiles[tileId] = animTile;
                }
            }
        }
    }
}
