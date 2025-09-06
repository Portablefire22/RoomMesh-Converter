using System.IO;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh.Entity;

namespace RMeshConverter.RMesh;

/*
 *  This could not have been done without the information shared by Koanyaku 
 *  At https://github.com/Koanyaku/godot_rmesh_import/blob/main/docs/rmesh_format_scp-cb.md
 */

public class RoomMeshReader : IDisposable, IAsyncDisposable
{
    private FileStream _fileStream;
    private ILogger _logger;
    
    private bool _hasTriggers;

    private int _textureCount;
    private int _indicesOffset;
    public List<Vertex[]> TextureVertices { get; set; }
    public List<string> TexturePaths { get; set; }
    public List<int[]> _vertexIndices;
    
    public RoomMeshReader(string path)
    {
        _fileStream = File.Open(path, FileMode.Open);
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<RoomMeshReader>();
        TextureVertices = new List<Vertex[]>{};
        TexturePaths = new List<string>();
        _vertexIndices = new List<int[]>();
    }
    
    ~RoomMeshReader()
    {
        Dispose(false);
    }

    public int ReadInt32()
    {
        byte[] buf = new byte[4];
        _fileStream.ReadExactly(buf, 0, 4);
        return BitConverter.ToInt32(buf);
    }
    
    public string ReadB3DString()
    {
        var length = ReadInt32();
        var buf = new byte[length];
        _fileStream.ReadExactly(buf, 0, length);
        return Encoding.Default.GetString(buf);
    }

