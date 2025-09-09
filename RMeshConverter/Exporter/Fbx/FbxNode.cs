using System.IO;

namespace RMeshConverter.Exporter;

public class FbxNode
{
    public FbxNode(string name, FbxProperty[] properties, FbxNode[] children)
    {
        Name = name;
        Properties = properties;
        Children = children;
        
        CalculateEndOffset();
    }

    public ulong GetPropertiesSize()
    {
        var x = 0ul; 
        foreach (var property in Properties)
        {
            x += property.GetSize();
        }

        return x;
    } 
    
    private void CalculateEndOffset()
    {
        EndOffset = (uint) (25 + Name.Length);
        EndOffset += GetPropertiesSize();
        foreach (var node in Children)
        {
            EndOffset += node.EndOffset;
        }
        EndOffset += 13;
    }

    public ulong EndOffset { get; private set; }
    public string Name { get; }
    public FbxProperty[] Properties { get; }

    public FbxNode[] Children;
}