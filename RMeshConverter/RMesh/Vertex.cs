using System.Numerics;

namespace RMeshConverter.RMesh;

public class Vertex
{
    public Vector3 Position { get; set; }
    public Vector2 Uv { get; set; }
    public Vector2 LightmapUv { get; set; }
    
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    public Vertex(Vector3 position, Vector2 uv, Vector2 lightmapUv, byte[] rgb)
    {
        if (rgb.Length != 3) throw new ArgumentOutOfRangeException(nameof(rgb), "rgb  must be a length of 3");
        Position = position;
        Uv = uv;
        LightmapUv = lightmapUv;
        Red = rgb[0];
        Green = rgb[1];
        Blue = rgb[2];
    }
}