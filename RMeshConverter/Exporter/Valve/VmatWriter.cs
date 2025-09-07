using System.IO;
using System.Text;

namespace RMeshConverter.Exporter.Valve;

public class VmatWriter : MaterialWriter
{
    private string _currentName;
    private string _root = "models";
    public VmatWriter(List<string> textureLocations, string originalDirectory, string path, string name) : base(textureLocations, originalDirectory, path, name)
    {
    }

    private void WriteOpenLayer(int layerNum)
    {
        WriteString($"Layer{layerNum} \n{{\n");
    }

    private void WriteShader(string shaderPath)
    {
        WriteString($"shader \"{shaderPath}\"\n");
    }

    private void WriteColor(string textureName)
    {
        WriteString($"TextureColor \"{_root}\\source\\{textureName}\"\n");
    }

    private void TryWriteBump(string textureName)
    {
        var bumpName = GetBumpName(textureName);
        if (File.Exists($"{Path}\\{bumpName}"))
        {
            WriteString($"TextureNormal \"{_root}\\source\\{bumpName}\"\n");
        }
    }
    
    public override void Convert()
    {
        foreach (var textureName in TextureLocations)
        {
            _currentName = RemoveExtension(textureName);
            OutputFileStream = File.Create($"{Path}\\{_currentName}.vmat");

            OutputFileStream.Write("// Lilith's RoomMesh Converter\n"u8 +
                                   "// https://github.com/Portablefire22/RoomMesh-Converter\n"u8);
            WriteOpenLayer(0);
            WriteShader("shaders/complex.shader");
            WriteColor(textureName);
            TryWriteBump(textureName);
            
            OutputFileStream.Write("}"u8);
            
            OutputFileStream.Close();
        }
    }
}