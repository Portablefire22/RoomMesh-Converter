using System.Numerics;
using System.Text.Json.Serialization;
using RMeshConverter.Exporter.Valve.Json;

public class ModelColliderComponent : Component
{
    public float? Elasticity { get; set; }= null;
    public float? Friction { get; set; }= null;
    public bool IsTrigger { get; set; }= false;
    public string Model { get; set; }
    public float? RollingResistance { get; set; } = null;
    public bool Static { get; set; }= false;
    public object? Surface { get; set; }= null;
    public Vector3Model SurfaceVelocity { get; set; }= new Vector3Model(new Vector3(0));

    public ModelColliderComponent(string model) : base()
    {
        __type = "Sandbox.ModelCollider";
        Model = model;
    }
}