using System.IO;
using System.Text;

namespace RMeshConverter.Exporter;

public class FbxWriter
{
    
    private FileStream _fileStream;

    public FbxWriter(string path)
    {
        _fileStream = File.Create(path);
    }

    private byte[] Int32ToBytes(int num)
    {
        return BitConverter.GetBytes(num);
    } 
    private byte[] UInt32ToBytes(uint num)
    {
        return BitConverter.GetBytes(num);
    }

    private void WriteNode(FbxNode node)
    {
        _fileStream.Write(UInt32ToBytes(node.EndOffset));
        _fileStream.Write(UInt32ToBytes((uint)node.Properties.Length));
    }
    private void WriteHeader()
    {
        var str = Encoding.UTF8.GetBytes("Kaydara FBX Binary  \0");
        _fileStream.Write(str);
        _fileStream.Write(new byte[] {0x1A, 0x00});
        _fileStream.Write(Int32ToBytes(7300));
    }
}