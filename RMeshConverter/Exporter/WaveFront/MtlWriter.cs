using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter.Valve;

namespace RMeshConverter.Exporter.Obj;

public class MtlWriter : MaterialWriter
{
    public MtlWriter(string path, string name, string originalDirectory, List<string> textureLocations) 
        : base(textureLocations, originalDirectory, path, name)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<MtlWriter>();
        
        OutputFileStream = File.Create($"{path}\\{name}.mtl");
    }

    public override void Convert()
    {
        var str = Encoding.UTF8.GetBytes($"# Lilith's RoomMesh Converter\n" +
                                         $"# https://github.com/Portablefire22/RoomMesh-Converter\n");
        OutputFileStream.Write(str);
        foreach (var path in TextureLocations)
        {
            CopyTexture(path);
            str = Encoding.UTF8.GetBytes($"newmtl {path}\n");
            OutputFileStream.Write(str);
            str = Encoding.UTF8.GetBytes($"map_Ka {path}\n");
            OutputFileStream.Write(str);
            str = Encoding.UTF8.GetBytes($"map_Kd {path}\n");
            OutputFileStream.Write(str);
        }

        OutputFileStream.Close();
    }
    
}