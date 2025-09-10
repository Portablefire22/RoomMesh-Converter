using System.Numerics;

namespace RMeshConverter.XModel;

public class MeshNormals
{
    public List<Vector3> Normals;
    public List<MeshFace> FaceNormals;

    public MeshNormals(List<Vector3> normals, List<MeshFace> faceNormals)
    {
        Normals = normals;
        FaceNormals = faceNormals;
    }
}