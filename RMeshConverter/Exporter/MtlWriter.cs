using System.IO;
using System.Text;
using RMeshConverter.XModel;

namespace RMeshConverter.Exporter;

public abstract class MtlWriter : MaterialWriter
{
    public MtlWriter(List<string> textureLocations, string path, string name, string originalDirectory) 
        : base(textureLocations, originalDirectory, path, name)
    {
        OutputFileStream = File.Create($"{path}\\{name}.mtl");
    }
    protected void WriteString(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        OutputFileStream.Write(bytes);
    }
    
    protected void WriteHeader()
    {
        WriteLine("# Lilith's RoomMesh Converter\n" +
                  "# https://github.com/Portablefire22/RoomMesh-Converter");
    }

    protected void WriteTexture(string path)
    {
        WriteLine($"newmtl {path}");
        WriteLine($"map_Ka {path}");
        WriteLine($"map_Kd {path}");
    }

    protected void WriteTexture(Material material)
    {
        WriteLine($"newmtl {material.Name}");
        if (material.TextureFile != null)
        {
            WriteLine($"map_Ka {material.TextureFile}");
            WriteLine($"map_Kd {material.TextureFile}");
        }
        else
        {
            WriteLine($"map_Ka {material.FaceColor.X} {material.FaceColor.Y} {material.FaceColor.Z}");
            WriteLine($"map_Kd {material.FaceColor.X} {material.FaceColor.Y} {material.FaceColor.Z}");
        }
        WriteLine($"map_Ks {material.SpecularColor.X} {material.SpecularColor.Y} {material.SpecularColor.Z}");
    }
    
    protected void WriteLine(string str)
    {
        if (str.EndsWith('\n'))
        {
            WriteString(str);
            return;
        }
        WriteString(str + '\n');
    }
}