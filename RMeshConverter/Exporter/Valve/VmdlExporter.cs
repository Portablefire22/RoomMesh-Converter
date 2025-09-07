using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter.Obj;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter.Valve;

public class VmdlExporter : Exporter
{
    private Exporter _objExporter;
    private string _root = "models";
    public VmdlExporter(RoomMeshReader reader, string inputFilePath, string name, string outputDirectory) : base(reader, inputFilePath, name, outputDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<VmdlExporter>();
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.vmdl");
        
        // Vmdl files require an initial file, we use OBJ for this 
        _objExporter = new ObjExporter(name, $"{outputDirectory}\\source", inputFilePath, reader);
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
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

    private void WriteRenderMesh()
    {
        var str = "{\n" +
                                "_class=\"RenderMeshList\"\n" +
                                "children = [\n" +
                                "{\n" +
                                "_class=\"RenderMeshFile\"\n" +
                                $"filename=\"{_root}/source/{Name}.obj\"\n" +
                                "import_translation = [0.0, 0.0, 0.0]\n" +
                                "import_rotation=[0.0,0.0,0.0]\n" +
                                "import_scale=1.0\n" +
                                "align_origin_x_type=\"None\"\n" +
                                "align_origin_y_type=\"None\"\n" +
                                "align_origin_z_type=\"None\"\n" +
                                "parent_bone=\"\"\n" +
                                "import_filter={\n" +
                                "exclude_by_default = false\n" +
                                "exception_list = []\n";

        str += "}\n},\n";
        OutputFileStream.Write(Encoding.UTF8.GetBytes(str));
    }
    
    private void WriteChildren()
    {
        WriteMaterialGroup();
        WriteRenderMesh();
    }
    
    public override void Convert()
    {
        _objExporter.Convert();
        
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