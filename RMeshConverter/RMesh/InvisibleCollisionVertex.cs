using System.Numerics;

namespace RMeshConverter.RMesh;

public class InvisibleCollisionVertex
{
    
    public Vector3 Position { get; set; }
    
    public InvisibleCollisionVertex(Vector3 position)
    {
        Position = position;
    }
}