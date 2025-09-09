using System.Numerics;

namespace RMeshConverter.XModel;

public class Material
{
    public string Name;
    public Vector4 FaceColor;
    public Double Power;
    public Vector3 SpecularColor;
    public Vector3 EmissiveColor;

    public string? TextureFile;

    public Material(string name, Vector4 faceColor, double power, Vector3 specularColor, Vector3 emissiveColor, string? textureFile)
    {
        Name = name;
        FaceColor = faceColor;
        Power = power;
        SpecularColor = specularColor;
        EmissiveColor = emissiveColor;
        TextureFile = textureFile;
    }
}