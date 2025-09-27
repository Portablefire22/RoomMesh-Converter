using System.Numerics;

namespace RMeshConverter.RMesh;

public class BoundingBox
{
    public Vector2 x1;
    private Vector2 x2;
    public Vector2 y1;
    private Vector2 y2;

    public BoundingBox(Vector2 x1, Vector2 y1)
    {
        this.x1 = x1;
        this.y1 = y1;
    }
}