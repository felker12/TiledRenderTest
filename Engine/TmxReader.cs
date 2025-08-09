using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TiledRenderTest.Engine
{
    public readonly struct XDoc
    {
        public readonly XDocument Doc { get; private init; }
        public readonly XElement Map { get; private init; }
        public readonly int TileWidth { get; private init; }
        public readonly int TileHeight { get; private init; }

        public XDoc(string tmxPath) 
        {
            this.Doc = XDocument.Load(tmxPath);
            this.Map = Doc.Element("map") ?? throw new InvalidOperationException("TMX file must have a <map> element.");
            this.TileWidth = int.Parse(Map.Attribute("tilewidth").Value) ;
            this.TileHeight = int.Parse(Map.Attribute("tileheight").Value);
        }
    }

    internal class TmxReader
    {
        public static TileMap LoadMapFromTmx(string tmxPath, ContentManager content)
        {
            XDoc xDoc = new(tmxPath);

            // Load tilesets first as they're needed for tile layers
            List<TileSet> tileSets = LoadTileSetsFromTmx(xDoc, tmxPath, content);
            List<Layer> layers = LoadLayersFromTmx(xDoc, tileSets);

            return new(layers, tileSets, content);
        }

        public static List<TileSet> LoadTileSetsFromTmx(XDoc xDoc, string tmxPath, ContentManager content)
        {
            List<TileSet> tileSets = [];
            var map = xDoc.Map;
            int tileWidth = xDoc.TileWidth;
            int tileHeight = xDoc.TileHeight;

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
                    texture = content.Load<Texture2D>(Path.GetFileNameWithoutExtension(imageSource));
                }
                else
                {
                    imageSource = tilesetElem.Element("image").Attribute("source").Value;
                    texture = content.Load<Texture2D>(Path.GetFileNameWithoutExtension(imageSource));
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

                tileSets.Add(tileSet);
            }

            return tileSets;
        }

        public static List<Layer> LoadLayersFromTmx(XDoc xDoc, List<TileSet> tileSets)
        {
            List<Layer> layers = [];
            var map = xDoc.Map;
            int tileWidth = xDoc.TileWidth;
            int tileHeight = xDoc.TileHeight; 

            // Load all layers in order they appear in the TMX file
            foreach (var layerElement in map.Elements().Where(e => e.Name == "layer" || e.Name == "objectgroup"))
            {
                bool layerVisible = true; // default
                var visibleAttr = layerElement.Attribute("visible");
                if (visibleAttr != null)
                    layerVisible = ((int)visibleAttr) != 0; //if visible = 0 then the object should not be visible

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
                        Visible = layerVisible
                    };

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * width + x;
                            int tileId = tileIds[index];
                            if (tileId > 0)
                            {
                                TileSet tileSet = tileSets.Last(ts => tileId >= ts.FirstGid);
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

                    layers.Add(tileLayer);
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
                        Visible = layerVisible,
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

                        var vis = obj.Attribute("visible");
                        if (vis != null)
                            mapObject.Visible = ((int)vis) != 0; //if visible = 0 then the object should not be visible
                        else
                            mapObject.Visible = true;

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

                        if (obj.Element("ellipse") != null)
                            mapObject.MapObjectShape = MapObjectShape.Ellipse;
                        else if (obj.Element("polygon") != null)
                            mapObject.MapObjectShape = MapObjectShape.Polygon;
                        else
                            mapObject.MapObjectShape = MapObjectShape.Rectangle;

                        var polygonElement = obj.Element("polygon");
                        if (polygonElement != null)
                        {
                            string pointsString = polygonElement.Attribute("points").Value;
                            var points = pointsString
                                .Split(' ') // split by spaces into each "x,y"
                                .Select(pair =>
                                {
                                    var coords = pair.Split(',');
                                    float px = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
                                    float py = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                                    return new Vector2(px, py);
                                })
                                .ToArray();

                            // Store in your MapObject
                            mapObject.PolygonPoints = points;
                        }

                        objectLayer.MapObjects.Add(mapObject);
                    }

                    layers.Add(objectLayer);
                }
            }

            return layers;
        }
    }
}
