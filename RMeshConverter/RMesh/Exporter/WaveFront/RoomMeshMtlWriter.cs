using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter.Valve;

namespace RMeshConverter.Exporter.Obj;

public class RoomMeshMtlWriter : MtlWriter
{
    public RoomMeshMtlWriter(string path, string name, string originalDirectory, List<string> textureLocations) 
        : base( textureLocations, path, name, originalDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<RoomMeshMtlWriter>();
    }

    public override void Convert()
    {
        WriteHeader();
        foreach (var path in TextureLocations)
        {
            try
            {
                CopyTexture(path);
            }
            catch (IOException e)
            {
                // Some rooms are completely unfinished and dont have textures;
                Logger.LogCritical("{}", e);
            }
            WriteTexture(path);
        }
        OutputFileStream.Close();
    }
    
}