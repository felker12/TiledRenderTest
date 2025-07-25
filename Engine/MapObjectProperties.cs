
namespace TiledRenderTest.Engine
{
    public class MapObjectProperties()
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Property Name: {Name}, Value: {Value}";
        }
    }
}
