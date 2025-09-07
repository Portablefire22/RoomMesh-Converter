using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter.Obj;

public class ObjExporter : Exporter
{
    
    public ObjExporter(string name, string outputDirectory, string filePath, RoomMeshReader reader) : base(reader, filePath, name, outputDirectory)
    {
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.obj");
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<RoomMeshReader>();
    }

    private void WriteVertexPosition(Vector3 pos)
    {
        var str = Encoding.UTF8.GetBytes($"v {pos.X} {pos.Y} {pos.Z}\n");
        OutputFileStream.Write(str);
    }

    private void WriteVertexUv(Vector2 pos)
    {
        var str = Encoding.UTF8.GetBytes($"vt {-pos.X} {-pos.Y}\n");
        OutputFileStream.Write(str);
    }
    
    private int WriteGeometricVertices()
    {
        int i = 0;
        var dict = Reader.TextureVertices;
        foreach (var texture in dict)
        {
            foreach (var vertex in texture)
            {
                WriteVertexPosition(vertex.Position);
                i++;
            }
        } 
        foreach (var texture in dict)
        {
            foreach (var vertex in texture)
            {
                WriteVertexUv(vertex.Uv);
            }
        }

        return i;
    }

    private void WriteIndex(int[] face)
    {
        var str = Encoding.UTF8.GetBytes($"f {face[0]}/{face[0]} {face[1]}/{face[1]} {face[2]}/{face[2]}\n");
        OutputFileStream.Write(str);
    }
    
    private int WriteVertexIndices()
    {
        int i = 0;
        var l = 0;
        var list = Reader._vertexIndices;
        foreach (var texture in list)
        {
            var str = Encoding.UTF8.GetBytes($"g {l}\n");
            OutputFileStream.Write(str);
            // Specify what material this should use
            str = Encoding.UTF8.GetBytes($"usemtl {Reader.TexturePaths[l]}\n");
            OutputFileStream.Write(str);
            for (int j = 0; j < texture.Length; j += 3)
            {
               WriteIndex(texture[new Range(j,j + 3)]);
               i += 3;
            }
            l++;
        }
        return i;
    }


    
    public override void Convert()
    {
        var str = Encoding.UTF8.GetBytes($"# Lilith's RoomMesh Converter\n" +
                                         $"# https://github.com/Portablefire22/RoomMesh-Converter\n" +
                                         $"mtllib {Name}.mtl\n");
        OutputFileStream.Write(str);
        // OBJ indices start at 1 :)
        foreach (var indices in Reader._vertexIndices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] += 1;
            }
        }
        Logger.LogInformation("Wrote Geometric Vertices: {}", WriteGeometricVertices());
        Logger.LogInformation("Wrote Indices: {}", WriteVertexIndices());

        var mtl = new MtlWriter($"{OutputDirectory}", Name,  InputDirectory, Reader.TexturePaths);
        mtl.Convert();
        
        OutputFileStream.Close();
    }

    public override void Dispose()
    {
        Reader.Dispose();
        OutputFileStream.Dispose();
        GC.SuppressFinalize(this);
    }

    public override async ValueTask DisposeAsync()
    {
        await Reader.DisposeAsync();
        await OutputFileStream.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}