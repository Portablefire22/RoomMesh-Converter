using System.Numerics;
using System.Text.Json.Serialization;

namespace RMeshConverter.Exporter.Valve.Json;

[JsonDerivedType(typeof(SpotlightComponent))]
public class LightComponent : Component
{
    public float Attenuation{get;set;}
    public bool FogMode{get;set;}
    public float FogStrength{get;set;}
    public Vector4Model LightColor{get;set;}
    public float Radius{get;set;}
    public bool Shadows{get;set;}

    public LightComponent(float attenuation, bool fogMode, float fogStrength, Vector4 lightColor, float radius, bool shadows)
    {
        __type = "Sandbox.PointLight";
        Attenuation = .1f; //attenuation;
        FogMode = fogMode;
        FogStrength = fogStrength;
        LightColor = new Vector4Model(lightColor);
        Radius = radius * 1.5f;
        Shadows = shadows;
    }
}