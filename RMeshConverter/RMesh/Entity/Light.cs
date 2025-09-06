using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class Light : Entity
{
    public float Range { get; set; }
    public string Color { get; set; }
    public float Intensity { get; set; }
    
    public Light(Vector3 position, float range, string color, float intensity) : base(position)
    {
        Range = range;
        Color = color;
        Intensity = intensity;
    }
}