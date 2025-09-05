using System.IO;
using System.Text;

namespace RMeshConverter.RMesh;

public class CbConverter
{
    private FileStream _fileStream;
    public CbConverter(string path)
    {
        _fileStream = File.Open(path, FileMode.Open);
    }
    
    public string ReadB3DString()
    {
        byte[] buf = new byte[4];
        _fileStream.ReadExactly(buf, 0, 4);
        var length = BitConverter.ToInt32(buf);
        buf = new byte[length];
        _fileStream.ReadExactly(buf, 0, length);
        return Encoding.Default.GetString(buf);
    }
}