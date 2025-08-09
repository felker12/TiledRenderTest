using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TiledRenderTest.Engine
{
    public enum MapObjectShape { Rectangle, Ellipse, Polygon }
    public class MapObject()
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool Visible { get; set; } = true;
        public List<MapObjectProperties> Properties { get; set; } = [];
        public MapObjectShape MapObjectShape { get; set; }
        public Vector2[] PolygonPoints { get; set; } = []; //only used if MapObjectShape is Polygon

        public override string ToString()
        {
            string polygonInfo = string.Empty;

            if (MapObjectShape == MapObjectShape.Polygon && PolygonPoints.Length > 0)
                polygonInfo = ", PolygonPoints: " + string.Join(" ", PolygonPoints.Select(p => $"({p.X},{p.Y})"));

            return $"Id: {ID}, Name: {Name}, X: {X}, Y: {Y}, " +
                $"Width: {Width}, Height: {Height}, Visible: {Visible}, " +
                $"Properties: {string.Join(", ", Properties.Select(t => t.ToString()))}, " +
                $"MapObjectShape: {MapObjectShape}" +
                $"{polygonInfo}";
        }
    }
}
