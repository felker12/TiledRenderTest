using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;

namespace TiledRenderTest.Engine
{
    internal class TileMap(ContentManager content)
    {
        public List<Layer> Layers { get; set; } = [];
        public List<TileSet> TileSets { get; set; } = [];
        private ContentManager Content { get; set; } = content;
        public int LongestLayerWidthInTiles { get; set; } = 0; //TODO
        public int LongestLayerHeightInTiles { get; set; } = 0;

        public void Update(GameTime gameTime)
        {
            foreach (var layer in Layers)
            {
                layer.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in Layers)
            {
                layer.Draw(spriteBatch);
            }
        }

        public Layer GetLayerByName(string name)
        {
            return Layers.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return $"TileMap with {Layers.Count} layers and {TileSets.Count} tile sets.\n" +
                $"Layers: {string.Join(", ", Layers.Select(t => t.Name))}\n" +
                $"TileSets: {string.Join(", ", TileSets.Select(t => t.Source))}";
        }

        public void LoadFromTmx(string tmxPath)
        {
            XDocument doc = XDocument.Load(tmxPath);
            var map = doc.Element("map");
            int tileWidth = int.Parse(map.Attribute("tilewidth").Value);
            int tileHeight = int.Parse(map.Attribute("tileheight").Value);

            // Load tilesets first as they're needed for tile layers
            foreach (var tilesetElem in map.Elements("tileset"))
            {
                int firstGid = int.Parse(tilesetElem.Attribute("firstgid").Value);

                // Handle external TSX reference
                string tsxSource = tilesetElem.Attribute("source")?.Value;
                string imageSource;
                Texture2D texture;

                if (!string.IsNullOrEmpty(tsxSource))
                {
                    // Load TSX file to get image source
                    var tsxPath = Path.Combine(Path.GetDirectoryName(tmxPath) ?? "", tsxSource);
                    var tsxDoc = XDocument.Load(tsxPath);
                    var tsxTileset = tsxDoc.Element("tileset");
                    imageSource = tsxTileset.Element("image").Attribute("source").Value;
                    texture = Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(imageSource));
                }
                else
                {
                    imageSource = tilesetElem.Element("image").Attribute("source").Value;
                    texture = Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(imageSource));
                }

                var tileSet = new TileSet
                {
                    FirstGid = firstGid,
                    TileWidth = tileWidth,
                    TileHeight = tileHeight,
                    Texture = texture,
                    Source = imageSource
                };

                // If using external TSX, load animated tiles
                if (!string.IsNullOrEmpty(tsxSource))
                {
                    var tsxPath = Path.Combine(Path.GetDirectoryName(tmxPath) ?? "", tsxSource);
                    tileSet.LoadAnimatedTiles(tsxPath);
                }

                TileSets.Add(tileSet);
            }

            // Load all layers in order they appear in the TMX file
            foreach (var layerElement in map.Elements().Where(e => e.Name == "layer" || e.Name == "objectgroup"))
            {
                if (layerElement.Name == "layer")
                {
                    // Handle tile layer
                    int width = int.Parse(layerElement.Attribute("width").Value);
                    int height = int.Parse(layerElement.Attribute("height").Value);
                    var data = layerElement.Element("data").Value.Trim();
                    var tileIds = data.Split(',').Select(id => int.Parse(id.Trim())).ToArray();

                    TileLayer tileLayer = new()
                    {
                        Width = width,
                        Height = height,
                        TileWidth = tileWidth,
                        TileHeight = tileHeight,
                        ID = int.Parse(layerElement.Attribute("id").Value),
                        Name = layerElement.Attribute("name")?.Value ?? "Layer" + layerElement.Attribute("id").Value,
                    };

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * width + x;
                            int tileId = tileIds[index];
                            if (tileId > 0)
                            {
                                TileSet tileSet = TileSets.Last(ts => tileId >= ts.FirstGid);
                                Tile tile;
                                int localTileId = tileId - tileSet.FirstGid;
                                if (tileSet.AnimatedTiles.TryGetValue(localTileId, out var animTemplate))
                                {
                                    tile = new AnimatedTile(x * tileWidth, y * tileHeight, tileWidth, tileHeight, tileId, tileSet, animTemplate.Frames);
                                }
                                else
                                {
                                    tile = new Tile(x * tileWidth, y * tileHeight, tileWidth, tileHeight, tileId, tileSet);
                                }
                                tileLayer.Tiles.Add(tile);
                            }
                        }
                    }

                    Layers.Add(tileLayer);
                    //Debug.WriteLine($"Loaded layer: {tileLayer.Name} (ID: {tileLayer.ID}) with {tileLayer.Tiles.Count} tiles.");
                }
                else if (layerElement.Name == "objectgroup")
                {
                    // Handle object layer
                    ObjectLayer objectLayer = new()
                    {
                        ID = (int?)layerElement.Attribute("id") ?? 0,
                        Name = (string)layerElement.Attribute("name") ?? string.Empty,
                        Width = (int?)layerElement.Attribute("width") ?? 0,
                        Height = (int?)layerElement.Attribute("height") ?? 0,
                    };

                    foreach (var obj in layerElement.Elements("object"))
                    {
                        MapObject mapObject = new()
                        {
                            ID = (int?)obj.Attribute("id") ?? 0,
                            Name = (string)obj.Attribute("name") ?? string.Empty,
                            X = (float?)obj.Attribute("x") ?? 0,
                            Y = (float?)obj.Attribute("y") ?? 0,
                            Width = (float?)obj.Attribute("width") ?? 0,
                            Height = (float?)obj.Attribute("height") ?? 0,
                        };

                        var propertiesElement = obj.Element("properties");
                        if (propertiesElement != null)
                        {
                            foreach (var prop in propertiesElement.Elements("property"))
                            {
                                mapObject.Properties.Add(new MapObjectProperties
                                {
                                    Name = (string)prop.Attribute("name") ?? string.Empty,
                                    Value = (string)prop.Attribute("value") ?? string.Empty
                                });
                            }
                        }

                        objectLayer.MapObjects.Add(mapObject);
                    }

                    Layers.Add(objectLayer);
                    //Debug.WriteLine($"Loaded object layer: {objectLayer.Name} (ID: {objectLayer.ID}) with {objectLayer.MapObjects.Count} objects.");
                }
            }
        }
    }
}
