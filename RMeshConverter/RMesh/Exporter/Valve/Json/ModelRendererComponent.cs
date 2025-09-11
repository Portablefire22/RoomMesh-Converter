using System.Numerics;

namespace RMeshConverter.Exporter.Valve.Json;

public class ModelRendererComponent : Component
{
    public ulong BodyGroups{get;set;}
    public bool CreateAttachments{get;set;}
    public string? LodOverride{get;set;}
    public string? MaterialGroup{get;set;}
    public string? MaterialOverride{get;set;}
    public string? Materials{get;set;}
    public string Model{get;set;}

    public RenderOptions RenderOptions { get; set; } = new RenderOptions();
    public string RenderType { get; set; } = "On";
    public Vector4Model Tint{get;set;}

    public ModelRendererComponent(ulong bodyGroups, bool createAttachments, string? lodOverride, string? materialGroup, string? materialOverride, string? materials, string model, string renderType, Vector4 tint)
    {
        __type = "Sandbox.ModelRenderer";
        BodyGroups = bodyGroups;
        CreateAttachments = createAttachments;
        LodOverride = lodOverride;
        MaterialGroup = materialGroup;
        MaterialOverride = materialOverride;
        Materials = materials;
        Model = model;
        RenderType = renderType;
        Tint = new Vector4Model(tint);
    }
}