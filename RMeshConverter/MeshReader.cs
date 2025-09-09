using System.IO;
using Microsoft.Extensions.Logging;

namespace RMeshConverter;

public abstract class MeshReader : IDisposable
{
    protected ILogger Logger;
    protected FileStream InputFileStream;


    public abstract void Dispose();
}