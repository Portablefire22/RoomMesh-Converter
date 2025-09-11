using System.Numerics;

namespace RMeshConverter.Exporter.Valve.Json;

public class SpotlightComponent : LightComponent
{
    public float ConeInner{get;set;}
    public float ConeOuter{get;set;}
    public string? Cookie{get;set;}
    
    public SpotlightComponent(float attenuation, bool fogMode, float fogStrength, Vector4 lightColor, float radius, bool shadows, float coneInner, float coneOuter, string? cookie) : base(attenuation, fogMode, fogStrength, lightColor, radius, shadows)
    {
        __type = "Sandbox.SpotLight";
        ConeInner = coneInner;
        ConeOuter = coneOuter;
        Cookie = cookie;
    }
}