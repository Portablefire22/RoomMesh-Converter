using System.Numerics;

namespace RMeshConverter.Exporter.Valve.Json;

public class Vector3Model
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public Vector3Model() {}

    public Vector3Model(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
    public Vector3 ToVector3()   
    {  
        return new Vector3(X, Y, Z);  
    }  
}