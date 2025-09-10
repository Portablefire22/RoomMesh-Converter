using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using RMeshConverter.RMesh;

namespace RMeshConverter.Exporter;

/* Credit
 * https://gist.github.com/iscle/0dbcee58be8582978d15ea3629ce3e8b
 */

public class FbxRoomMeshExporter : MeshExporter
{
    public FbxRoomMeshExporter(RoomMeshReader reader, string inputFilePath, string name, string outputDirectory) : base(inputFilePath, name, outputDirectory)
    {
        OutputFileStream = File.Create($"{OutputDirectory}\\{Name}.fbx");
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = factory.CreateLogger<RoomMeshReader>();
    }

    
    // The following tobyte functions are just to ensure that I write the correct data types, nothing else
    private byte[] Int32ToBytes(int num)
    {
        return BitConverter.GetBytes(num);
    } 
    private byte[] UInt32ToBytes(uint num)
    {
        return BitConverter.GetBytes(num);
    }   
    
    private byte[] ULongToBytes(ulong num)
    {
        return BitConverter.GetBytes(num);
    }

    private void WriteNode(FbxNode node)
    {
        OutputFileStream.Write(ULongToBytes(node.EndOffset));
        OutputFileStream.Write(ULongToBytes((ulong)node.Properties.Length));
        OutputFileStream.Write(ULongToBytes(node.GetPropertiesSize()));
        OutputFileStream.WriteByte((byte)node.Name.Length);
        OutputFileStream.Write(Encoding.UTF8.GetBytes(node.Name));
        foreach (var child in node.Children)
        {
            WriteNode(child);
        }
        OutputFileStream.Write(new byte[13]);
    }

    private void WriteFoot()
    {
        OutputFileStream.Write("\xfa\xbc\xab\x09\xd0\xc8\xd4\x66\xb1\x76\xfb\x83\x1c\xf7\x26\x7e"u8.ToArray());
    }
    private void WriteHeader()
    {
        var str = "Kaydara FBX Binary\x20\x20\x00\x1a\x00"u8.ToArray();
        OutputFileStream.Write(str);
        OutputFileStream.Write(UInt32ToBytes(7500));
        // Binary format has an entire empty node for some reason
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override void Convert()
    {
        WriteHeader();
        WriteNode(new FbxNode("", [], [new FbxNode("Nyaa", [], [])]));
        WriteFoot();
        OutputFileStream.Write("\0\0\0\0"u8.ToArray());
        
        OutputFileStream.Close();
    }
}