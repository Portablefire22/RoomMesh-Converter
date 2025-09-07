using System.Text;

namespace RMeshConverter.Exporter;

public static class ExporterUtility
{
    public static byte[] StringToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}