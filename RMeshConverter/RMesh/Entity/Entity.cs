using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class Entity
{
    public Vector3 Position { get; set; }

    public Entity(Vector3 position)
    {
        Position = position;
    }
}