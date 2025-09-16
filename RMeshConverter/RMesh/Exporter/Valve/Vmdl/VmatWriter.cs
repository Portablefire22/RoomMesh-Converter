using System.IO;
using System.Text;

namespace RMeshConverter.Exporter.Valve;

public class VmatWriter : MaterialWriter, IDisposable
{
    private string _currentName;
    private string _root = "models";
    private bool _isChild;
    
    public VmatWriter(List<string> textureLocations, string originalDirectory, string path, string name, string root, bool isChild) : base(textureLocations, originalDirectory, path, name)
    {
        _root = root;
        _isChild = isChild;
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
        if (_isChild) textureName = $"source\\{textureName}";
        WriteString($"TextureColor \"{_root}\\source\\{textureName}\"\n");
    }

    private void TryWriteBump(string textureName)
    {
        var bumpName = GetBumpName(textureName);
        if (_isChild) bumpName = $"source\\{bumpName}";
        if (File.Exists($"{Path}\\{bumpName}"))
        {
            WriteString($"TextureNormal \"{_root}\\source\\{bumpName}\"\n");
        }
    }

    public void WriteTranslucent(string textureName)
    {
        if (_isChild) textureName = $"source\\{textureName}";
        OutputFileStream.Write("F_TRANSLUCENT 1\n"u8);
        
        OutputFileStream.Write("g_flOpacityScale \"1.000\"\n"u8);
        
        var str = $"TextureTranslucency \"{_root}\\source\\{textureName}\"\n";
        OutputFileStream.Write(Encoding.UTF8.GetBytes(str));
    }
    
    public override void Convert()
    {
        foreach (var textureName in TextureLocations)
        {
            try
            {
                _currentName = RemoveExtension(textureName);
                OutputFileStream = File.Create($"{Path}\\{_currentName}.vmat");

                OutputFileStream.Write("// Lilith's RoomMesh Converter\n"u8 +
                                       "// https://github.com/Portablefire22/RoomMesh-Converter\n"u8);
                WriteOpenLayer(0);
                WriteShader("shaders/complex.shader");

                if (textureName == "glass.png")
                {
                    WriteTranslucent(textureName);
                }

                WriteColor(textureName);
                TryWriteBump(textureName);


                OutputFileStream.Write("}"u8);

                OutputFileStream.Close();
            } catch (IOException e) {}
        }
    }

    public void Dispose()
    {
        OutputFileStream.Dispose();
    }
}