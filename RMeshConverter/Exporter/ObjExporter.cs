using System.IO;
using System.Numerics;
using System.Text;

namespace RMeshConverter.Exporter;

public class ObjExporter : MeshExporter
{
    public ObjExporter(string inputFilePath, string name, string outputDirectory) : base( inputFilePath, name, outputDirectory)
    {
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.obj");
    }

    protected void WriteVertexPosition(Vector3 pos)
    {
        var str = Encoding.UTF8.GetBytes($"v {pos.X} {pos.Y} {-pos.Z}\n");
        OutputFileStream.Write(str);
    }

    protected void WriteVertexUv(Vector2 pos)
    {
        var str = Encoding.UTF8.GetBytes($"vt {pos.X} {-pos.Y}\n");
        OutputFileStream.Write(str);
    }
    protected void WriteIndex(int[] face)
    {
        // OBJ indices start at 1 :)
        for (int i = 0; i < face.Length; i++)
        {
            face[i] += 1 + IndicesOffset;
        }
        var str = "f";
        foreach (var index in face)
        {
            str += $" {index}/{index}";
        }
        WriteLine(str);
    }

    protected void WriteString(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        OutputFileStream.Write(bytes);
    }

    protected void WriteLine(string str)
    {
        if (str.EndsWith('\n'))
        {
            WriteString(str);
            return;
        }
        WriteString(str + '\n');
    }

    protected void WriteMtlLib()
    {
        WriteLine($"mtllib {Name}.mtl");
    }

    protected void WriteGroup(string groupName)
    {
        WriteLine($"g {groupName}");
    }

    protected void WriteObjectName(string name)
    {
        WriteLine($"o {name}");
    }
    
    protected void WriteHeader()
    {
        WriteLine("# Lilith's RoomMesh Converter\n" +
                  "# https://github.com/Portablefire22/RoomMesh-Converter");
        WriteObjectName(Name);
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public override void Convert()
    {
        throw new NotImplementedException();
    }
}