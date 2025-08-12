using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using TiledRenderTest.Engine.ShapeOutlines;
using System.Diagnostics;

namespace TiledRenderTest.Engine
{
    internal class TileMap
    {
        public List<Layer> Layers { get; private set; } = [];
        public List<TileSet> TileSets { get; private set; } = [];
        private ContentManager Content { get; set; }
        public float LongestLayerWidthInTiles { get; set; } = 0;
        public float LongestLayerWidthInPixels { get; set; } = 0;
        public float LongestLayerHeightInTiles { get; set; } = 0;
        public float LongestLayerHeightInPixels { get; set; } = 0;

        public TileMap(ContentManager content, string tmxPath, GraphicsDevice graphicsDevice)
        {
            Content = content; 
            XDoc xDoc = new(tmxPath);

            // Load tilesets first as they're needed for tile layers
            TileSets = TmxReader.LoadTileSetsFromTmx(xDoc, tmxPath, content);
            Layers = TmxReader.LoadLayersFromTmx(xDoc, TileSets, graphicsDevice);

            CalculateLongestLayerDimensions();
        }

        public TileMap(List<Layer> layers, List<TileSet> tileSets, ContentManager content)
        {
            Layers = layers;
            TileSets = tileSets;
            Content = content;

            CalculateLongestLayerDimensions();
        }

        public TileMap(TileMap tileMap)
        {
            Layers = [.. tileMap.Layers];
            TileSets = [.. tileMap.TileSets];
            Content = tileMap.Content;
            LongestLayerWidthInTiles = tileMap.LongestLayerWidthInTiles;
            LongestLayerHeightInTiles = tileMap.LongestLayerHeightInTiles;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var layer in Layers)
            {
                // Only update layers that are visible
                if (!layer.Visible)
                    continue;

                layer.Update(gameTime);

                if(layer is ObjectLayer objectLayer)
                {
                    bool insideAnyObject = false;

                    foreach (var mapObject in objectLayer.MapObjects)
                    {
                        if (OutlineCollisionDetection.IsPointInsideOutlineDetailed(Game1.Player.ShapeRectangle.Points, mapObject.PolygonPoints))
                        {
                            insideAnyObject = true;
                            break; // Exit early - no need to check remaining objects
                        }
                    }

                    // Set color once after checking all objects
                    Game1.Player.Color = insideAnyObject ? Color.Red : Color.Blue;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in Layers)
            {
                // Only draw layers that are visible
                if (!layer.Visible)
                    continue;

                layer.Draw(spriteBatch);
            }
        }

        public Layer GetLayerByName(string name)
        {
            return Layers.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void InsertLayerAt(int index, Layer layer)
        {
            if (index < 0)
            {
                index = 0; // Insert at the beginning if index is out of range
            }
            else if(index > Layers.Count)
            {
                index = Layers.Count; // Append to the end if index is out of range
            }

            Layers.Insert(index, layer);

            CalculateLongestLayerDimensions();
        }

        public void CalculateLongestLayerDimensions()
        {
            float longestWidth = 0;
            float longestHeight = 0;
            float longestWidthInPixels = 0;
            float longestHeightInPixels = 0;

            foreach (var layer in Layers)
            {
                if (layer is TileLayer tileLayer)
                {
                    if (tileLayer.WidthInTiles > longestWidth)
                    {
                        longestWidth = tileLayer.WidthInTiles;
                        longestWidthInPixels = tileLayer.WidthInTiles * tileLayer.TileWidth;
                    }
                    if (tileLayer.HeightInTiles > longestHeight)
                    {
                        longestHeight = tileLayer.HeightInTiles;
                        longestHeightInPixels = tileLayer.HeightInTiles * tileLayer.TileHeight;
                    }
                }
            }

            LongestLayerWidthInTiles = longestWidth;
            LongestLayerHeightInTiles = longestHeight;
            LongestLayerWidthInPixels = longestWidthInPixels;
            LongestLayerHeightInPixels = longestHeightInPixels;
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer);
        }

        public override string ToString()
        {
            return $"TileMap with {Layers.Count} layers and {TileSets.Count} tile sets.\n" +
                $"Layers: {string.Join(", ", Layers.Select(t => t.Name))}\n" +
                $"TileSets: {string.Join(", ", TileSets.Select(t => t.Source))}";
        }
    }
}