    public float ReadFloat32()
    {
        byte[] buf = new byte[4];
        _fileStream.ReadExactly(buf, 0, 4);
        return BitConverter.ToSingle(buf);
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat32(), ReadFloat32(), ReadFloat32());
    }

    public Vector2 ReadVector2()
    {
        return new Vector2(ReadFloat32(), ReadFloat32());
    }

    public Vertex ReadVertexData()
    {
        var x = _fileStream.Position;
        var position = ReadVector3();
        var uv = ReadVector2();
        var lightmapUv = ReadVector2();
        var buf = new byte[3];
        _fileStream.ReadExactly(buf, 0, 3);
        
        return new Vertex(position, uv, lightmapUv, buf);
    }
    
    public void ReadTextureObjectData(string relativePath)
    {
        var vertexCount = ReadInt32();
        _logger.LogInformation("Associated Vertex Count: {}", vertexCount);
        var verticies = new Vertex[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            verticies[i] = ReadVertexData();
        }

        TextureVertices.Add(verticies);
        _logger.LogInformation("Read {} vertices", vertexCount);
        var x = new byte[32];
        var triangleCount = ReadInt32();
        _logger.LogInformation("Triangle Count: {}", triangleCount);
        var indices = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount * 3; i++)
        {
            indices[i] =  _indicesOffset + ReadInt32();
        }
        _indicesOffset += vertexCount;
        _vertexIndices.Add(indices);
    }
    public void ReadOpaque()
    {
        var relativePath = ReadB3DString();
        _logger.LogInformation("Found Opaque Texture: {}", relativePath);
        TexturePaths.Add(relativePath);
        ReadTextureObjectData(relativePath);
    }
    public void ReadLightmap()
    {
        var relativePath = ReadB3DString();
        _logger.LogInformation("Found Lightmap Texture: {}", relativePath);
    }

    public void ReadTransparency()
    {
        var relativePath = ReadB3DString();
        _logger.LogInformation("Found Transparency Texture: {}", relativePath);
        TexturePaths.Add(relativePath);
        ReadTextureObjectData(relativePath);
    }
    
    public void ReadTexture()
    { 
        var flag = IntToTextureFlag(_fileStream.ReadByte());
        ReadLightmap();
        flag = IntToTextureFlag(_fileStream.ReadByte());
        switch (flag)
        {
            case TextureType.Opaque:
                ReadOpaque();
                break;
            case TextureType.Transparency:
                ReadTransparency();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }     
    }

    public TextureType IntToTextureFlag(int flag)
    {
        if (flag < 1 || flag > 3)
        {
            throw new RoomMeshException($"invalid texture flag '{flag}'");
        }
        return (TextureType)flag;
    }

    public void GetInvisCollisions()
    {
        var invisCollisions = ReadInt32();
        _logger.LogInformation("Invisible Collisions: {}", invisCollisions);
        if (invisCollisions == 0) return;
        var invisCollisionsVertices = ReadInt32();
        _logger.LogInformation("Invisible Collisions Vertices: {}", invisCollisionsVertices);
        for (int i = 0; i < invisCollisionsVertices; i++)
        {
            var vert = new InvisibleCollisionVertex(ReadVector3());
        }
        
        var invisCollisionsTriangles = ReadInt32();
        _logger.LogInformation("Invisible Collisions Triangles: {}", invisCollisionsTriangles);
        for (int i = 0; i < invisCollisionsTriangles * 3; i++)
        {
            ReadInt32(); // index
        }
    }

    public void GetTriggerBoxes()
    {
        _logger.LogInformation("Reading Trigger Boxes");
        var count = ReadInt32();
        _logger.LogInformation("Trigger boxes: {}", count);
        for (int i = 0; i < count; i++)
        {
            var surfaceAmount = ReadInt32();
            var vertexCount = ReadInt32();
            for (int j = 0; j < vertexCount; j++)
            {
                var vert = new InvisibleCollisionVertex(ReadVector3());
            }
           

            var triangleCount = ReadInt32(); 
            for (int j = 0; j < triangleCount * 3; j++)
            {
                // _logger.LogInformation("Index: {}, Written: {}, Offset: {}", i, _ind, _indicesOffset);
                // indices[i] =  _indicesOffset + ReadInt32();
                ReadInt32();
                // _ind++;
            }
            var triggerBoxName = ReadB3DString();
        }
    }

    public void GetEntities()
    {
        var entityCount = ReadInt32();
        _logger.LogInformation("Entity Count: {}", entityCount);
        for (int i = 0; i < entityCount; i++)
        {
            var type = ReadB3DString();
            Entity.Entity e;
            switch (type)
            {
                case "screen":
                    e = ReadScreen();
                    _logger.LogInformation("Screen Position: {}", e.Position.ToString());
                    break;
                case "waypoint":
                    e = ReadWaypoint();
                    _logger.LogInformation("Waypoint Position: {}", e.Position.ToString());
                    break;
                case "light":
                    e = ReadLight();
                    _logger.LogInformation("Light Position: {}", e.Position.ToString());
                    break;
                case "spotlight":
                    e = ReadSpotlight();
                    _logger.LogInformation("Spotlight Position: {}", e.Position.ToString());
                    break;
                case "soundemitter":
                    e = ReadSoundEmitter();
                    _logger.LogInformation("Sound Emitter Position: {}", e.Position.ToString());
                    break;
                case "playerstart":
                    e = ReadPlayerStart();
                    _logger.LogInformation("Player Start Position: {}", e.Position.ToString());
                    break;
                case "model":
                    var m = ReadModel();
                    _logger.LogInformation("Model Name: {}", m.Name);
                    break;
                default:
                    throw new RoomMeshException("invalid entity type");
            }
        }
    }

    private Screen ReadScreen()
    {
        return new Screen(ReadVector3(), ReadB3DString());
    }

    private Waypoint ReadWaypoint()
    {
        return new Waypoint(ReadVector3());
    }

    private Light ReadLight()
    {
        return new Light(ReadVector3(), ReadFloat32(), ReadB3DString(), ReadFloat32());
    }

    private Spotlight ReadSpotlight()
    {
        return new Spotlight(ReadVector3(), ReadFloat32(), ReadB3DString(), ReadFloat32(), 
            ReadB3DString(), ReadInt32(), ReadInt32());
    }

    private SoundEmitter ReadSoundEmitter()
    {
        return new SoundEmitter(ReadVector3(), ReadInt32(), ReadFloat32());
    }

    private PlayerStart ReadPlayerStart()
    {
        return new PlayerStart(ReadVector3(), ReadB3DString());
    }

    private Model ReadModel()
    {
        return new Model(ReadB3DString(), ReadVector3(), ReadVector3(), ReadVector3());
    }
    
    public void Read()
    {
        _logger.LogInformation("Reading file: {}", _fileStream.Name);
        switch (ReadB3DString())
        {
            case "RoomMesh":
                break;
            case "RoomMesh.HasTriggerBox":
                _hasTriggers = true;
                break;
            default:
                throw new RoomMeshException("not a .rmesh file, missing 'RoomMesh' header");
        }
        _logger.LogInformation("Has triggers: {}", _hasTriggers);
        _textureCount = ReadInt32();
        _logger.LogInformation("Unique Textures: {}", _textureCount);
        for (int i = 0; i < _textureCount; i++)
        {
            ReadTexture();
        }

        GetInvisCollisions();
        if (_hasTriggers) GetTriggerBoxes();
        GetEntities();
       
        _fileStream.Close();
        _logger.LogInformation("Finished");
    }

    private void ReleaseUnmanagedResources()
    {
        _vertexIndices.Clear();
        TextureVertices.Clear();
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            _fileStream.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        ReleaseUnmanagedResources();

        await _fileStream.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }
}

public enum TextureType
{
    Opaque = 1,
    Lightmap = 2,
    Transparency = 3,
}