using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter;
using RMeshConverter.RMesh;

namespace RMeshConverter.XModel;

public class XExporter : ObjExporter
{
    private XAsciiReader Reader;
    public XExporter(XAsciiReader reader, string inputFilePath, string name, string outputDirectory) : base(inputFilePath, name, outputDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<XExporter>();
        Reader = reader;
    }
    
    public void WriteFace(MeshFace face)
    {
        WriteIndex(face.Indices);
    }

    private void WriteFaceMaterial(string material)
    {
        WriteLine($"usemtl {material}");
    }
    
    private void WriteMesh(Mesh mesh)
    {
        foreach (var vert in mesh.Vertices)
        {
            WriteVertexPosition(vert);
        }
        foreach (var uv in mesh.TextureCoords)
        {
            WriteVertexUv(uv);
        }

        var i = 0;
        foreach (var face in mesh.Faces)
        {
            if (!mesh.MaterialList.AllMaterialsSame || i == 0)
            {
                WriteFaceMaterial(mesh.MaterialList.MaterialName[i]);
            }
            WriteFace(face);
            i++;
        }
        IndicesOffset += mesh.Vertices.Count;
    }
    
    private void WriteFrame(Frame frame)
    {
        WriteGroup(frame.Name);
        foreach (var childFrame in frame.Children)
        {
            WriteFrame(childFrame);
        }

        foreach (var mesh in frame.Meshes)
        {
            WriteMesh(mesh);
        }
    }
    
    public override void Convert()
    {
        WriteHeader();
        WriteMtlLib();
        using var mtlWriter = new XMtlWriter(Reader.Materials, OutputDirectory, Name, InputDirectory);
        mtlWriter.Convert();
        foreach (var frame in Reader.Frames)
        {
            WriteFrame(frame);
        }
        OutputFileStream.Close();
    }
    
    public override void Dispose()
    {
        OutputFileStream.Dispose();
        Reader.Dispose();
    }

}