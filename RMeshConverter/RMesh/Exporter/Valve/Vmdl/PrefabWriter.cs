using System.CodeDom;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using RMeshConverter.Exporter.Obj;
using RMeshConverter.Exporter.Valve.Json;
using RMeshConverter.RMesh;
using RMeshConverter.RMesh.Entity;

namespace RMeshConverter.Exporter.Valve;

public class PrefabWriter : MeshExporter
{
    private RoomMeshReader _reader;
    private VmdlRoomMeshExporter _roomMeshExporter;

    private int _lightCount;
    private int _modelCount;
    private int _playerStartCount;
    private int _screenCount;
    private int _soundEmitterCount;
    private int _spotlightCount;
    private int _wayPointCount;

    private string _root;
    
    public PrefabWriter(RoomMeshReader reader, string inputFilePath, string name, string outputDirectory, string root) : base(inputFilePath, name, outputDirectory)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<VmdlRoomMeshExporter>();
        _reader = reader;
        _root = root;
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.prefab");
        
        _roomMeshExporter = new VmdlRoomMeshExporter(reader, inputFilePath, name,  $"{outputDirectory}\\source", root, true);
    }

    public override void Dispose()
    {
        _reader.Dispose();
        _roomMeshExporter.Dispose();
        OutputFileStream.Dispose();
    }

    public Vector4 Vector4FromLightString(string str)
    {
        var split = str.Split(" ");
        return new Vector4(float.Parse(split[0]) / 255f, float.Parse(split[1]) / 255f, float.Parse(split[2]) / 255f, 1);
    }

    public Component SpotlightComponent(Spotlight spotlight)
    {
        return new SpotlightComponent(spotlight.Intensity, true, 1, Vector4FromLightString(spotlight.Color), spotlight.Range, true, spotlight.InnerConeAngle, spotlight.OuterConeAngle, null);
    }

    public Component LightComponent(Light light)
    {
        return new LightComponent(light.Intensity, true, 1, Vector4FromLightString(light.Color), light.Range, true);
    }
    
    public Component? ComponentFromEntity(Entity entity)
    {
        switch (entity)
        {
            case PlayerStart:
            case Screen:
            case SoundEmitter:
            case Waypoint:
            case Model m:
                return null;
            case Spotlight spotlight:
                return SpotlightComponent(spotlight);
            case Light light:
                return LightComponent(light);
            default:
                throw new Exception($"invalid component '{entity.GetType()}'");
        }
    }

    public string GetName(Entity entity)
    {
        var str = "";
        switch (entity)
        { 
            case PlayerStart:
                str = $"PlayerStart {_playerStartCount}";
                _playerStartCount++;
                break;
            case Screen:
                str = $"Screen {_screenCount}";
                _screenCount++;
                break;
            case SoundEmitter:
                str = $"SoundEmitter {_soundEmitterCount}";
                _soundEmitterCount++;
                break;
            case Waypoint:
                str = $"Waypoint {_wayPointCount}";
                _wayPointCount++;
                break;
            case Model m:
                str = $"Model {_modelCount}";
                _modelCount++;
                break;
            case Spotlight spotlight:
                str = $"Spotlight {_spotlightCount}";
                _spotlightCount++;
                break;
            case Light light:
                str = $"Light {_lightCount}";
                _lightCount++;
                break;
            default:
                throw new Exception($"invalid component '{entity.GetType()}'");
        }

        return str;
    }

    public string AddTag(string tags, string tag)
    {
        if (tags.Length == 0) return tag;
        return tags + $",{tag}";
    }
    public string GetTags(Entity entity)
    {
        var str = "";
        switch (entity)
        {
            case Spotlight:
                AddTag(str, "light");
                AddTag(str, "light_spot");
                break;
            case Light:
                AddTag(str, "light");
                break;
        }
        return str;
    }

    public Vector4 Vector4FromAngles(string angles)
    {
        var temp = angles.Split(" ");
        return new Vector4(float.Parse(temp[0]), float.Parse(temp[1]) - 90f, float.Parse(temp[2]), 1);
    }
    
    
    public override void Convert()
    {
        try
        {
            _roomMeshExporter.Convert();
        }
        catch (Exception e)
        {
            Logger.LogCritical("{}", e);
        }
        // Exporting isn't perfect so we wait until conversion to create the file
        var children = new List<GameObject>();
        foreach (var entity in _reader.Entities)
        {
            GameObject obj;
            var comp = ComponentFromEntity(entity);
            if (comp == null) continue;
            switch (entity)
            {
                case Spotlight spotlight:
                    obj = new GameObject(GetName(entity), spotlight.Position, Vector4FromAngles(spotlight.Angles), GetTags(entity), new []{comp}, null);
                    break;
                default:
                    obj = new GameObject(GetName(entity), entity.Position, GetTags(entity), new []{comp}, null);
                    break;
            }
            children.Add(obj);
        }

        var modelRenderer = new ModelRendererComponent(18446744073709551615, false, null, null, null, null, $"prefabs\\Map\\source\\{Name}.vmdl", "On", new Vector4(1));
        var rootObject = new GameObject(Name, new Vector3(0), new Vector4(0,0,0,1), "", new []{modelRenderer}, children.ToArray());
        var root = new Root(rootObject);

        var options = new JsonSerializerOptions();
        options.Converters.Add(new Vector3Converter());
        options.Converters.Add(new Vector4Converter());
        string jsonString = JsonSerializer.Serialize(root, options);
        OutputFileStream.Write(Encoding.UTF8.GetBytes(jsonString));
    }
}