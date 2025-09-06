using System.IO;

namespace RMeshConverter.Exporter;

public class FbxNode
{
    public uint EndOffset { get; }
    public string Name { get; }
    public FbxProperty[] Properties { get; }

    public List<dynamic> _nestedList; // Idk what this is for
}