using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter.Obj;

public class RoomMeshToObjWriter : IDisposable, IAsyncDisposable
{
    private RoomMeshReader _reader;

    private FileStream _fileStream;

    private ILogger _logger;
    
    
    public RoomMeshToObjWriter(string path, RoomMeshReader reader)
    {
        try
        {
            Directory.CreateDirectory(path.TrimEnd('\\').Remove(path.LastIndexOf('\\') + 1));
        } catch {}
        _fileStream = File.Create(path);
        _reader = reader;
        
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<RoomMeshReader>();
    }

    private void WriteVertexPosition(Vector3 pos)
    {
        var str = Encoding.UTF8.GetBytes($"v {pos.X} {pos.Y} {pos.Z}\n");
        _fileStream.Write(str);
    }

    private void WriteVertexUv(Vector2 pos)
    {
        var str = Encoding.UTF8.GetBytes($"vt {pos.X} {pos.Y}\n");
        _fileStream.Write(str);
    }
    
    private int WriteGeometricVertices()
    {
        int i = 0;
        var dict = _reader.TextureVertices;
        foreach (var texture in dict)
        {
            foreach (var vertex in texture)
            {
                WriteVertexPosition(vertex.Position);
                WriteVertexUv(vertex.Uv);
                i++;
            }
        }

        return i;
    }

    private void WriteIndex(int[] face)
    {
        var str = Encoding.UTF8.GetBytes($"f {face[0]}// {face[1]}// {face[2]}//\n");
        _fileStream.Write(str);
    }
    
    private int WriteVertexIndices()
    {
        int i = 0;
        var l = 0;
        var list = _reader._vertexIndices;
        foreach (var texture in list)
        {
            var str = Encoding.UTF8.GetBytes($"g {l}\n");
            _fileStream.Write(str);
            for (int j = 0; j < texture.Length; j += 3)
            {
               WriteIndex(texture[new Range(j,j + 3)]);
               i += 3;
            }
            l++;
        }
        return i;
    }
    
    public void Convert()
    {
        // OBJ indices start at 1 :)
        foreach (var indices in _reader._vertexIndices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] += 1;
            }
        }
        _logger.LogInformation("Wrote Geometric Vertices: {}", WriteGeometricVertices());
        _logger.LogInformation("Wrote Indices: {}", WriteVertexIndices()); 
        _fileStream.Close();
    }

    public void Dispose()
    {
        _reader.Dispose();
        _fileStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _reader.DisposeAsync();
        await _fileStream.DisposeAsync();
    }
}