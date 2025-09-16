namespace RMeshConverter.Exporter;

public class DmxException : Exception
{
    
    public DmxException(){}
    public DmxException(string message): base(message) {}
    public DmxException(string message, Exception innerException) : base(message, innerException) {}
}