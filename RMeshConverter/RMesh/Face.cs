using System.Numerics;

namespace RMeshConverter.RMesh;

public class Face
{
    public Vertex[] Vertices;

    public Face(Vertex[] vertices)
    {
        Vertices = vertices;
    }
    
    public BoundingBox GetBoundingBox()
    {
        Vector2 min = Vector2.Min(Vertices[0].Uv, Vector2.Min(Vertices[1].Uv, Vertices[2].Uv));
        Vector2 max = Vector2.Max(Vertices[0].Uv, Vector2.Max(Vertices[1].Uv, Vertices[2].Uv));
        return new BoundingBox(min, max);
    }
}