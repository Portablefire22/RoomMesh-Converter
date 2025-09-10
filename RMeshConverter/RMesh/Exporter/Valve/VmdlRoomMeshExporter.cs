using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter.Obj;
using RMeshConverter.RMesh;
using RMeshConverter.RMesh.Entity;

namespace RMeshConverter.Exporter.Valve;

public class VmdlRoomMeshExporter : MeshExporter
{
    private RoomMeshReader Reader;
    private MeshExporter _objRoomMeshExporter;
    private string _root = "models";
    public VmdlRoomMeshExporter(RoomMeshReader reader, string inputFilePath, string name, string outputDirectory) : base(inputFilePath, name, outputDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<VmdlRoomMeshExporter>();
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.vmdl");
        Reader = reader;
        
        _objRoomMeshExporter = new ObjRoomMeshExporter(name, $"{outputDirectory}\\source", inputFilePath, reader);
    }

    public override void Dispose()
    {
        Reader.Dispose();
        _objRoomMeshExporter.Dispose();
        OutputFileStream.Dispose();
    }

    private void WriteRootNode()
    {
        OutputFileStream.Write(("{\n"u8 +
                                "rootNode=\n"u8 +
                                "{\n"u8 +
                                "_class=\"RootNode\"\n"u8 +
                                "model_archetype=\"\"\n"u8 +
                                "primary_associated_entity=\"\"\n"u8 +
                                "anim_graph_name=\"\"\n"u8 +
                                "base_model_name=\"\"\n"u8 +
                                "children = [\n"u8).ToArray());
    }

    private void WriteCloseRoot()
    {
        OutputFileStream.Write("]\n},\n]\n}\n}"u8.ToArray());
    }

    private void WriteMaterialRemaps()
    {
        foreach (var texture in Reader.TexturePaths)
        {
            var textureName = texture.Remove(texture.LastIndexOf('.'));
            var str = $"{{\nfrom=\"{textureName}.vmat\"\n" +
                      $"to=\"{_root}/source/{textureName}.vmat\"\n" +
                      $"}},\n";
            OutputFileStream.Write(Encoding.UTF8.GetBytes(str));
        }
    }
    
    private void WriteMaterialGroup()
    {
        OutputFileStream.Write("{\n"u8 +
                                "_class=\"MaterialGroupList\"\n"u8 +
                                "children =\n"u8 +
                                "[\n"u8 +
                                "{\n"u8 +
                                "_class=\"DefaultMaterialGroup\"\n"u8 +
                                "remaps = [\n"u8);
        WriteMaterialRemaps();
        OutputFileStream.Write( "]\n"u8 +
                                "use_global_default = false\n"u8 +
                                "global_default_material = \"materials/default.vmat\"\n"u8 +
                                "},\n"u8 +
                                "]\n"u8 +
                                "},\n"u8);    
    }

    private void WriteRenderMeshList()
    {

        var str = "{\n"u8 +
                   "_class=\"RenderMeshList\"\n"u8 +
                   "children = [\n"u8;
        OutputFileStream.Write(str);
    }
    private void WriteRenderMesh(string fileName, Vector3 translation, Vector3 rotation, float scale)
    {
        var str = "{\n_class=\"RenderMeshFile\"\n" +
                  $"filename=\"{_root}/source/{fileName}.obj\"\n" +
                  $"import_translation = [{translation.X}, {translation.Y}, {translation.Z}]\n" +
                  $"import_rotation=[{rotation.X},{rotation.Y},{rotation.Z}]\n" +
                  $"import_scale={scale}\n" +
                  "align_origin_x_type=\"None\"\n" +
                  "align_origin_y_type=\"None\"\n" +
                  "align_origin_z_type=\"None\"\n" +
                  "parent_bone=\"\"\n" +
                  "import_filter={\n" +
                  "exclude_by_default = false\n" +
                  "exception_list = []\n" +
              "}\n" +
          "},\n";
        OutputFileStream.Write(Encoding.UTF8.GetBytes(str));
    }
    
    private void WriteChildren()
    {
        WriteMaterialGroup();
        WriteRenderMeshList();
        WriteRenderMesh(Name, new Vector3(0), new Vector3(0), 1);
        foreach (var mesh in Reader.Entities.OfType<Model>().ToArray())
        {
           WriteRenderMesh($"models/{mesh.Name.Remove(mesh.Name.LastIndexOf('.'))}", mesh.Position, mesh.Rotation, mesh.Scale.X); 
        }
    }
    
    public override void Convert()
    {
        _objRoomMeshExporter.Convert();
        
        var header = "<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:modeldoc29:version{3cec427c-1b0e-4d48-a90a-0436f33a6041} -->\n"u8.ToArray();
        OutputFileStream.Write(header);
        WriteRootNode();
        WriteChildren();
        WriteCloseRoot();
        OutputFileStream.Close();

        var matExporter = new VmatWriter(Reader.TexturePaths, InputDirectory, $"{OutputDirectory}\\source", Name);
        matExporter.Convert();
    }
}