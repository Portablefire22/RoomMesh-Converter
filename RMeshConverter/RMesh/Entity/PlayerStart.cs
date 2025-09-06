using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

/// <summary>
/// Unused entity but it could still exist
/// </summary>
public class PlayerStart : Entity
{
    public string StartAngles { get; set; }

    public PlayerStart(Vector3 position,string startAngles) : base(position)
    {
        StartAngles = startAngles;
    }
}