using System.Numerics;

namespace RMeshConverter.XModel;

public class Frame : Node
{
    public Matrix4x4 TransformMatrix;
    public List<Frame> Children;
    public List<Mesh> Meshes;

    public Frame()
    {
        Meshes = new List<Mesh>();
    }
    
    public Frame(Matrix4x4 transformMatrix, List<Frame> children, List<Mesh> meshes)
    {
        TransformMatrix = transformMatrix;
        Children = children;
        Meshes = meshes;
    }
}