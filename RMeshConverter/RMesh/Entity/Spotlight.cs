using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class Spotlight : Light
{
    public string Angles { get; set; }
    
    public int InnerConeAngle { get; set; }
    public int OuterConeAngle { get; set; }

    public Spotlight(Vector3 position, float range, string color, float intensity, string angles, int innerConeAngle, int outerConeAngle) : base(position, range, color, intensity)
    {
        Angles = angles;
        InnerConeAngle = innerConeAngle;
        OuterConeAngle = outerConeAngle;
    }
}