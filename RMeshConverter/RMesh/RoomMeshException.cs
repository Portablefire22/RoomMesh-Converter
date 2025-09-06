namespace RMeshConverter.RMesh;

public class RoomMeshException : Exception
{
    public RoomMeshException(){}
    public RoomMeshException(string message): base(message) {}
    public RoomMeshException(string message, Exception innerException) : base(message, innerException) {}
}