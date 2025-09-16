using System.Numerics;

namespace RMeshConverter.Exporter.Valve.Json;

public class Vector4Model
{
    
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
    public Vector4Model() {}

    public Vector4Model(Vector4 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
        W = v.W;
    }
    public Vector4 ToVector3()   
    {  
        return new Vector4(X, Y, Z, W);  
    }  }