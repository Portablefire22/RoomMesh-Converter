using System.IO;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter;

public abstract class Exporter : IDisposable, IAsyncDisposable
{

    protected readonly RoomMeshReader Reader;
    protected readonly string Name;
    protected FileStream OutputFileStream;
    protected readonly string InputDirectory;
    protected readonly string OutputDirectory;
    
    protected ILogger Logger;

    protected Exporter(RoomMeshReader reader, string inputFilePath, string name, string outputDirectory)
    {
        Reader = reader;
        Name = name;
        OutputDirectory = outputDirectory;

        try
        {
            Directory.CreateDirectory(outputDirectory);
        } catch{}
        
        InputDirectory = inputFilePath.Replace($"{Name}.rmesh", "");
    }

    public abstract void Dispose();
    public abstract ValueTask DisposeAsync();
    
    public abstract void Convert();


}