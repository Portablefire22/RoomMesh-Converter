using Microsoft.Extensions.Logging;

namespace RMeshConverter.Exporter.Valve.Dmx;

public class DmxRoomMeshExporter : DmxExporter
{
    public DmxRoomMeshExporter(string inputFilePath, string name, string outputDirectory, string root) : base(inputFilePath, name, outputDirectory, root)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<VmdlRoomMeshExporter>();
    }

    public override void Dispose()
    {
        OutputFileStream.Dispose();
    }

    public override void Convert()
    {
        throw new NotImplementedException();
    }
}