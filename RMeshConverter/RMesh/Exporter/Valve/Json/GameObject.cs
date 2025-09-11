using System.Numerics;

namespace RMeshConverter.Exporter.Valve.Json;

public class GameObject
{
    public string __guid { get; set; } = Guid.NewGuid().ToString();
    public int __version { get; set; } = 1;
    public int Flags { get; set; } = 0;
    public string Name{get;set;}
    public Vector3Model Position{get;set;}
    public Vector4Model Rotation{get;set;}
    public Vector3Model Scale { get; set; } = new Vector3Model(new Vector3(1));
    public string Tags{get;set;}
    public bool Enabled { get; set; } = true;
    public int NetworkMode { get; set; } = 2;
    public bool NetworkInterpolation { get; set; } = true;
    public int NetworkOrphaned { get; set; } = 0;
    public int OwnerTransfer {get;set;} = 1;
    public Component[]? Components { get; set; } = null;
    public GameObject[]? Children { get; set; } = null;

    public GameObject(string name, Vector3 position, Vector4 rotation, string tags, Component[]? components, GameObject[]? children)
    {
        Name = name;
        // Y is up for CB but Z is up for S&Box
        position = new Vector3(position.X, position.Z, position.Y);
        Position = new Vector3Model(position);
        rotation = Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Z, rotation.Y).AsVector4();
        Rotation = new Vector4Model(rotation);
        Tags = tags;
        if (components != null && components.Length != 0) Components = components;
        if (children != null && children.Length != 0) Children = children;
    }

    public GameObject(string name, Vector3 position, string tags, Component[]? components, GameObject[]? children)
    {
        Name = name;
        position = new Vector3(position.X, position.Z, position.Y);
        Position = new Vector3Model(position);
        Rotation = new Vector4Model(new Vector4(0,0,0,1));
        Scale = new Vector3Model(new Vector3(1));
        Tags = tags;
        if (components != null && components.Length != 0) Components = components;
        if (children != null && children.Length != 0) Children = children;
    }
}