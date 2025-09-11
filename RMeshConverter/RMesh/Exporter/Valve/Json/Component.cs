using System.Numerics;
using System.Text.Json.Serialization;

namespace RMeshConverter.Exporter.Valve.Json;

[JsonDerivedType(typeof(LightComponent))]
[JsonDerivedType(typeof(SpotlightComponent))]
[JsonDerivedType(typeof(ModelRendererComponent))]
public class Component
{
    public string __type{get;set;}
    public string __guid { get; set; } = Guid.NewGuid().ToString();
    public bool __enabled { get; set; } = true;

    public string? OnComponentDestroy{get;set;}
    public string? OnComponentDisabled{get;set;}
    public string? OnComponentEnabled{get;set;}
    public string? OnComponentFixedUpdate{get;set;}
    public string? OnComponentStart{get;set;}
    public string? OnComponentUpdate{get;set;}

}