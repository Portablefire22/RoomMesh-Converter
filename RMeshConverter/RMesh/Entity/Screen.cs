using System.Numerics;

namespace RMeshConverter.RMesh.Entity;

public class Screen : Entity
{
    public Screen(Vector3 position, string imagePath) : base(position)
    {
        ImagePath = imagePath;
    }

    public string ImagePath { get; set; }
}