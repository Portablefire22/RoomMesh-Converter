namespace RMeshConverter.XModel;

public class XException : Exception
{
    public XException(){}
    public XException(string message): base(message) {}
    public XException(string message, Exception innerException) : base(message, innerException) {}
}