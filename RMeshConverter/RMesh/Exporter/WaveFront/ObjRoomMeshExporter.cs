using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;
using RMeshConverter.RMesh.Entity;
using RMeshConverter.XModel;

namespace RMeshConverter.Exporter.Obj;

public class ObjRoomMeshExporter : ObjExporter
{
    private RoomMeshReader Reader;
    public ObjRoomMeshExporter(string name, string outputDirectory, string filePath, RoomMeshReader reader) : base(filePath, name, outputDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<RoomMeshReader>();
        Reader = reader;
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
    
    private int WriteVertexIndices()
    {
        int i = 0;
        var l = 0;
        var list = Reader._vertexIndices;
        foreach (var texture in list)
        {
            WriteGroup(l.ToString());
            // Specify what material this should use
            WriteLine($"usemtl {Reader.TexturePaths[l]}");
            for (int j = 0; j < texture.Length; j += 3)
            {
                var ind = texture[new Range(j, j +  3)];
                ind = new[] { ind[0], ind[2], ind[1] };
               WriteIndex(ind);
               i += 3;
            }
            l++;
        }
        return i;
    }
    
    public override void Convert()
    {
        WriteHeader();
        WriteMtlLib();
        Logger.LogInformation("Wrote Geometric Vertices: {}", WriteGeometricVertices());
        Logger.LogInformation("Wrote Indices: {}", WriteVertexIndices());

        var mtl = new RoomMeshMtlWriter($"{OutputDirectory}", Name,  InputDirectory, Reader.TexturePaths);
        mtl.Convert();
        
        OutputFileStream.Close();
    }

    public override void Dispose()
    {
        Reader.Dispose();
        OutputFileStream.Dispose();
        GC.SuppressFinalize(this);
    }
}