using System.IO;
using System.Text;

namespace RMeshConverter.Exporter.Obj;

public class MtlWriter
{
    private List<string> _textureLocations;
    private FileStream _fileStream;

    private string _originalDirectory;
    private string _name;
    private string _path;
    public MtlWriter(string path, string name, string originalDirectory, List<string> textureLocations)
    {
        _name = name;
        _path = path;
        _originalDirectory = originalDirectory;
        _fileStream = File.Create($"{path}\\{name}.mtl");
        _textureLocations = textureLocations;
    }

    private void CopyTexture(string name)
    {
        File.Copy($"{_originalDirectory}{name}", $"{_path}\\{name}", true);
    }
    public void Convert()
    {
        var str = Encoding.UTF8.GetBytes($"# Lilith's RoomMesh Converter\n" +
                                         $"# https://github.com/Portablefire22/RoomMesh-Converter\n");
        _fileStream.Write(str);
        foreach (var path in _textureLocations)
        {
            CopyTexture(path);
            str = Encoding.UTF8.GetBytes($"newmtl {path}\n");
            _fileStream.Write(str);
            str = Encoding.UTF8.GetBytes($"map_Ka {path}\n");
            _fileStream.Write(str);
            str = Encoding.UTF8.GetBytes($"map_Kd {path}\n");
            _fileStream.Write(str);
        }

        _fileStream.Close();
    }
    
}