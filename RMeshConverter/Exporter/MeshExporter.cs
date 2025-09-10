using System.IO;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter;

public abstract class MeshExporter : IDisposable, IAsyncDisposable
{

    protected readonly string Name;
    protected FileStream OutputFileStream;
    protected readonly string InputDirectory;
    protected readonly string OutputDirectory;

    protected int IndicesOffset = 0;
    
    protected ILogger Logger;

    protected MeshExporter(string inputFilePath, string name, string outputDirectory)
    {
        Name = name;
        OutputDirectory = outputDirectory;

        try
        {
            Directory.CreateDirectory(outputDirectory);
        } catch{}
        
        InputDirectory = inputFilePath.Remove(inputFilePath.LastIndexOf('.')).Replace(Name, "");
    }
    

    public abstract void Dispose();
    public abstract ValueTask DisposeAsync();
    
    public abstract void Convert();


}