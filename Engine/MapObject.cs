using System.Collections.Generic;
using System.Linq;

namespace TiledRenderTest.Engine
{
    public class MapObject()
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public List<MapObjectProperties> Properties { get; set; } = [];

        public override string ToString()
        {
            return $"Id, Name, X, Y, Width, Height, {string.Join(", ", Properties.Select(t => t.ToString()))}";
        }
    }
}
