using System.Numerics;

namespace RMeshConverter.XModel;

public class Frame : Node
{
    public string Name;
    public Matrix4x4 TransformMatrix;
    public List<Frame> Children;
    public List<Mesh> Meshes;

    public Frame(string name)
    {
        Name = name;
        Meshes = new List<Mesh>();
        Children = new List<Frame>();
    }
    
    public Frame(string name, Matrix4x4 transformMatrix, List<Frame> children, List<Mesh> meshes)
    {
        Name = name;
        TransformMatrix = transformMatrix;
        Children = children;
        Meshes = meshes;
    }
}